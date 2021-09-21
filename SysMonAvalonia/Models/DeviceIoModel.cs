using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Hardware.Windows.DeviceIoInfo;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using SysMonAvalonia.Data;

namespace SysMonAvalonia.Models
{
    public class DeviceIoModel : IDisposable
    {
        public ObservableCollection<DeviceIoData> DeviceCollection { get; set; } = new();

        public void UpdateDeviceCollection()
        {
            if (DeviceInfo.IsUpdateDiscs())
            {
                List<Device> devices = DeviceInfo.LogicalDrives;

                RemoveOldDeviceInCollection(devices);

                foreach (Device dev in devices)
                {
                    if (ExistDeviceInCollection(dev)) continue;

                    DeviceIoData device = new();
                    device.Model = dev.Model.Trim(' ');
                    device.Letter = dev.Letter;
                    device.DeviceChart.Fill = App.Current.FindResource("DeviceChartFill") as SolidColorPaint;
                    DeviceCollection.Add(device);
                }
            }
        }

        public void UpdateDeviceSpace()
        {
            DeviceInfo.UpdateSizeOfDiscs();

            foreach (DeviceIoData devData in DeviceCollection)
            {
                foreach (Device dev in DeviceInfo.LogicalDrives)
                {
                    if (devData.Letter.Contains(dev.Letter))
                    {
                        devData.TotalSize = dev.TotalSpace;
                        devData.UsedSize = dev.UsedSpace;
                        devData.PercentUsed = dev.PercentUsedSpace;
                        
                        break;
                    }
                }
            }
        }

        public void UpdateDeviceInfo()
        {
            DeviceInfo.UpdateInfoOfDiscs();

            foreach (DeviceIoData devData in DeviceCollection)
            {
                foreach (Device dev in DeviceInfo.LogicalDrives)
                {
                    if (devData.Letter.Contains(dev.Letter))
                    {
                        devData.Health = dev.Health;
                        devData.TotalWrite = dev.TotalWrite;

                        devData.IsHealth = dev.Health != 0;
                        devData.IsTotalWrite = dev.TotalWrite != 0;
                    }
                }
            }
        }

        public void UpdatePerformance()
        {
            for (byte b = 0; b < DeviceCollection.Count; b++)
            {
                DeviceInfo.PerfomanceDiskUpdate(b);

                DeviceCollection[b].ReadIO = DeviceInfo.ReadIO;
                DeviceCollection[b].WriteIO = DeviceInfo.WriteIO;
                DeviceCollection[b].DeviceChart.Value = DeviceInfo.ReadIO + DeviceInfo.WriteIO;
            }
        }

        private bool ExistDeviceInCollection(Device device)
        {
            bool isExist = false;

            foreach (DeviceIoData dev in DeviceCollection)
            {
                if (dev.Model.Contains(device.Model) && dev.TotalSize == device.TotalSpace)
                {
                    isExist = true;
                    break;
                }
            }

            return isExist;
        }

        private void RemoveOldDeviceInCollection(List<Device> device)
        {
            int countCollection = DeviceCollection.Count;

            for (byte b = 0; b < countCollection; b++)
            {
                bool isExist = true;

                foreach (Device dev in device)
                {
                    if (dev.Model.Contains(DeviceCollection[b].Model) && dev.TotalSpace == DeviceCollection[b].TotalSize)
                        isExist = false;
                }

                if (isExist)
                {
                    DeviceCollection.RemoveAt(b);
                    countCollection--;
                }
            }
        }

        public void Dispose()
        {
            DeviceInfo.Close();
            DeviceCollection.Clear();
        }
    }
}
