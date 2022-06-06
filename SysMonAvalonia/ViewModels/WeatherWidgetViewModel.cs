using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using ReactiveUI;
using SysMonAvalonia.Models;
using SysMonAvalonia.Data;
using SysMonAvalonia.Services;

namespace SysMonAvalonia.ViewModels
{
    public class WeatherWidgetViewModel : ViewModelBase
    {
        private readonly WeatherModel _weatherModel;

        public ReactiveCommand<Unit, Unit> ShutdownCommand { get; } = ReactiveCommand.Create(() => AppService.ShutdownApp());
        public ReactiveCommand<Unit, Unit> ShowSettingsCommand { get; } = ReactiveCommand.Create(() => AppService.ShowSettings());
        public ReactiveCommand<Unit, Unit> UpdateCommand { get; }
        public ReactiveCommand<PixelPointEventArgs, Unit> MoveCommand { get; }
        public ReactiveCommand<EventArgs, Unit> ClosingCommand { get; }

        public WeatherData? CurrentWeather => _weatherModel.CurrentWeather;
        public ReadOnlyObservableCollection<ForecastData> ForecastHourlyCollection => _weatherModel.HourlyForecasts;
        public ReadOnlyObservableCollection<ForecastData> ForecastDailyCollection => _weatherModel.DailyForecasts;

        private IDisposable _timer;

        public WeatherWidgetViewModel()
        {
            UpdateCommand = ReactiveCommand.Create(WeatherRefresh);
            MoveCommand = ReactiveCommand.Create<PixelPointEventArgs>(e =>
            {
                if (e != null)
                {
                    SettingData.CurrentSetting.WeatherWidget.X = e.Point.X;
                    SettingData.CurrentSetting.WeatherWidget.Y = e.Point.Y;
                }
            });
            ClosingCommand = ReactiveCommand.Create<EventArgs, Unit>(e =>
            {
                _timer?.Dispose();
                return Unit.Default;
            });

            _weatherModel = new();
            
            _timer = Observable.Timer(TimeSpan.Zero, TimeSpan.FromMinutes(SettingData.CurrentSetting.WeatherInterval))
                .Subscribe(x => _weatherModel.WeatherRefresh());
        }

        private async void WeatherRefresh()
        {
            await Task.Run(() => _weatherModel.WeatherRefresh());
        }
    }
}
