using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Controls;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using ReactiveUI;
using SysMonAvalonia.Models;
using SysMonAvalonia.Data;
using SysMonAvalonia.Services;

namespace SysMonAvalonia.ViewModels
{
    public class DeviceIoWidgetViewModel : ViewModelBase
    {
        private readonly DeviceIoModel _deviceIoModel;
        private readonly ChartModel _chartModel;
        private IDisposable _timer;
        
        private byte _updCountCollection;
        private byte _updCountSpace;
        private byte _updCountInfo;

        public ReactiveCommand<Unit, Unit> ShutdownCommand { get; } = ReactiveCommand.Create(() => AppService.ShutdownApp());

        public ReactiveCommand<Unit, Unit> ShowSettingsCommand { get; } =
            ReactiveCommand.Create(() => AppService.ShowSettings());

        public ReactiveCommand<EventArgs, Unit> ClosedCommand { get; }
        public ReactiveCommand<PixelPointEventArgs, Unit> MoveWidgetCommand { get; }

        public ReadOnlyObservableCollection<DeviceIoData> DeviceCollection => _deviceIoModel.DeviceCollection;

        public ISeries[] Series
        {
            get => _chartModel.Series;
        }
        public Axis[] XAxis
        {
            get => _chartModel.XAxis;
        }
        public Axis[] YAxis
        {
            get => _chartModel.YAxis;
        }

        public double ChartValue
        {
            set => _chartModel.Value = value;
        }

        public DeviceIoWidgetViewModel()
        {
            _updCountCollection = 0;
            _updCountInfo = 0;
            _updCountSpace = 0;
            
            _deviceIoModel = new();
            _chartModel = new();
            
            MoveWidgetCommand = ReactiveCommand.Create<PixelPointEventArgs>(e =>
            {
                SettingData.CurrentSetting.DeviceIoWidget.X = e.Point.X;
                SettingData.CurrentSetting.DeviceIoWidget.Y = e.Point.Y;
            });
                
            ClosedCommand = ReactiveCommand.Create<EventArgs, Unit>(e =>
            {
                _deviceIoModel.Dispose();
                return Unit.Default;
            });

            _timer = Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1)).Subscribe(x => UpdateTimer());
        }

        private void UpdateTimer()
        {
            if (_updCountCollection == 0)
            {
                _deviceIoModel.UpdateDeviceCollection();

                _updCountCollection = 180;
            }

            _deviceIoModel.UpdatePerformance();

            if (_updCountSpace == 0)
            {
                _deviceIoModel.UpdateDeviceSpace();
                _updCountSpace = 60;
            }

            if (_updCountInfo == 0)
            {
                _deviceIoModel.UpdateDeviceInfo();
                _updCountInfo = 120;
            }

            _updCountCollection--;
            _updCountSpace--;
            _updCountInfo--;
        }
    }
}
