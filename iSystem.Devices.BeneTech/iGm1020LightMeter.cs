using System.Diagnostics;
using HidLibrary;
using static System.Threading.Thread;

namespace iSystem.Devices.BeneTech;

// ReSharper disable once UnusedMember.Global
public class iGm1020LightMeter
{
    public const int InvalidIlluminance = 0;
    public const double InvalidTemperature = double.NaN;
    private const string ConnectionToken = "080000";
    private const string SubConnectionToken = "01FF";
    private const string ValueToken = "083322";
    private bool _connectionFlag = true;
    private HidDevice? _hidDevice;

    public iGm1020LightMeter()
    {
        this.Illuminance = InvalidIlluminance;
        this.Temperature = InvalidTemperature;
    }

    public delegate void DeviceConnectionChangedEventHandler(object source, iDeviceConnectionChangedEventArgs e);

    public delegate void MeasurementReceivedEventHandler(object source, iMeasurementReceivedEventArgs e);

    public event DeviceConnectionChangedEventHandler? DeviceConnectionChanged;
    public event MeasurementReceivedEventHandler? MeasurementReceived;
    public int Illuminance { get; private set; }
    public bool IsOpen { get; private set; }
    public double Temperature { get; private set; }

    public void Close()
    {
        if (!this.IsOpen) return;

        if (this._hidDevice is null) return;

        try
        {
            this._hidDevice.CloseDevice();
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception.Message);
        }

        if (this._hidDevice.IsOpen) return;

        this._connectionFlag = false;
        this.IsOpen = this._hidDevice.IsOpen;
        this.Illuminance = InvalidIlluminance;
        this.Temperature = InvalidTemperature;

        if (this.DeviceConnectionChanged is not null)
        {
            var eventArgs = new iDeviceConnectionChangedEventArgs(this.IsOpen);
            this.DeviceConnectionChanged(this, eventArgs);
        }

        if (this.MeasurementReceived is not null)
        {
            var eventArgs = new iMeasurementReceivedEventArgs(this.Illuminance, this.Temperature);
            this.MeasurementReceived(this, eventArgs);
        }
    }

    public void Open()
    {
        if (this.IsOpen) return;

        void OpenTask()
        {
            const int vendorId = 0x1A86;
            const int productId = 0xE010;
            this._hidDevice = HidDevices.Enumerate(vendorId, productId).FirstOrDefault() ?? null!;
            if (this._hidDevice is null)
            {
                Debug.WriteLine("No device connected");
                return;
            }

            var timeSlice = TimeSpan.FromMilliseconds(50);
            try
            {
                this._hidDevice.OpenDevice();
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
            }

            if (!this._hidDevice.IsOpen) return;

            this._connectionFlag = true;
            this._hidDevice.ReadReport(this.ReadReportCallback);

            const int ReportLength = 32;
            var data = new byte[ReportLength];

            var featureBuffer = new byte[] { 0x00, 0xff, 0xc7, 0x82, 0xd9, 0x30 };
            Array.Copy(featureBuffer, data, featureBuffer.Length);
            this._hidDevice.WriteFeatureData(data);
            Sleep(timeSlice);

            var reportBuffer = new byte[] { 0x00, 0x08, 0xbb, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xbb };
            Array.Copy(reportBuffer, data, reportBuffer.Length);
            this._hidDevice.WriteReport(new HidReport(ReportLength,
                new HidDeviceData(data, HidDeviceData.ReadStatus.WaitTimedOut)));
            Sleep(timeSlice);

            reportBuffer = new byte[] { 0x00, 0x08, 0x1e, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1e };
            Array.Copy(reportBuffer, data, reportBuffer.Length);
            this._hidDevice.WriteReport(new HidReport(ReportLength,
                new HidDeviceData(data, HidDeviceData.ReadStatus.WaitTimedOut)));
            Sleep(timeSlice);

            reportBuffer = new byte[] { 0x00, 0x08, 0x1e, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1e };
            Array.Copy(reportBuffer, data, reportBuffer.Length);
            this._hidDevice.WriteReport(new HidReport(ReportLength,
                new HidDeviceData(data, HidDeviceData.ReadStatus.WaitTimedOut)));
            Sleep(timeSlice);

            reportBuffer = new byte[] { 0x00, 0x08, 0x3c, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x3d };
            Array.Copy(reportBuffer, data, reportBuffer.Length);
            this._hidDevice.WriteReport(new HidReport(ReportLength,
                new HidDeviceData(data, HidDeviceData.ReadStatus.WaitTimedOut)));
        }

        Task.Factory.StartNew(OpenTask);
    }

    private void ReadReportCallback(HidReport report)
    {
        if (!this._connectionFlag) return;

        var s = BitConverter.ToString(report.Data).Substring(0, 26).Replace("-", string.Empty);

        //Debug.WriteLine(s);

        var token = s.Substring(0, 6);

        if (this.IsOpen)
        {
            if (token == ValueToken)
            {
                var illuminance = Convert.ToInt32(s.Substring(7, 3), 16);
                switch (s[6])
                {
                    case '8':
                        illuminance *= 10;
                        break;
                    case 'C':
                        illuminance *= 100;
                        break;
                }

                this.Illuminance = illuminance;

                this.Temperature = Convert.ToInt32(s.Substring(14, 2), 16) / 10d;

                if (this.MeasurementReceived is not null)
                {
                    var eventArgs = new iMeasurementReceivedEventArgs(this.Illuminance, this.Temperature);
                    this.MeasurementReceived(this, eventArgs);
                }

                //Debug.WriteLine(this.Illuminance + "   " + this.Temperature);
            }
        }
        else if (token == ConnectionToken)
        {
            var subToken = s.Substring(ConnectionToken.Length, SubConnectionToken.Length);

            if (subToken == SubConnectionToken) this.IsOpen = true;

            if (this.DeviceConnectionChanged is not null)
            {
                var eventArgs = new iDeviceConnectionChangedEventArgs(this.IsOpen);
                this.DeviceConnectionChanged(this, eventArgs);
            }
        }

        this._hidDevice?.ReadReport(this.ReadReportCallback);
    }
}