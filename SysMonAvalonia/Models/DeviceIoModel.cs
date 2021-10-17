using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using Avalonia;
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
                    device.Index = dev.Index;
                    device.Model = dev.Model;
                    device.Letter = dev.Letter;
                    device.DeviceChart.Fill = Application.Current.FindResource("DeviceChartFill") as SolidColorPaint;
                    
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
                devData.TotalSize = DeviceInfo.LogicalDrives[devData.Index].TotalSpace;
                devData.UsedSize = DeviceInfo.LogicalDrives[devData.Index].UsedSpace;
                devData.PercentUsed = DeviceInfo.LogicalDrives[devData.Index].PercentUsedSpace;
            }
        }

        public void UpdateDeviceInfo()
        {
            DeviceInfo.UpdateInfoOfDiscs();

            foreach (DeviceIoData devData in _deviceList.Items)
            {
                devData.Health = DeviceInfo.LogicalDrives[devData.Index].Health;
                devData.TotalWrite = DeviceInfo.LogicalDrives[devData.Index].TotalWrite;

                devData.IsHealth = devData.Health != 0;
                devData.IsTotalWrite = devData.TotalWrite != 0;
            }
        }

        public void UpdatePerformance()
        {
            foreach (DeviceIoData dId in _deviceList.Items)
            {
                DeviceInfo.PerfomanceDiskUpdate(dId.Index);

                dId.ReadIO = DeviceInfo.ReadIo;
                dId.WriteIO = DeviceInfo.WriteIo;
                dId.DeviceChart.Value = DeviceInfo.ReadIo + DeviceInfo.WriteIo;
            }
        }

        private bool ExistDeviceInCollection(Device device)
        {
            return _deviceList.Items.Any(did => did.Model.Contains(device.Model) && did.TotalSize == device.TotalSpace);
        }

        private void RemoveOldDeviceInCollection(IReadOnlyCollection<Device> device)
        {
            short index = 0;

            foreach (DeviceIoData item in _deviceList.Items)
            {
                if (!device.Any(dev => dev.Model.Contains(item.Model) && dev.TotalSpace == item.TotalSize))
                    _deviceList.RemoveAt(index);
                
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
