using static System.Diagnostics.Debug;

namespace iSystem.Threading.Tasks;

public class TaskManager
{
	private readonly object _locker = new();

	public bool IsTaskContinued => !this.TaskTokenSource.Token.IsCancellationRequested;

	public bool IsTaskReady { get; private set; } = true;

	public string TaskName { get; private set; } = "Auto-Run Task";

	public CancellationTokenSource TaskTokenSource { get; private set; } = new();

	private string CompletedHint => $"[{this.TaskName}] is completed.";

	private string InterruptedHint => $"[{this.TaskName}] is interrupted";

	//private static AutoResetEvent AutoRestPauseEvent { get; } = new(false);

	private ManualResetEvent ManualResetPauseEvent { get; } = new(true);

	private string StartedHint => $"[{this.TaskName}] has started...";

	private string UnCompletedHint => $"[{this.TaskName}] is not yet complete!";

	public static void WakeableSleep(CancellationTokenSource tokenSource, TimeSpan timeSpan)
	{
		if (timeSpan == TimeSpan.Zero) return;

		tokenSource.Token.WaitHandle.WaitOne(timeSpan);
	}

	public static void WakeableSleep(CancellationTokenSource tokenSource, int milliseconds)
	{
		if (milliseconds <= 0) return;

		tokenSource.Token.WaitHandle.WaitOne(milliseconds);
	}

	public void ManualPauseTask()
	{
		this.ManualResetPauseEvent.Reset();
		WriteLine("Pause task is activated.");
	}

	public void ManualResumeTask()
	{
		//To set the state of the event to signaled, allowing one or more waiting threads to proceed.
		this.ManualResetPauseEvent.Set();
		WriteLine("Task has been resumed.");
	}

	public void RunTask(Action action)
	{
		this.RunTask(action, action.Method.Name);
	}

	public void RunTask(Action action, string taskName)
	{
		if (!this.IsTaskReady)
		{
			WriteLine(this.UnCompletedHint);

			return;
		}

		lock (this._locker)
		{
			this.IsTaskReady = false;
			this.TaskTokenSource = new CancellationTokenSource();
			this.TaskName = taskName;
		}

		Task.Run(MyAction, this.TaskTokenSource.Token).ContinueWith(t =>
		{
			t.Exception?.Handle(_ => true);
			Console.WriteLine(this.InterruptedHint);
		}, TaskContinuationOptions.OnlyOnCanceled);
		return;

		void MyAction()
		{
			try
			{
				WriteLine(this.StartedHint);
				action();
			}
			catch (Exception exception)
			{
				WriteLine(exception.Message);
			}
			finally
			{
				this.TaskTokenSource.Cancel();
				this.IsTaskReady = true;
				WriteLine(this.CompletedHint);
			}
		}
	}

	public void RunTask(Action<object[]> action, object[] objects)
	{
		this.RunTask(action, objects, action.Method.Name);
	}

	public void RunTask(Action<object[]> action, object[] objects, string taskName)
	{
		if (!this.IsTaskReady)
		{
			WriteLine(this.UnCompletedHint);

			return;
		}

		lock (this._locker)
		{
			this.IsTaskReady = false;
			this.TaskTokenSource = new CancellationTokenSource();
			this.TaskName = taskName;
		}

		Task.Run(MyAction, this.TaskTokenSource.Token).ContinueWith(t =>
		{
			t.Exception?.Handle(_ => true);
			WriteLine(this.InterruptedHint);
		}, TaskContinuationOptions.OnlyOnCanceled);
		return;

		void MyAction()
		{
			try
			{
				WriteLine(this.StartedHint);
				action(objects);
			}
			catch (Exception exception)
			{
				WriteLine(exception.Message);
			}
			finally
			{
				this.TaskTokenSource.Cancel();
				this.IsTaskReady = true;
				WriteLine(this.CompletedHint);
			}
		}
	}

	public void RunTask(Func<Task> func)
	{
		this.RunTask(func, func.Method.Name);
	}

	public void RunTask(Func<Task> func, string taskName)
	{
		if (!this.IsTaskReady)
		{
			WriteLine(this.UnCompletedHint);

			return;
		}

		lock (this._locker)
		{
			this.IsTaskReady = false;
			this.TaskTokenSource = new CancellationTokenSource();
			this.TaskName = taskName;
		}

		Task.Run(MyFunction, this.TaskTokenSource.Token).ContinueWith(t =>
		{
			t.Exception?.Handle(_ => true);
			WriteLine(this.InterruptedHint);
		}, TaskContinuationOptions.OnlyOnCanceled);
		return;

		void MyFunction()
		{
			try
			{
				WriteLine(this.StartedHint);
				func.Invoke().Wait();
			}
			catch (Exception exception)
			{
				WriteLine(exception.Message);
			}
			finally
			{
				this.TaskTokenSource.Cancel();
				this.IsTaskReady = true;
				WriteLine(this.CompletedHint);
			}
		}
	}

	public void RunTask(Func<object[], Task> func, object[] objects)
	{
		this.RunTask(func, objects, func.Method.Name);
	}

	public void RunTask(Func<object[], Task> func, object[] objects, string taskName)
	{
		if (!this.IsTaskReady)
		{
			WriteLine(this.UnCompletedHint);

			return;
		}

		lock (this._locker)
		{
			this.IsTaskReady = false;
			this.TaskTokenSource = new CancellationTokenSource();
			this.TaskName = taskName;
		}

		Task.Run(MyFunction, this.TaskTokenSource.Token).ContinueWith(t =>
		{
			t.Exception?.Handle(_ => true);
			WriteLine(this.InterruptedHint);
		}, TaskContinuationOptions.OnlyOnCanceled);
		return;

		void MyFunction()
		{
			try
			{
				WriteLine(this.StartedHint);
				func.Invoke(objects).Wait();
			}
			catch (Exception exception)
			{
				WriteLine(exception.Message);
			}
			finally
			{
				this.TaskTokenSource.Cancel();
				this.IsTaskReady = true;
				WriteLine(this.CompletedHint);
			}
		}
	}

	public void StartNew(Action action)
	{
		if (!this.IsTaskReady)
		{
			WriteLine(this.UnCompletedHint);

			return;
		}

		lock (this._locker)
		{
			this.IsTaskReady = false;
			this.TaskTokenSource = new CancellationTokenSource();
		}

		Task.Factory.StartNew(MyAction, this.TaskTokenSource.Token).ContinueWith(t =>
		{
			t.Exception?.Handle(_ => true);
			WriteLine(this.InterruptedHint);
		}, TaskContinuationOptions.OnlyOnCanceled);
		return;

		void MyAction()
		{
			try
			{
				WriteLine(this.StartedHint);
				action();
			}
			catch (Exception exception)
			{
				WriteLine(exception.Message);
			}
			finally
			{
				this.TaskTokenSource.Cancel();
				this.IsTaskReady = true;
				WriteLine(this.CompletedHint);
			}
		}
	}

	public void StopTask()
	{
		this.ManualResumeTask();
		this.TaskTokenSource.Cancel();
	}

	public void TogglePlayPause()
	{
		if (this.ManualResetPauseEvent.WaitOne(0))
			this.ManualPauseTask();
		else
			this.ManualResumeTask();
	}

	public void WaitToContinue()
	{
		this.ManualResetPauseEvent.WaitOne(Timeout.Infinite);
	}

	public void WakeableSleep(TimeSpan timeSpan)
	{
		if (timeSpan == TimeSpan.Zero) return;

		this.TaskTokenSource.Token.WaitHandle.WaitOne(timeSpan);
	}

	public void WakeableSleep(int milliseconds)
	{
		if (milliseconds <= 0) return;

		this.TaskTokenSource.Token.WaitHandle.WaitOne(milliseconds);
	}
}