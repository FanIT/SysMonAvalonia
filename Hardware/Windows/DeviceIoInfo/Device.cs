namespace Hardware.Windows.DeviceIoInfo
{
    public class Device
    {
        public string Letter { get; set; }
        public VendorEnum Vendor;
        public string Model { get; set; }
        public byte DeviceID { get; set; }
        public long TotalSpace { get; set; }
        public long FreeSpace { get; set; }
        public long UsedSpace { get; set; }
        public byte PercentUsedSpace { get; set; }
        public float TotalWrite { get; set; }
        public byte Health { get; set; }
        public bool IsSmart { get; set; }

        public Device()
        {
            IsSmart = true;
        }
    }
}
