namespace iSystem.Devices.BeneTech;

public class iMeasurementReceivedEventArgs : EventArgs
{
    internal iMeasurementReceivedEventArgs(int illuminance, double temperature)
    {
        this.Illuminance = illuminance;
        this.Temperature = temperature;
    }

    public int Illuminance { get; }
    public double Temperature { get; }
}