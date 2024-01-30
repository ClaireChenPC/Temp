// ReSharper disable UnusedMember.Global

namespace iSystem.Threading.Tasks;

/// <summary>
///   Provides a task scheduler that ensures a maximum concurrency level while running on top of the ThreadPool.
/// </summary>
public class LimitedConcurrencyLevelTaskScheduler : TaskScheduler
{
    /// <summary>Whether the current thread is processing work items.</summary>
    [ThreadStatic] private static bool _currentThreadIsProcessingItems;

    /// <summary>The maximum concurrency level allowed by this scheduler.</summary>
    private readonly int _maxDegreeOfParallelism;

    /// <summary>The list of tasks to be executed.</summary>
    private readonly LinkedList<Task> _tasks = new(); // protected by lock(_tasks)

    /// <summary>Whether the scheduler is currently processing work items.</summary>
    private int _delegatesQueuedOrRunning; // protected by lock(_tasks)

    /// <summary>
    ///   Initializes an instance of the LimitedConcurrencyLevelTaskScheduler class with the
    ///   specified degree of parallelism.
    /// </summary>
    /// <param name="maxDegreeOfParallelism">The maximum degree of parallelism provided by this scheduler.</param>
    public LimitedConcurrencyLevelTaskScheduler(int maxDegreeOfParallelism)
    {
        if (maxDegreeOfParallelism < 1) throw new ArgumentOutOfRangeException(nameof(maxDegreeOfParallelism));
        this._maxDegreeOfParallelism = maxDegreeOfParallelism;
    }

    /// <summary>Gets the maximum concurrency level supported by this scheduler.</summary>
    public sealed override int MaximumConcurrencyLevel => this._maxDegreeOfParallelism;

    public Task CurrentTask { get; private set; } = null!;

    /// <summary>Gets an enumerable of the tasks currently scheduled on this scheduler.</summary>
    /// <returns>An enumerable of the tasks currently scheduled.</returns>
    protected sealed override IEnumerable<Task> GetScheduledTasks()
    {
        var lockTaken = false;
        try
        {
            Monitor.TryEnter(this._tasks, ref lockTaken);
            if (lockTaken) return this._tasks.ToArray();
            else throw new NotSupportedException();
        }
        finally
        {
            if (lockTaken) Monitor.Exit(this._tasks);
        }
    }

    /// <summary>Queues a task to the scheduler.</summary>
    /// <param name="task">The task to be queued.</param>
    protected sealed override void QueueTask(Task task)
    {
        // Add the task to the list of tasks to be processed.  If there aren't enough
        // delegates currently queued or running to process tasks, schedule another.
        lock (this._tasks)
        {
            this._tasks.AddLast(task);
            if (this._delegatesQueuedOrRunning >= this._maxDegreeOfParallelism) return;
            ++this._delegatesQueuedOrRunning;
            this.NotifyThreadPoolOfPendingWork();
        }
    }

    /// <summary>Attempts to remove a previously scheduled task from the scheduler.</summary>
    /// <param name="task">The task to be removed.</param>
    /// <returns>Whether the task could be found and removed.</returns>
    protected sealed override bool TryDequeue(Task task)
    {
        lock (this._tasks)
        {
            return this._tasks.Remove(task);
        }
    }

    /// <summary>Attempts to execute the specified task on the current thread.</summary>
    /// <param name="task">The task to be executed.</param>
    /// <param name="taskWasPreviouslyQueued"></param>
    /// <returns>Whether the task could be executed on the current thread.</returns>
    protected sealed override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
    {
        // If this thread isn't already processing a task, we don't support inlining.
        if (!_currentThreadIsProcessingItems) return false;

        // If the task was previously queued, remove it from the queue
        if (taskWasPreviouslyQueued) this.TryDequeue(task);

        // Try to run the task.
        return this.TryExecuteTask(task);
    }

    /// <summary>
    ///   Informs the ThreadPool that there's work to be executed for this scheduler.
    /// </summary>
    private void NotifyThreadPoolOfPendingWork()
    {
        ThreadPool.UnsafeQueueUserWorkItem(_ =>
        {
            // Note that the current thread is now processing work items.
            // This is necessary to enable inlining of tasks into this thread.
            _currentThreadIsProcessingItems = true;
            try
            {
                // Process all available items in the queue.
                while (true)
                {
                    lock (this._tasks)
                    {
                        // When there are no more items to be processed,
                        // note that we're done processing, and get out.
                        if (this._tasks.Count == 0)
                        {
                            --this._delegatesQueuedOrRunning;
                            break;
                        }

                        // Get the next item from the queue
                        var linkedListNode = this._tasks.First;
                        if (linkedListNode != null) this.CurrentTask = linkedListNode.Value;
                        this._tasks.RemoveFirst();
                    }

                    // Execute the task we pulled out of the queue
                    this.TryExecuteTask(this.CurrentTask);
                }
            }

            // We're done processing items on the current thread
            finally
            {
                _currentThreadIsProcessingItems = false;
            }
        }, null);
    }
}