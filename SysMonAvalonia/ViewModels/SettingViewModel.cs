using System;
using System.Collections.Generic;
using System.Reactive;
using ReactiveUI;
using SysMonAvalonia.Data;
using SysMonAvalonia.Models;
using SysMonAvalonia.Localization;
using Weather;
using Hardware.Windows.Network;
using SysMonAvalonia.Services;

namespace SysMonAvalonia.ViewModels
{
    public class SettingViewModel : ViewModelBase
    {
        private SettingModel _settingModel;
        private List<LocationData> _cityCollection;
        private string _textFind;
        private bool _isOpenPopup;
        private int _selectCity;
        private int _selectNetworkAdapter;

        public SettingData CurrentSettingData { get; }
        public string[] LanguageList { get; }
        public string[] ThemeList { get; }
        public string[] WeatherProviderList { get; }
        public string[] NetworkUnitList { get; }
        public List<LocationData> CityCollection
        {
            get => _cityCollection; 
            set => this.RaiseAndSetIfChanged(ref _cityCollection, value);
        }
        public List<Adapter> NetworkAdapterCollection
        {
            get => _settingModel.NetworkAdapters;
        }
        public string TextFind
        {
            get => _textFind;
            set => this.RaiseAndSetIfChanged(ref _textFind, value);
        }
        public bool IsOpenPopup
        {
            get => _isOpenPopup;
            set => this.RaiseAndSetIfChanged(ref _isOpenPopup, value);
        }
        public int SelectCity
        {
            get => _selectCity;
            set => this.RaiseAndSetIfChanged(ref _selectCity, value);
        }
        public int SelectNetworkAdapter
        {
            get => _selectNetworkAdapter;
            set => this.RaiseAndSetIfChanged(ref _selectNetworkAdapter, value);
        }

        public bool IsAutoStart
        {
            get => _settingModel.AutostartEnable; 
            set => _settingModel.AutostartEnable = value;
        }

        public ReactiveCommand<Unit, Unit> CloseCommand { get; } = ReactiveCommand.Create(() =>
        {
            SettingModel.Save();
            AppService.CloseSettings();
        });

        public SettingViewModel()
        {
            _settingModel = new();

            CurrentSettingData = SettingData.CurrentSetting;
            LanguageList = new[] { Language.Locale.English, Language.Locale.Russia };
            ThemeList = new[] { Language.Locale.Dark, Language.Locale.Light };
            WeatherProviderList = new[] { "AccuWeather", "Gismeteo", "OpenWeatherMap", "WorldWeatherOnline", "Weather.com" };
            NetworkUnitList = new[] { Language.Locale.MbitSettings, Language.Locale.MbyteSettings };
            IsOpenPopup = false;

            this.WhenAnyValue(x => x.TextFind).Subscribe(text =>
            {
                if (text != null && text.Length > 3) CityCollection = SettingModel.FindCities(text);

                if (CityCollection == null || CityCollection.Count == 0) IsOpenPopup = false;
                else IsOpenPopup = true;
            });

            this.WhenAnyValue(x => x.SelectCity).Subscribe(index =>
            {
                if (CityCollection == null) return;

                SettingData.CurrentSetting.WeatherCity = CityCollection[index].City;
                SettingData.CurrentSetting.WeatherID = CityCollection[index].ID;

                IsOpenPopup = false;
            });

            this.WhenAnyValue(x => x.SelectNetworkAdapter).Subscribe(index =>
            {
                List<Adapter> adapterList = NetworkAdapterCollection;

                if (adapterList == null) return;

                SettingData.CurrentSetting.AdapterID = adapterList[index].ID;
                SettingData.CurrentSetting.AdapterName = adapterList[index].Name;
            });
        }
    }
}
