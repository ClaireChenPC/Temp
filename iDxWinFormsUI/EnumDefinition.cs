namespace iDxWinFormsUI;

public enum EDdsCalLimitColor : uint
{
	Black = 0,
	White
}

public enum EEngineeringModeCalStateMask : byte
{
	CmdError = 0x01,
	AutoCalFinish = 0x02,
	SpiI2CFinish = 0x04,
	ScanFinish = 0x08
}