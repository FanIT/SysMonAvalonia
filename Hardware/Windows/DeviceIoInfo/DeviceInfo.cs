using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using DeviceIOControl;
using DeviceIOControl.Native;
using DeviceIOControl.Disc.Smart;

namespace Hardware.Windows.DeviceIoInfo
{
    public static class DeviceInfo
    {
        public static float ReadIo { get; private set; }
        public static float WriteIo { get; private set; }

        public static List<Device> LogicalDrives { get; }
        
        static DeviceInfo()
        {
            LogicalDrives = new();
        }

        private static void SetDiscCollection()
        {
            Close();

            IEnumerable<LogicalDrive> logicalDevicesIo = DeviceIoControl.GetLogicalDrives();

            short devIndex = 0;
            
            foreach (LogicalDrive logical in logicalDevicesIo)
            {
                if (logical.Type != WinApi.DRIVE.FIXED) 
                    continue;

                Device device = new()
                {
                    Index = devIndex,
                    Letter = logical.Name,
                    DeviceControl = new DeviceIoControl(logical.Name)
                };
                device.DevicePerformance = device.DeviceControl.Disc.GetDiscPerformance();

                LogicalDrives.Add(device);

                devIndex++;
            }
            
            foreach (Device dev in LogicalDrives)
            {
                SmartInfoCollection smart = dev.DeviceControl.Disc.Smart;

                dev.Model = smart != null ? smart.SystemParams.ModelNumber.Trim(' ') : new DriveInfo(dev.Letter).VolumeLabel;

                if (dev.Model.Contains("samsung", StringComparison.OrdinalIgnoreCase)) dev.Vendor = VendorEnum.Samsung;
                else if (dev.Model.Contains("kingston", StringComparison.OrdinalIgnoreCase)) dev.Vendor = VendorEnum.Kingston;
                else if (dev.Model.Contains("wdc", StringComparison.OrdinalIgnoreCase)) dev.Vendor = VendorEnum.WDC;
                else dev.Vendor = VendorEnum.Other;
                
                DiscApi.DISK_PERFORMANCE diskPerformance = dev.DevicePerformance.QueryPerformanceInfo();

                dev.PrevReadIo = diskPerformance.BytesRead;
                dev.PrevWriteIo = diskPerformance.BytesWritten;
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

            foreach (Device dev in LogicalDrives)
            {
                SmartInfoCollection smart = dev.DeviceControl.Disc.Smart;

                if (smart == null) continue;

                DiscApi.DRIVEATTRIBUTE[] driveAttributes = smart.GetAttributes();

                foreach (DiscApi.DRIVEATTRIBUTE attribute in driveAttributes)
                {
                    if (dev.Vendor == VendorEnum.Samsung)
                    {
                        if (attribute.bAttrID == 177) dev.Health = attribute.bAttrValue;
                        if (attribute.bAttrID == 241) dev.TotalWrite = attribute.RawValue / (float)int.MaxValue;
                    }

                    if (dev.Vendor == VendorEnum.Kingston)
                    {
                        if (attribute.bAttrID == 231) dev.Health = attribute.bAttrValue;
                        if (attribute.bAttrID == 241) dev.TotalWrite = attribute.RawValue / 1024f;
                    }
                }
            }
        }

        public static void PerfomanceDiskUpdate(short index)
        {
            DiscApi.DISK_PERFORMANCE dIskPerformance = LogicalDrives[index].DevicePerformance.QueryPerformanceInfo();

            ReadIo = (dIskPerformance.BytesRead - LogicalDrives[index].PrevReadIo) / 1000000;
            WriteIo = (dIskPerformance.BytesWritten - LogicalDrives[index].PrevWriteIo) / 1000000;

            LogicalDrives[index].PrevReadIo = dIskPerformance.BytesRead;
            LogicalDrives[index].PrevWriteIo = dIskPerformance.BytesWritten;
        }

        public static void Close()
        {
            foreach (Device dev in LogicalDrives)
            {
                dev.Dispose();
            }
            
            LogicalDrives.Clear();
        }
    }
}
