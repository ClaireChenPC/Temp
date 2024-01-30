namespace iSystem.Devices.Keysight;

public class KsFgInformationEventArgs : EventArgs
{
    public KsFgInformationEventArgs(EFgItem item, string value)
    {
        this.Item = item;
        this.Value = value;
    }

    public EFgItem Item { get; set; }
    public string Value { get; set; }
}