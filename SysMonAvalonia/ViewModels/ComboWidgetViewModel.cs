using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using LiveChartsCore.SkiaSharpView.Painting;
using ReactiveUI;
using SysMonAvalonia.Models;
using SysMonAvalonia.Data;
using SysMonAvalonia.Services;

namespace SysMonAvalonia.ViewModels
{
    public class ComboWidgetViewModel
    {
        private ComboModel _comboModel;
        private ComboData _comboData;
        private IDisposable _timer;
        
        public ComboData ComboData
        {
            get => _comboData;
        }
        public ChartModel CpuChartModel { get; }
        public ChartModel RamChartModel { get; }
        public ChartModel GpuChartModel { get; }
        public ChartModel NetDownChartModel { get; }
        public ChartModel NetUpChartModel { get; }

        public ReactiveCommand<Unit, Unit> ShowSettingsCommand { get; } =
            ReactiveCommand.Create(() => AppService.ShowSettings());

        public ReactiveCommand<Unit, Unit> ShutdownCommand { get; } = ReactiveCommand.Create(() => AppService.ShutdownApp() );

        public ReactiveCommand<EventArgs, Unit> ClosedCommand { get; }

        public ReactiveCommand<PixelPointEventArgs, Unit> MoveWidgetCommand { get; } = ReactiveCommand.Create<PixelPointEventArgs>(e =>
        {
            SettingData.CurrentSetting.ComboWidget.X = e.Point.X;
            SettingData.CurrentSetting.ComboWidget.Y = e.Point.Y;
        });

        public ComboWidgetViewModel()
        {
            _comboModel = new();
            _comboData = _comboModel.ComboData;
            _comboModel.ComputerDataUpdate();

            CpuChartModel = new();
            CpuChartModel.Fill = App.Current.FindResource("CpuChartFill") as SolidColorPaint;

            RamChartModel = new();
            RamChartModel.Fill = App.Current.FindResource("RamChartFill") as SolidColorPaint;
            RamChartModel.ClearPoints();
            RamChartModel.FillPoints(_comboData.RamPercent);

            GpuChartModel = new();
            GpuChartModel.Fill = App.Current.FindResource("GpuChartFill") as SolidColorPaint;

            NetDownChartModel = new();
            NetDownChartModel.Fill = App.Current.FindResource("NetDownChartFill") as SolidColorPaint;

            NetUpChartModel = new();
            NetUpChartModel.Fill = App.Current.FindResource("NetUpChartFill") as SolidColorPaint;

            _comboData.WhenAnyValue(x => x.CpuUsedPercent).Subscribe(percent => CpuChartModel.Value = percent);
            _comboData.WhenAnyValue(x => x.RamPercent).Subscribe(percent => RamChartModel.Value = percent);
            _comboData.WhenAnyValue(x => x.GpuUsedPercent).Subscribe(percent => GpuChartModel.Value = percent);
            _comboData.WhenAnyValue(x => x.GetByteNet).Subscribe(getByte => NetDownChartModel.Value = getByte);
            _comboData.WhenAnyValue(x => x.SentByteNet).Subscribe(sentByte => NetUpChartModel.Value = sentByte);

            _timer = Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1))
                .Subscribe(x => _comboModel.ComputerDataUpdate());

            ClosedCommand = ReactiveCommand.Create<EventArgs>(e =>
            {
                _comboModel.Dispose();
                _timer.Dispose();
            });
        }
    }
}
