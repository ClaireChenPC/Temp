namespace iSystem.Devices.BeneTech;

public class iDeviceConnectionChangedEventArgs : EventArgs
{
    internal iDeviceConnectionChangedEventArgs(bool isOpen)
    {
        this.IsOpen = isOpen;
    }

    public bool IsOpen { get; }
}