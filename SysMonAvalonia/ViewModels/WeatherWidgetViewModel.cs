using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;
using SysMonAvalonia.Models;
using SysMonAvalonia.Data;
using SysMonAvalonia.Services;

namespace SysMonAvalonia.ViewModels
{
    public class WeatherWidgetViewModel : ViewModelBase
    {
        private readonly WeatherModel _weatherModel;
        private WeatherData _weatherData;
        private ObservableCollection<ForecastData> _forecastHourlyCollection;
        private ObservableCollection<ForecastData> _forecastDailyCollection;

        public ReactiveCommand<CancelEventArgs, Unit> ShutdownCommand { get; } = ReactiveCommand.Create<CancelEventArgs, Unit>(e =>
        {
            if (App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                desktop.Shutdown();
            return Unit.Default;
        });

        public ReactiveCommand<Unit, Unit> ShowSettingsCommand { get; } = ReactiveCommand.Create(() => AppService.ShowSettings());
        public ReactiveCommand<Unit, Unit> UpdateCommand { get; }
        public ReactiveCommand<PixelPointEventArgs, Unit> MoveCommand { get; }

        public WeatherData WeatherData
        {
            get => _weatherData; 
            set => this.RaiseAndSetIfChanged(ref _weatherData, value);
        }

        public ObservableCollection<ForecastData> ForecastHourlyCollection
        {
            get => _forecastHourlyCollection; 
            set => this.RaiseAndSetIfChanged(ref _forecastHourlyCollection, value);
        }

        public ObservableCollection<ForecastData> ForecastDailyCollection
        {
            get => _forecastDailyCollection; 
            set => this.RaiseAndSetIfChanged(ref _forecastDailyCollection, value);
        }

        private IDisposable Timer;

        public WeatherWidgetViewModel()
        {
            UpdateCommand = ReactiveCommand.Create(UpdateWeather);
            MoveCommand = ReactiveCommand.Create<PixelPointEventArgs>(e =>
            {
                SettingData.CurrentSetting.WeatherWidget.X = e.Point.X;
                SettingData.CurrentSetting.WeatherWidget.Y = e.Point.Y;
            });

            _weatherModel = new();

            Timer = Observable.Timer(TimeSpan.Zero, TimeSpan.FromMinutes(SettingData.CurrentSetting.WeatherInterval))
                .Subscribe(x => UpdateWeather());
        }
        
        private Unit Execute(CancelEventArgs e)
        {
            if (App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                desktop.Shutdown();

            return Unit.Default;
        }

        private void UpdateWeather()
        {
            WeatherData = _weatherModel.WeatherRefresh();
            ForecastHourlyCollection = _weatherModel.ForecastHourlyCollection;
            ForecastDailyCollection = _weatherModel.ForecastDailyCollection;
        }
    }
}
