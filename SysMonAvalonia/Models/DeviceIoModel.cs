using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive.Linq;
using Avalonia.Controls;
using DynamicData;
using Hardware.Windows.DeviceIoInfo;
using LiveChartsCore.SkiaSharpView.Painting;
using ReactiveUI;
using SysMonAvalonia.Data;

namespace SysMonAvalonia.Models
{
    public class DeviceIoModel : IDisposable
    {
        private readonly SourceList<DeviceIoData> _deviceList;
        public ReadOnlyObservableCollection<DeviceIoData> DeviceCollection;

        public DeviceIoModel()
        {
            _deviceList = new();
            _deviceList.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(out DeviceCollection).Subscribe();
        }

        public void UpdateDeviceCollection()
        {
            if (DeviceInfo.IsUpdateDiscs())
            {
                UpdateDeviceSpace();

                List<Device> devices = DeviceInfo.LogicalDrives;

                if (_deviceList.Count > devices.Count)
                    RemoveOldDeviceInCollection(devices);

                foreach (Device dev in devices)
                {
                    if (ExistDeviceInCollection(dev)) continue;

                    DeviceIoData device = new();
                    device.Model = dev.Model;
                    device.Letter = dev.Letter;
                    device.DeviceChart.Fill = App.Current.FindResource("DeviceChartFill") as SolidColorPaint;
                    
                    _deviceList.Add(device);
                }
            }
        }

        public void UpdateDeviceSpace()
        {
            try
            {
                DeviceInfo.UpdateSizeOfDiscs();
            }
            catch (DriveNotFoundException)
            {
                UpdateDeviceCollection();
                DeviceInfo.UpdateSizeOfDiscs();
            }

            foreach (DeviceIoData devData in _deviceList.Items)
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

            foreach (DeviceIoData devData in _deviceList.Items)
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
            byte count = 0;
            
            foreach (DeviceIoData dId in _deviceList.Items)
            {
                DeviceInfo.PerfomanceDiskUpdate(count);

                dId.ReadIO = DeviceInfo.ReadIO;
                dId.WriteIO = DeviceInfo.WriteIO;
                dId.DeviceChart.Value = DeviceInfo.ReadIO + DeviceInfo.WriteIO;
                
                count++;
            }
        }

        private bool ExistDeviceInCollection(Device device)
        {
            foreach (DeviceIoData dID in _deviceList.Items)
            {
                if (dID.Model.Contains(device.Model) && dID.TotalSize == device.TotalSpace)
                    return true;
            }

            return false;
        }

        private void RemoveOldDeviceInCollection(List<Device> device)
        {
            byte index = 0;

            foreach (DeviceIoData item in _deviceList.Items)
            {
                bool isExist = false;

                foreach (Device dev in device)
                {
                    if (dev.Model.Contains(item.Model) && dev.TotalSpace == item.TotalSize)
                    {
                        isExist = true;
                        break;
                    }
                }

                if (!isExist) _deviceList.RemoveAt(index);

                index++;
            }
        }

        public void Dispose()
        {
            DeviceInfo.Close();
            _deviceList.Dispose();
        }
    }
}
