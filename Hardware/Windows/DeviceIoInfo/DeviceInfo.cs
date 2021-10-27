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

                device.Model = device.DeviceControl.Disc.Smart == null ? new DriveInfo(logical.Name).VolumeLabel : 
                    device.DeviceControl.Disc.Smart.SystemParams.ModelNumber.Trim(' ');

                if (device.Model.Contains("samsung", StringComparison.OrdinalIgnoreCase)) 
                    device.Vendor = VendorEnum.Samsung;
                else if (device.Model.Contains("kingston", StringComparison.OrdinalIgnoreCase)) 
                    device.Vendor = VendorEnum.Kingston;
                else if (device.Model.Contains("wdc", StringComparison.OrdinalIgnoreCase)) 
                    device.Vendor = VendorEnum.WDC;
                else 
                    device.Vendor = VendorEnum.Other;

                device.DevicePerformance = device.DeviceControl.Disc.GetDiscPerformance();
                device.PrevReadIo = device.DevicePerformance.QueryPerformanceInfo().BytesRead;
                device.PrevWriteIo = device.DevicePerformance.QueryPerformanceInfo().BytesWritten;
                device.TotalSpace = new DriveInfo(logical.Name).TotalSize;

                LogicalDrives.Add(device);

                devIndex++;
            }
        }

        public static bool IsUpdateDiscs()
        {
            if (LogicalDrives.Count == DeviceIoControl.GetLogicalDrives().Count()) return false;
            
            SetDiscCollection();

            return true;

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
