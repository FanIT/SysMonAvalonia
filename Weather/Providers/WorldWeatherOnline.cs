using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json.Linq;

namespace Weather.Providers
{
    public class WorldWeatherOnline : IProvider
    {
        private const string LocationUrl = "https://api.weatherapi.com/v1/search.json?key={0}&q={1}";
        private const string ForecastUrl = "https://api.weatherapi.com/v1/forecast.json?key={0}&q={1}&days=4&lang={2}&aqi=no&alerts=no";

        public WeatherData GetCurrentWeatherData(string id, CultureInfo culture, string apiKey)
        {
            string json = Helper.GetNetContent(string.Format(ForecastUrl, apiKey, id, culture.TwoLetterISOLanguageName));

            dynamic jObject;

            try { jObject = JObject.Parse(json); } catch { return null; }

            WeatherData weatherData = new WeatherData();
            weatherData.Temperature = Round(jObject.current.temp_c.ToObject<double>());
            weatherData.Text = jObject.current.condition.text.ToObject<string>();
            weatherData.SkyCode = GetWeatherIcon(jObject.current.condition.code.ToObject<int>(), jObject.current.is_day.ToObject<byte>());
            weatherData.ApparentTemperature = Round(jObject.current.feelslike_c.ToObject<double>());
            weatherData.CloudCover = jObject.current.cloud.ToObject<byte>();
            weatherData.Humidity = jObject.current.humidity.ToObject<byte>();
            weatherData.Pressure = (short)(jObject.current.pressure_mb.ToObject<double>() / 1.333);
            weatherData.UVIndex = (byte)Round(jObject.current.uv.ToObject<double>());
            weatherData.WindSpeed = jObject.current.wind_mph.ToObject<short>();
            weatherData.WindUnit = WindUnitEnum.Mh;
            weatherData.WindDirection = jObject.current.wind_degree.ToObject<int>();
            weatherData.Date = jObject.current.last_updated.ToObject<DateTime>();
            weatherData.ForecastDaily = GetForecastDaily(jObject.forecast);
            weatherData.ForecastHourly = GetForecastHourly(jObject.forecast);
            weatherData.HighTemperature = weatherData.ForecastDaily[0].HighTemperature;
            weatherData.LowTemperature = weatherData.ForecastDaily[0].LowTemperature;
            weatherData.Sunrise = weatherData.ForecastDaily[0].Sunrise;
            weatherData.Sunset = weatherData.ForecastDaily[0].Sunset;

            return weatherData;
        }

        private List<WeatherData> GetForecastDaily(dynamic forecast)
        {
            List<WeatherData> weatherDataList = new List<WeatherData>();

            foreach (dynamic day in forecast.forecastday)
            {
                WeatherData weatherData = new WeatherData();
                weatherData.HighTemperature = Round(day.day.maxtemp_c.ToObject<double>());
                weatherData.LowTemperature = Round(day.day.mintemp_c.ToObject<double>());
                weatherData.Text = day.day.condition.text.ToObject<string>();
                weatherData.SkyCode = GetWeatherIcon(day.day.condition.code.ToObject<int>(), 1);
                weatherData.Sunrise = day.astro.sunrise.ToObject<DateTime>();
                weatherData.Sunset = day.astro.sunset.ToObject<DateTime>();
                weatherData.Date = day.date.ToObject<DateTime>();

                weatherDataList.Add(weatherData);
            }

            return weatherDataList;
        }

        private List<WeatherData> GetForecastHourly(dynamic forecast)
        {
            List<WeatherData> weatherDataList = new List<WeatherData>();

            foreach (dynamic day in forecast.forecastday)
            {
                foreach (dynamic hour in day.hour)
                {
                    DateTime time = hour.time.ToObject<DateTime>();

                    if (time.Date == DateTime.Now.Date && time.Hour <= DateTime.Now.Hour) continue;

                    WeatherData weatherData = new WeatherData();
                    weatherData.Temperature = Round(hour.temp_c.ToObject<double>());
                    weatherData.Text = hour.condition.text.ToObject<string>();
                    weatherData.SkyCode = GetWeatherIcon(hour.condition.code.ToObject<int>(), hour.is_day.ToObject<byte>());
                    weatherData.Date = hour.time.ToObject<DateTime>();

                    weatherDataList.Add(weatherData);

                    if (weatherDataList.Count == 4) break;
                }

                if (weatherDataList.Count == 4) break;
            }

            return weatherDataList;
        }

        public List<LocationData> GetLocation(string query, CultureInfo culture, string apiKey)
        {
            string json = Helper.GetNetContent(string.Format(LocationUrl, apiKey, query));

            dynamic jArray;

            try { jArray = JArray.Parse(json); } catch { return null; }

            List<LocationData> locationDataList = new List<LocationData>();

            foreach (dynamic city in jArray)
            {
                string temp = city.name.ToObject<string>();

                LocationData location = new LocationData();
                location.ID = $"{city.lat.ToObject<string>()},{city.lon.ToObject<string>()}";
                location.City = temp.Substring(0, temp.IndexOf(','));
                location.Country = city.country.ToObject<string>();

                locationDataList.Add(location);

                if (locationDataList.Count == 4) break;
            }

            return locationDataList;
        }

        private byte GetWeatherIcon(int code, byte isDay)
        {
            bool day = isDay == 1;

            switch (code)
            {
                case 1000: return (byte)(day ? 1 : 19); //Ясно
                case 1003: return (byte)(day ? 2 : 20); //Переменная облачность
                case 1006: return 8; //Облачно
                case 1009: return 9; //Пасмурно
                case 1030: return 18; //Дымка
                case 1063: return (byte)(day ? 3 : 21); //Местами дождь
                case 1066: return (byte)(day ? 6 : 24); //Местами снег
                case 1069: return (byte)(day ? 6 : 24); //Местами мокрый снег
                case 1072: return (byte)(day ? 3 : 21); //Местами морозь
                case 1087: return (byte)(day ? 7 : 25); //Местами грозы
                case 1114: return 16; //Поземок
                case 1117: return 17; //Метель
                case 1135: return 17; //Туман
                case 1147: return 17; //Холодный туман
                case 1150: return (byte)(day ? 3 : 21); //Местами слабая морось
                case 1153: return 11; //Слабая морось
                case 1168: return 14; //Замерзающая морось
                case 1171: return 14; //Сильная, замержающая морось
                case 1180: return (byte)(day ? 3 : 21); //Местами небольшой дождь
                case 1183: return 11; //Небольшой дождь
                case 1186: return (byte)(day ? 3 : 21); //Временами умеренный дождь
                case 1189: return 11; //Умеренный дождь
                case 1192: return (byte)(day ? 3 : 21); //Временами сильный дождь
                case 1195: return 12; //Сильный дождь
                case 1198: return 16; //Слабый, ледяной дождь
                case 1201: return 16; //Умеренный или сильный, ледяной дождь
                case 1204: return 14; //Небольшие осадки
                case 1207: return 14; //Умеренные или силные осадки
                case 1210: return (byte)(day ? 6 : 24); //Местами небольшой снег
                case 1213: return 15; //Небольшой снег
                case 1216: return (byte)(day ? 6 : 24); //Местами умеренный снег
                case 1219: return 15; //Умеренный снег
                case 1222: return (byte)(day ? 6 : 24); //Местами сильный снег
                case 1225: return 15; //Сильный снег
                case 1237: return 16; //Ледяной дождь
                case 1240: return 11; //Небольшой ливень
                case 1243: return 12; //Умеренный или сильный ливень
                case 1246: return 12; //Сильный ливень
                case 1249: return 14; //Небольшой ливень со снегом
                case 1252: return 14; //Умеренный или сильный ливень со снегом
                case 1255: return 15; //Небольшой снег
                case 1258: return 15; //Умеренный или сильный снег
                case 1261: return 16; //Небольшой, ледяной дождь
                case 1264: return 16; //Умеренный или сильный, ледяной дождь
                case 1273: return (byte)(day ? 4 : 22); //В отдельных районах местами небольшой дождь с грозой
                case 1276: return (byte)(day ? 4 : 22); //В отдельных районах умеренный или сильный дождь с грозой
                case 1279: return (byte)(day ? 6 : 24); //В отдельных районах местами небольшой снег с грозой
                case 1282: return (byte)(day ? 6 : 24); //В отдельных районах умеренный или сильный снег с грозой
                default: return 1;
            }
        }

        private sbyte Round(double value)
        {
            return (sbyte)Math.Round(value, MidpointRounding.AwayFromZero);
        }
    }
}
