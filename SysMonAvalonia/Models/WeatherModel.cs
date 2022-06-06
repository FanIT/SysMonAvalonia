using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DynamicData;
using ReactiveUI;
using Weather.Providers;
using SysMonAvalonia.Data;
using SysMonAvalonia.Localization;
using Weather;
using WeatherData = SysMonAvalonia.Data.WeatherData;
using System.Reactive.Linq;

namespace SysMonAvalonia.Models
{
    public class WeatherModel
    {
        private readonly SourceList<ForecastData> _hourlyForecasts;
        private readonly SourceList<ForecastData> _dailyForecasts;

        public ReadOnlyObservableCollection<ForecastData> HourlyForecasts;
        public ReadOnlyObservableCollection<ForecastData> DailyForecasts;

        public WeatherData CurrentWeather { get; set; }

        public WeatherModel()
        {
            CurrentWeather = new();

            _hourlyForecasts = new();
            _hourlyForecasts.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(out HourlyForecasts).Subscribe();

            _dailyForecasts = new();
            _dailyForecasts.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(out DailyForecasts).Subscribe();
        }

        private Weather.WeatherData? GetWeatherData()
        {
            if (SettingData.CurrentSetting.Provider == Providers.None || SettingData.CurrentSetting.WeatherID == string.Empty)
                return null;

            Weather.WeatherData? currentWeatherData = null;

            DateTime nextUpdateDateTime = SettingData.CurrentSetting.WeatherLastUpdate.AddSeconds(SettingData.CurrentSetting.WeatherInterval);

            if (DateTime.Now >= nextUpdateDateTime)
            {
                IProvider provider = GetProvider();

                for (byte b = 0; b < 4; b++)
                {
                    currentWeatherData = provider.GetCurrentWeatherData(SettingData.CurrentSetting.WeatherID,
                        SettingData.CurrentSetting.Culture,
                        SettingData.CurrentSetting.WeatherApiKey);

                    if (currentWeatherData != null) break;
                }

                currentWeatherData?.Serialize();

                if (currentWeatherData != null) SettingData.CurrentSetting.WeatherLastUpdate = DateTime.Now;
            }
            else
                currentWeatherData = Weather.WeatherData.Deserialize();

            return currentWeatherData;
        }

        private void CreateForecastControl(int dailyCount, int hourlyCount)
        {
            if (HourlyForecasts.Count != hourlyCount)
            {
                _hourlyForecasts.Clear();

                for (byte b = 0; b < hourlyCount; b++)
                {
                    _hourlyForecasts.Add(new());
                }
            }

            if (DailyForecasts.Count != dailyCount)
            {
                _dailyForecasts.Clear();

                for (byte b = 0; b < dailyCount; b++)
                {
                    _dailyForecasts.Add(new());
                }
            }
        }
 
         private void ForecastUpdate(List<Weather.WeatherData> dailyList, List<Weather.WeatherData> hourlyList)
         {
            string iconsDir = SettingData.CurrentSetting.Style == SettingData.Theme.Dark ? "White" : "Black";

            byte count = 0;

            foreach (ForecastData hourly in _hourlyForecasts.Items)
            {
                hourly.DateTime = hourlyList[count].Date.ToShortTimeString();
                hourly.Icon = $"/Assets/WeatherIcons/{iconsDir}/30/weather_{hourlyList[count].SkyCode}.png";
                hourly.TempMax = hourlyList[count].Temperature;
                hourly.Phase = hourlyList[count].Text;

                count++;
            }

            count = 0;

           foreach (ForecastData daily in _dailyForecasts.Items)
           {
               daily.DateTime = dailyList[count].Date.ToString("ddd,dd");
               daily.Icon = $"/Assets/WeatherIcons/{iconsDir}/30/weather_{dailyList[count].SkyCode}.png";
               daily.TempMax = dailyList[count].HighTemperature;
               daily.TempMin = dailyList[count].LowTemperature;
               daily.Phase = dailyList[count].Text;

               count++;
           }
         }

         public void WeatherRefresh()
         {
            Weather.WeatherData? currentWeatherData = GetWeatherData();

            if (currentWeatherData == null) return;

            string iconsDir = SettingData.CurrentSetting.Style == SettingData.Theme.Dark ? "White" : "Black";

            CurrentWeather.CurrentIcon = $"/Assets/WeatherIcons/{iconsDir}/65/weather_{currentWeatherData.SkyCode}.png";
            CurrentWeather.CurrentPhase = currentWeatherData.Text;
            CurrentWeather.CurrentTemp = currentWeatherData.Temperature;
            CurrentWeather.MinTemp = currentWeatherData.LowTemperature;
            CurrentWeather.MaxTemp = currentWeatherData.HighTemperature;
            CurrentWeather.ApparTemp = currentWeatherData.ApparentTemperature;
            CurrentWeather.CloudCover = currentWeatherData.CloudCover;
            CurrentWeather.WindSpeed = currentWeatherData.WindSpeed;
            CurrentWeather.WindDegree = currentWeatherData.WindDirection;
            CurrentWeather.WindAbbr = GetWindAbbr(currentWeatherData.WindDirection);
            CurrentWeather.WindUnit = GetUnitAbbr(currentWeatherData.WindUnit);
            CurrentWeather.Humidity = currentWeatherData.Humidity;
            CurrentWeather.Pressure = currentWeatherData.Pressure;
            CurrentWeather.Precipitation = currentWeatherData.PrecipChange;
            CurrentWeather.UVIndex = currentWeatherData.UVIndex;
            CurrentWeather.Sunrise = currentWeatherData.Sunrise.ToShortTimeString();
            CurrentWeather.Sunset = currentWeatherData.Sunset.ToShortTimeString();
            CurrentWeather.City = currentWeatherData.City == null ? SettingData.CurrentSetting.WeatherCity : currentWeatherData.City;
            CurrentWeather.LastUpdate = currentWeatherData.Date.ToShortTimeString();
            CurrentWeather.IsCloudyCover = currentWeatherData.CloudCover != 120;
            CurrentWeather.IsPreciptation = CurrentWeather.Precipitation != 120;

            CreateForecastControl(currentWeatherData.ForecastDaily.Count, currentWeatherData.ForecastHourly.Count);
            ForecastUpdate(currentWeatherData.ForecastDaily, currentWeatherData.ForecastHourly);
         }

        private IProvider GetProvider()
        {
            IProvider provider;

            switch (SettingData.CurrentSetting.Provider)
            {
                case Providers.AccuWeather: provider = new AccuWeather(); break;
                case Providers.Gismeteo: provider = new Gismeteo(); break;
                case Providers.WorldWeatherOnline: provider = new WorldWeatherOnline(); break;
                case Providers.OpenWeatherMap: provider = new OpenWeatherMap(); break;
                default: provider = new Wunderground(); break;
            }

            return provider;
        }

        private string GetUnitAbbr(WindUnitEnum windUnit)
        {
            string unit;

            switch (windUnit)
            {
                case WindUnitEnum.Ms: unit = Language.Locale.WindUnitMS; break;
                case WindUnitEnum.Mh: unit = Language.Locale.WindUnitMH; break;
                case WindUnitEnum.KmH: unit = Language.Locale.WindUnitKmH; break;
                default: unit = ""; break;
            }

            return unit;
        }

        private string GetWindAbbr(double degree)
        {
            if (degree == 0) return Language.Locale.WindDirDesc0;
            else if (degree > 0 && degree < 45) return Language.Locale.WindDirDesc22;
            else if (degree == 45) return Language.Locale.WindDirDesc45;
            else if (degree > 45 && degree < 90) return Language.Locale.WindDirDesc67;
            else if (degree == 90) return Language.Locale.WindDirDesc90;
            else if (degree > 90 && degree < 135) return Language.Locale.WindDirDesc112;
            else if (degree == 135) return Language.Locale.WindDirDesc135;
            else if (degree > 135 && degree < 180) return Language.Locale.WindDirDesc157;
            else if (degree == 180) return Language.Locale.WindDirDesc180;
            else if (degree > 180 && degree < 225) return Language.Locale.WindDirDesc202;
            else if (degree == 225) return Language.Locale.WindDirDesc225;
            else if (degree > 225 && degree < 270) return Language.Locale.WindDirDesc247;
            else if (degree == 270) return Language.Locale.WindDirDesc270;
            else if (degree > 270 && degree < 315) return Language.Locale.WindDirDesc292;
            else if (degree == 315) return Language.Locale.WindDirDesc315;
            else return Language.Locale.WindDirDesc337;
        }
    }
}
