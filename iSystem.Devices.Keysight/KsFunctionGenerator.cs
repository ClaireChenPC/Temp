using System.Diagnostics;
using System.Text;
using MMQ;
using SharedMemory;
using Vanara.Extensions;

// ReSharper disable UnusedMember.Global

namespace iSystem.Devices.Keysight;

public class KsFunctionGenerator
{
	//private int _commandsCount;
	public double[] Configurations = new double[(int)EFgConfig.Total];
	private double _amplitude;
	private IMemoryMappedQueueProducer? _commandProducer;
	private IMemoryMappedQueue? _commandQueue;
	private SharedArray<double>? _configMapProducer;
	private IMemoryMappedQueueConsumer? _dataConsumer;
	private int _dataCount;
	private IMemoryMappedQueue? _dataQueue;
	private Task? _dataTask;
	private double _frequency = 1000;
	private EWaveFormFunction _function = EWaveFormFunction.Sine;
	private double _highVoltage = 0.1;
	private bool _isTerminated;
	private double _lowVoltage;

	private double _offset;
	private bool _output;
	private double _phase;

	public event EventHandler<KsFgInformationEventArgs>? InformationUpdated;

	public double MaxAmplitude => this.MaxVoltage / 2d;

	public double Amplitude
	{
		get => this._amplitude;

		set
		{
			if (value < 0) return;
			if (value > this.MaxAmplitude) return;

			this.SendCommand($"{(int)EFgCmd.SetAmplitude},{value.ToString(Resources.ValueStringFormat)}");
		}
	}

	public double Frequency
	{
		get => this._frequency;

		set
		{
			switch (value)
			{
				case <= 0:
				case > 1000000:
					return;
				default:
					this.SendCommand($"{(int)EFgCmd.SetFrequency},{value.ToString(Resources.ValueStringFormat)}");
					break;
			}
		}
	}

	public EWaveFormFunction Function
	{
		get => this._function;

		set
		{
			if (value == EWaveFormFunction.Unknown) return;

			this.SendCommand($"{(int)EFgCmd.SetFunction},{value.GetDescription()}");
		}
	}

	public double HighVoltage
	{
		get => this._highVoltage;

		set
		{
			if (value < -this.MaxVoltage) return;
			if (value > this.MaxVoltage) return;

			this.SendCommand($"{(int)EFgCmd.SetHighVoltage},{value.ToString(Resources.ValueStringFormat)}");
		}
	}

	public string Identification { get; private set; } = string.Empty;

	public bool IsOpen { get; private set; }

	public double LowVoltage
	{
		get => this._lowVoltage;

		set
		{
			if (value < -this.MaxVoltage) return;
			if (value > this.MaxVoltage) return;

			this.SendCommand($"{(int)EFgCmd.SetLowVoltage},{value.ToString(Resources.ValueStringFormat)}");
		}
	}

	public double MaxVoltage { get; private set; } = 0.1;

	public double MinVoltage { get; private set; }

	public double Offset
	{
		get => this._offset;

		set
		{
			switch (value)
			{
				case < -5d:
				case > 5d:
					return;
				default:
					this.SendCommand($"{(int)EFgCmd.SetOffset},{value.ToString(Resources.ValueStringFormat)}");
					break;
			}
		}
	}

	public bool Output
	{
		get => this._output;

		set
		{
			var output = value ? "ON" : "OFF";

			this.SendCommand($"{(int)EFgCmd.SetOutput},{output}");
		}
	}

	public double Phase
	{
		get => this._phase;

		set
		{
			switch (value)
			{
				case < -360d:
				case > 360d:
					return;
				default:
					this.SendCommand($"{(int)EFgCmd.SetFrequency},{value.ToString(Resources.ValueStringFormat)}");
					break;
			}
		}
	}

	public void Close()
	{
		if (!this.IsOpen) return;

		this._isTerminated = true;

		if (this._dataTask != null)
		{
			this._dataTask.Wait();
			this._dataTask.Dispose();
		}

		this.IsOpen = false;
	}

	public void FocalConicTrigger()
	{
		this.SendCommand($"{(int)EFgCmd.FocalConicTrigger},");
	}

	public void Initialize()
	{
		try
		{
			var softwareTriggerDelay = Convert.ToDouble(Resources.SoftwareTriggerDelay);
			this._configMapProducer ??= new SharedArray<double>("iKSFunctionGeneratorConfigMap", (int)EFgConfig.Total);
			this.SetConfiguration(EFgConfig.WaveformHeadDelay, Convert.ToDouble(Resources.WaveformHeadDelay));
			this.SetConfiguration(EFgConfig.WaveformTailDelay,
				Convert.ToDouble(Resources.WaveformTailDelay) + softwareTriggerDelay);
			this.SetConfiguration(EFgConfig.HardwareTriggerDelay, Convert.ToDouble(Resources.HardwareTriggerDelay));
			this.SetConfiguration(EFgConfig.SoftwareTriggerDelay, softwareTriggerDelay);


			this._commandQueue ??= MemoryMappedQueue.Create("iKSFunctionGeneratorCmdQ");
			if (this._commandQueue is not null) this._commandProducer = this._commandQueue.CreateProducer();

			this._dataQueue ??= MemoryMappedQueue.Create("iKSFunctionGeneratorDataQ");
			if (this._dataQueue is not null) this._dataConsumer = this._dataQueue.CreateConsumer();
		}
		catch (Exception exception)
		{
			Debug.WriteLine(exception.Message);
		}
	}

	public void Open()
	{
		if (this.IsOpen) return;

		this._dataTask = Task.Run(this.ProcessData);

		this.IsOpen = true;
	}

	public void PlanarTrigger()
	{
		this.SendCommand($"{(int)EFgCmd.PlanarTrigger},");
	}

	public void QueryAmplitude()
	{
		this.SendCommand($"{(int)EFgCmd.QueryAmplitude}");
	}

	public void QueryFrequency()
	{
		this.SendCommand($"{(int)EFgCmd.QueryFrequency}");
	}

	public void QueryFunction()
	{
		this.SendCommand($"{(int)EFgCmd.QueryFunction}");
	}

	public void QueryHighVoltage()
	{
		this.SendCommand($"{(int)EFgCmd.QueryHighVoltage}");
	}

	public void QueryIdentification()
	{
		this.SendCommand($"{(int)EFgCmd.QueryIdentification}");
	}

	public void QueryLowVoltage()
	{
		this.SendCommand($"{(int)EFgCmd.QueryLowVoltage}");
	}

	public void QueryMaxVoltage()
	{
		this.SendCommand($"{(int)EFgCmd.QueryMaxVoltage}");
	}

	public void QueryMinVoltage()
	{
		this.SendCommand($"{(int)EFgCmd.QueryMinVoltage}");
	}

	public void QueryOffset()
	{
		this.SendCommand($"{(int)EFgCmd.QueryOffset}");
	}

	public void QueryOutput()
	{
		this.SendCommand($"{(int)EFgCmd.QueryOutput}");
	}

	public void SendCommand(string command)
	{
		if (string.IsNullOrEmpty(command)) return;

		if (command.Length > 128) return;

		if (this._commandProducer is null) return;

		var message = Encoding.UTF8.GetBytes(command);
		this._commandProducer.Enqueue(message);
		this.Configurations[(int)EFgConfig.CommandCount]++;
		this._configMapProducer?.Write(ref this.Configurations[(int)EFgConfig.CommandCount], 0);
	}

	public void SetConfiguration(EFgConfig item, double value)
	{
		if ((int)item <= (int)EFgConfig.Splitter) return;

		var index = (int)item;
		this.Configurations[index] = value;
		this._configMapProducer?.Write(ref this.Configurations[index], index);
	}

	public void SetFocalConicWaveform()
	{
		this.SendCommand($"{(int)EFgCmd.SetFocalConicWaveform},");
	}

	public void SetPlanarWaveform()
	{
		this.SendCommand($"{(int)EFgCmd.SetPlanarWaveform},");
	}

	protected virtual void OnInformationUpdated(KsFgInformationEventArgs e)
	{
		var handler = this.InformationUpdated;
		handler?.Invoke(this, e);
	}

	private void ProcessData()
	{
		var loaded = false;

		while (!this._isTerminated)
		{
			if (loaded)
				Thread.Sleep(TimeSpan.FromMilliseconds(1));
			else
				loaded = true;

			if (this._configMapProducer is null) continue;

			var dataCount = (int)this._configMapProducer[(int)EFgConfig.DataCount];

			if (dataCount == this._dataCount) continue;

			if (this._dataConsumer is null) continue;

			string text;
			try
			{
				var message = this._dataConsumer.Dequeue();
				if (message is null) continue;

				text = Encoding.UTF8.GetString(message);
			}
			catch (Exception e)
			{
				this._dataCount = dataCount;
				Debug.WriteLine(e.Message);
				continue;
			}

			var texts = text.Split(',');
			if (texts.Length < 2) continue;
			var cmdCode = Convert.ToInt32(texts[0]);
			var command = (EFgCmd)cmdCode;
			var data = texts[1];
			var item = EFgItem.Unknown;

			switch (command)
			{
				case EFgCmd.QueryIdentification:
					item = EFgItem.Identification;
					data = text[(text.IndexOf(',') + 1)..];
					this.Identification = data;
					break;

				case EFgCmd.QueryOutput:
					item = EFgItem.Output;
					this._output = data switch
					{
						"ON" => true,
						"OFF" => false,
						_ => this._output
					};
					break;

				case EFgCmd.QueryFunction:
					if (data == EWaveFormFunction.Sine.GetDescription())
						this._function = EWaveFormFunction.Sine;
					else if (data == EWaveFormFunction.Square.GetDescription())
						this._function = EWaveFormFunction.Square;
					else if (data == EWaveFormFunction.Ramp.GetDescription())
						this._function = EWaveFormFunction.Ramp;
					else if (data == EWaveFormFunction.Triangle.GetDescription())
						this._function = EWaveFormFunction.Triangle;
					else if (data == EWaveFormFunction.GaussianNoise.GetDescription())
						this._function = EWaveFormFunction.GaussianNoise;
					else if (data == EWaveFormFunction.Prbs.GetDescription())
						this._function = EWaveFormFunction.Prbs;
					else if (data == EWaveFormFunction.Arbitrary.GetDescription())
						this._function = EWaveFormFunction.Arbitrary;
					break;

				case EFgCmd.QueryFrequency:
					item = EFgItem.Frequency;
					this._frequency =  data.SafeParse(this._frequency);
					break;

				case EFgCmd.QueryPhase:
					item = EFgItem.Phase;
					this._phase = data.SafeParse(this._phase);
					break;

				case EFgCmd.QueryMaxVoltage:
					item = EFgItem.MaxVoltage;
					this.MaxVoltage = data.SafeParse(this.MaxVoltage);
					break;

				case EFgCmd.QueryMinVoltage:
					item = EFgItem.MinVoltage;
					this.MinVoltage = data.SafeParse(this.MinVoltage);
					break;

				case EFgCmd.QueryHighVoltage:
					item = EFgItem.HighVoltage;
					this._highVoltage = data.SafeParse(this._highVoltage);
					break;

				case EFgCmd.QueryLowVoltage:
					item = EFgItem.LowVoltage;
					this._lowVoltage = data.SafeParse(this._lowVoltage);
					break;

				case EFgCmd.QueryOffset:
					item = EFgItem.Offset;
					this._offset = data.SafeParse(this._offset);
					break;

				case EFgCmd.QueryAmplitude:
					item = EFgItem.Amplitude;
					this._amplitude = data.SafeParse(this._amplitude);
					break;

				case EFgCmd.SetOutput:
					break;

				case EFgCmd.SetFunction:
					break;

				case EFgCmd.SetFrequency:
					break;

				case EFgCmd.SetPhase:
					break;

				case EFgCmd.SetHighVoltage:
					break;

				case EFgCmd.SetLowVoltage:
					break;

				case EFgCmd.SetOffset:
					break;

				case EFgCmd.SetAmplitude:
					break;

				case EFgCmd.SetPlanarWaveform:
					break;

				case EFgCmd.PlanarTrigger:
					break;

				case EFgCmd.SetFocalConicWaveform:
					break;

				case EFgCmd.FocalConicTrigger:
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}

			var args = new KsFgInformationEventArgs(item, data);
			this.OnInformationUpdated(args);

			this._dataCount++;
		}
	}
}