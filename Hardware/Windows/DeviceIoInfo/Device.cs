using System;
using DeviceIOControl;
using DeviceIOControl.Disc;

namespace Hardware.Windows.DeviceIoInfo
{
    public class Device : IDisposable
    {
        public short Index { get; set; }
        public string Letter { get; set; }
        public VendorEnum Vendor;
        public string Model { get; set; }
        public long TotalSpace { get; set; }
        public long FreeSpace { get; set; }
        public long UsedSpace { get; set; }
        public byte PercentUsedSpace { get; set; }
        public float TotalWrite { get; set; }
        public byte Health { get; set; }
        public bool IsSmart { get; set; }
        public float PrevReadIo { get; set; }
        public float PrevWriteIo { get; set; }
        public DeviceIoControl DeviceControl { get; set; }
        public Performance DevicePerformance { get; set; }

        public Device()
        {
            IsSmart = true;
        }

        public void Dispose()
        {
            DevicePerformance?.Dispose();
            DeviceControl?.Dispose();
        }
    }
}
