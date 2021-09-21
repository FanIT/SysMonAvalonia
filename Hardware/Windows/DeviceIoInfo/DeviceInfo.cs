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

        public static List<Device> LogicalDrives { get; private set; }
        private static Dictionary<string, DeviceIoControl> DeviceIoList;
        private static int CountDiscs = 0;

        private static Performance[] Perfomances;
        private static float[] PrevReadValues;
        private static float[] PrevWriteValues;

        private static void SetDiscCollection()
        {
            LogicalDrives = new();
            DeviceIoList = new();

            IEnumerable<LogicalDrive> logicalDevicesIO = DeviceIoControl.GetLogicalDrives();

            CountDiscs = logicalDevicesIO.Count();

            byte index = 0;

            PrevReadValues = new float[logicalDevicesIO.Count()];
            PrevWriteValues = new float[PrevReadValues.Length];
            Perfomances = new Performance[PrevReadValues.Length];

            foreach (LogicalDrive logical in logicalDevicesIO)
            {
                if (logical.Type != WinApi.DRIVE.FIXED) continue;

                DeviceIoControl deviceIO = new(logical.Name);

                // if (!deviceIO.FileSystem.IsVolumeMounted || deviceIO.Storage.MediaTypesEx.DeviceType != StorageApi.STORAGE_DEVICE_NUMBER.FileDevice.DISK)
                // {
                //     deviceIO.Dispose();
                //     continue;
                // }

                Device device = new();
                device.Letter = logical.Name;

                var smart = deviceIO.Disc.Smart;

                if (smart != null) device.Model = smart.SystemParams.ModelNumber;
                else device.Model = new DriveInfo(logical.Name).VolumeLabel;

                if (device.Model.Contains("samsung", StringComparison.OrdinalIgnoreCase)) device.Vendor = VendorEnum.Samsung;
                else if (device.Model.Contains("kingston", StringComparison.OrdinalIgnoreCase)) device.Vendor = VendorEnum.Kingston;
                else if (device.Model.Contains("wdc", StringComparison.OrdinalIgnoreCase)) device.Vendor = VendorEnum.WDC;
                else device.Vendor = VendorEnum.Other;

                LogicalDrives.Add(device);

                DeviceIoList.Add(device.Letter, deviceIO);

                Perfomances[index] = deviceIO.Disc.GetDiscPerformance();

                DiscApi.DISK_PERFORMANCE dISK_PERFORMANCE = Perfomances[index].QueryPerformanceInfo();

                PrevReadValues[index] = dISK_PERFORMANCE.BytesRead;
                PrevWriteValues[index] = dISK_PERFORMANCE.BytesWritten;

                index++;
            }
        }

        public static bool IsUpdateDiscs()
        {
            if (CountDiscs != DeviceIoControl.GetLogicalDrives().Count())
            {
                SetDiscCollection();

                return true;
            }
            else
                return false;
        }

        public static void UpdateSizeOfDiscs()
        {
            if (LogicalDrives == null) SetDiscCollection();

            foreach (Device logic in LogicalDrives)
            {
                DriveInfo drive = new(logic.Letter);
                logic.TotalSpace = drive.TotalSize;
                logic.FreeSpace = drive.TotalFreeSpace;
                logic.UsedSpace = drive.TotalSize - drive.AvailableFreeSpace;
                logic.PercentUsedSpace = Convert.ToByte(logic.UsedSpace / (logic.TotalSpace / 100));
            }
        }

        public static void UpdateInfoOfDiscs()
        {
            if (LogicalDrives == null) SetDiscCollection();

            foreach (Device logical in LogicalDrives)
            {
                SmartInfoCollection smart = DeviceIoList[logical.Letter].Disc.Smart;

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
            DiscApi.DISK_PERFORMANCE dISK_PERFORMANCE = Perfomances[index].QueryPerformanceInfo();

            ReadIO = (dISK_PERFORMANCE.BytesRead - PrevReadValues[index]) / 1000000;
            WriteIO = (dISK_PERFORMANCE.BytesWritten - PrevWriteValues[index]) / 1000000;

            PrevReadValues[index] = dISK_PERFORMANCE.BytesRead;
            PrevWriteValues[index] = dISK_PERFORMANCE.BytesWritten;
        }

        public static void Close()
        {
            foreach (Performance performance in Perfomances)
            {
                performance?.Dispose();
            }

            foreach (DeviceIoControl device in DeviceIoList.Values)
            {
                device.Dispose();
            }
        }
    }
}
