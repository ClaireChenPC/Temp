using System.ComponentModel;

namespace iSystem.Devices.Keysight;

public enum EFgCmd
{
	QueryIdentification = 0,
	QueryOutput,
	QueryFunction,
	QueryFrequency,
	QueryPhase,
	QueryMaxVoltage,
	QueryMinVoltage,
	QueryHighVoltage,
	QueryLowVoltage,
	QueryOffset,
	QueryAmplitude,

	SetOutput = 100,
	SetFunction,
	SetFrequency,
	SetPhase,
	SetHighVoltage,
	SetLowVoltage,
	SetOffset,
	SetAmplitude,

	SetPlanarWaveform = 200,
	PlanarTrigger,

	SetFocalConicWaveform = 300,
	FocalConicTrigger
}

public enum EFgItem
{
	Unknown = -1,
	Identification = 0,
	Output,
	Function,
	Frequency,
	Phase,
	MaxVoltage,
	MinVoltage,
	HighVoltage,
	LowVoltage,
	Offset,
	Amplitude
}

public enum EFgConfig : uint
{
	CommandCount = 0,
	DataCount,

	Splitter,

	WaveformHeadDelay = 10,
	WaveformTailDelay,
	HardwareTriggerDelay,
	SoftwareTriggerDelay,

	#region Planar

	PlanarP0 = 20,
	PlanarN0,
	PlanarA0,

	PlanarT1 = 30,

	PlanarPn = 40,
	PlanarN,
	PlanarAn,
	PlanarIniA,
	PlanarFnlA,
	PlanarStepA,

	#endregion

	#region Focal Conic

	FocalConicP0 = 50,
	FocalConicN0,
	FocalConicA0,

	FocalConicT1 = 60,

	FocalConicP2 = 70,
	FocalConicN2,
	FocalConicA2,

	FocalConicT3 = 80,

	FocalConicPn = 90,
	FocalConicN,
	FocalConicAn,
	FocalConicIniA,
	FocalConicFnlA,
	FocalConicStepA,

	#endregion

	Total = 100
}

public enum EWaveFormFunction
{
	Unknown = -1,
	[Description("SIN")] Sine = 0,
	[Description("SQU")] Square,
	[Description("RAMP")] Ramp,
	[Description("TRI")] Triangle,
	[Description("NOIS")] GaussianNoise,
	[Description("PRBS")] Prbs,
	[Description("ARB")] Arbitrary
}