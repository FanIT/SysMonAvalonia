using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Weather.Providers;
using SysMonAvalonia.Data;
using SysMonAvalonia.Localization;
using Weather;
using WeatherData = SysMonAvalonia.Data.WeatherData;

namespace SysMonAvalonia.Models
{
    public class WeatherModel
    {
        public ObservableCollection<ForecastData> ForecastHourlyCollection { get; set; }
        public ObservableCollection<ForecastData> ForecastDailyCollection { get; set; }

        private Weather.WeatherData? GetWeatherData()
        {
            if (SettingData.CurrentSetting.Provider == Providers.None || SettingData.CurrentSetting.WeatherID == string.Empty)
                return null;

            Weather.WeatherData currentWeatherData;

            DateTime nextUpdateDateTime = SettingData.CurrentSetting.WeatherLastUpdate.AddSeconds(SettingData.CurrentSetting.WeatherInterval);

            if (DateTime.Now >= nextUpdateDateTime)
            {
                IProvider provider = GetProvider();

                currentWeatherData = provider.GetCurrentWeatherData(SettingData.CurrentSetting.WeatherID,
                                                                    SettingData.CurrentSetting.Culture,
                                                                    SettingData.CurrentSetting.WeatherApiKey);

                currentWeatherData?.Serialize();

                if (currentWeatherData != null) SettingData.CurrentSetting.WeatherLastUpdate = DateTime.Now;
            }
            else
                currentWeatherData = Weather.WeatherData.Deserialize();

            return currentWeatherData;
        }

        private void CreateForecastControl(int dailyCount, int hourlyCount)
        { 
            if (ForecastHourlyCollection == null)
            {
                ForecastHourlyCollection = new();

                for (byte b = 0; b < hourlyCount; b++)
                {
                    ForecastHourlyCollection.Add(new());
                }
            }
 
            if (ForecastDailyCollection == null)
            {
                ForecastDailyCollection = new();

                for (byte b = 0; b < dailyCount; b++)
                {
                    ForecastDailyCollection.Add(new());
                }
            }
        }
 
         private void ForecastUpdate(List<Weather.WeatherData> dailyList, List<Weather. WeatherData> hourlyList)
         {
             string iconsDir = SettingData.CurrentSetting.Style == SettingData.Theme.Dark ? "White" : "Black";
 
             for (byte b = 0; b < hourlyList.Count; b++)
             {
                 ForecastHourlyCollection[b].DateTime = hourlyList[b].Date.ToShortTimeString();
                 ForecastHourlyCollection[b].Icon = $"/Assets/WeatherIcons/{iconsDir}/30/weather_{hourlyList[b].SkyCode}.png";
                 ForecastHourlyCollection[b].TempMax = hourlyList[b].Temperature;
                 ForecastHourlyCollection[b].Phase = hourlyList[b].Text;
             }
 
             for (byte b = 0; b < dailyList.Count; b++)
             {
                 ForecastDailyCollection[b].DateTime = dailyList[b].Date.ToString("ddd,dd", SettingData.CurrentSetting.Culture);
                 ForecastDailyCollection[b].Icon = $"/Assets/WeatherIcons/{iconsDir}/30/weather_{dailyList[b].SkyCode}.png";
                 ForecastDailyCollection[b].TempMax = dailyList[b].HighTemperature;
                 ForecastDailyCollection[b].TempMin = dailyList[b].LowTemperature;
                 ForecastDailyCollection[b].Phase = dailyList[b].Text;
             }
         }

         public WeatherData WeatherRefresh()
         {
            Weather.WeatherData currentWeatherData = GetWeatherData();

            if (currentWeatherData == null) return null;

            WeatherData weatherData = new();

            string iconsDir = SettingData.CurrentSetting.Style == SettingData.Theme.Dark ? "White" : "Black";
            
            weatherData.CurrentIcon = $"/Assets/WeatherIcons/{iconsDir}/65/weather_{currentWeatherData.SkyCode}.png";
            weatherData.CurrentPhase = currentWeatherData.Text;
            weatherData.CurrentTemp = currentWeatherData.Temperature;
            weatherData.MinTemp = currentWeatherData.LowTemperature;
            weatherData.MaxTemp = currentWeatherData.HighTemperature;
            weatherData.ApparTemp = currentWeatherData.ApparentTemperature;
            weatherData.CloudCover = currentWeatherData.CloudCover;
            weatherData.WindSpeed = currentWeatherData.WindSpeed;
            weatherData.WindDegree = currentWeatherData.WindDirection;
            weatherData.WindAbbr = GetWindAbbr(currentWeatherData.WindDirection);
            weatherData.WindUnit = GetUnitAbbr(currentWeatherData.WindUnit);
            weatherData.Humidity = currentWeatherData.Humidity; 
            weatherData.Pressure = currentWeatherData.Pressure;
            weatherData.Precipitation = currentWeatherData.PrecipChange;
            weatherData.UVIndex = currentWeatherData.UVIndex;
            weatherData.Sunrise = currentWeatherData.Sunrise.ToShortTimeString();
            weatherData.Sunset = currentWeatherData.Sunset.ToShortTimeString();
            weatherData.City = currentWeatherData.City;
            weatherData.LastUpdate = currentWeatherData.Date.ToShortTimeString();
            weatherData.IsCloudyCover = weatherData.CloudCover != 120;
            weatherData.IsPreciptation = weatherData.Precipitation != 120;

            CreateForecastControl(currentWeatherData.ForecastDaily.Count, currentWeatherData.ForecastHourly.Count);
            ForecastUpdate(currentWeatherData.ForecastDaily, currentWeatherData.ForecastHourly);

            return weatherData;
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
