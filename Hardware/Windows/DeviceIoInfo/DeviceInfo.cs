using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using DeviceIOControl;
using DeviceIOControl.Native;
using DeviceIOControl.Disc;
using DeviceIOControl.Disc.Smart;

namespace Hardware.Windows.DeviceIoInfo
{
    public static class DeviceInfo
    {
        public static float ReadIO { get; private set; }
        public static float WriteIO { get; private set; }

        public static List<Device> LogicalDrives { get; }
        private static Dictionary<string, DeviceIoControl> _deviceIoList;
        
        private static Performance[] _perfomances;
        private static float[] _prevReadValues;
        private static float[] _prevWriteValues;

        static DeviceInfo()
        {
            LogicalDrives = new();
            _deviceIoList = new();
        }

        private static void SetDiscCollection()
        {
            LogicalDrives.Clear();
            _deviceIoList.Clear();

            IEnumerable<LogicalDrive> logicalDevicesIo = DeviceIoControl.GetLogicalDrives();

            foreach (LogicalDrive logical in logicalDevicesIo)
            {
                if (logical.Type != WinApi.DRIVE.FIXED) 
                    continue;

                Device device = new();
                device.Letter = logical.Name;

                LogicalDrives.Add(device);
                _deviceIoList.Add(device.Letter, new DeviceIoControl(device.Letter));
            }

            byte index = 0;

            _prevReadValues = new float[LogicalDrives.Count];
            _prevWriteValues = new float[LogicalDrives.Count];
            _perfomances = new Performance[LogicalDrives.Count];

            foreach (Device dev in LogicalDrives)
            {
                var smart = _deviceIoList[dev.Letter].Disc.Smart;

                if (smart != null) dev.Model = smart.SystemParams.ModelNumber.Trim(' ');
                else dev.Model = new DriveInfo(dev.Letter).VolumeLabel;

                if (dev.Model.Contains("samsung", StringComparison.OrdinalIgnoreCase)) dev.Vendor = VendorEnum.Samsung;
                else if (dev.Model.Contains("kingston", StringComparison.OrdinalIgnoreCase)) dev.Vendor = VendorEnum.Kingston;
                else if (dev.Model.Contains("wdc", StringComparison.OrdinalIgnoreCase)) dev.Vendor = VendorEnum.WDC;
                else dev.Vendor = VendorEnum.Other;
                
                _perfomances[index] = _deviceIoList[dev.Letter].Disc.GetDiscPerformance();

                DiscApi.DISK_PERFORMANCE diskPerformance = _perfomances[index].QueryPerformanceInfo();

                _prevReadValues[index] = diskPerformance.BytesRead;
                _prevWriteValues[index] = diskPerformance.BytesWritten;

                index++;
            }
        }

        public static bool IsUpdateDiscs()
        {
            if (LogicalDrives.Count != DeviceIoControl.GetLogicalDrives().Count())
            {
                SetDiscCollection();

                return true;
            }

            return false;
        }

        public static void UpdateSizeOfDiscs()
        {
            if (LogicalDrives == null) SetDiscCollection();

            foreach (Device logic in LogicalDrives)
            {
                DriveInfo drive = new(logic.Letter);
                try
                {
                    logic.TotalSpace = drive.TotalSize;
                    logic.FreeSpace = drive.TotalFreeSpace;
                    logic.UsedSpace = drive.TotalSize - drive.AvailableFreeSpace;
                    logic.PercentUsedSpace = Convert.ToByte(logic.UsedSpace / (logic.TotalSpace / 100));
                }
                catch (DriveNotFoundException)
                {
                    throw new DriveNotFoundException();
                }
            }
        }

        public static void UpdateInfoOfDiscs()
        {
            if (LogicalDrives == null) SetDiscCollection();

            foreach (Device logical in LogicalDrives)
            {
                SmartInfoCollection smart = _deviceIoList[logical.Letter].Disc.Smart;

                if (smart != null)
                {
                    DiscApi.DRIVEATTRIBUTE[] driveAttributes = smart.GetAttributes();

                    foreach (DiscApi.DRIVEATTRIBUTE attribute in driveAttributes)
                    {
                        if (logical.Vendor == VendorEnum.Samsung)
                        {
                            if (attribute.bAttrID == 177) logical.Health = attribute.bAttrValue;
                            if (attribute.bAttrID == 241) logical.TotalWrite = attribute.RawValue / (float)int.MaxValue;
                        }

                        if (logical.Vendor == VendorEnum.Kingston)
                        {
                            if (attribute.bAttrID == 231) logical.Health = attribute.bAttrValue;
                            if (attribute.bAttrID == 241) logical.TotalWrite = attribute.RawValue / 1024f;
                        }
                    }
                }
            }
        }

        public static void PerfomanceDiskUpdate(byte index)
        {
            DiscApi.DISK_PERFORMANCE dISK_PERFORMANCE = _perfomances[index].QueryPerformanceInfo();

            ReadIO = (dISK_PERFORMANCE.BytesRead - _prevReadValues[index]) / 1000000;
            WriteIO = (dISK_PERFORMANCE.BytesWritten - _prevWriteValues[index]) / 1000000;

            _prevReadValues[index] = dISK_PERFORMANCE.BytesRead;
            _prevWriteValues[index] = dISK_PERFORMANCE.BytesWritten;
        }

        public static void Close()
        {
            foreach (Performance performance in _perfomances)
            {
                performance?.Dispose();
            }

            foreach (DeviceIoControl device in _deviceIoList.Values)
            {
                device.Dispose();
            }
        }
    }
}
