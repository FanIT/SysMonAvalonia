using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json.Linq;

namespace Weather.Providers
{
    public class OpenWeatherMap : IProvider
    {
        private const string LocationUrl = "http://api.openweathermap.org/geo/1.0/direct?q={0}&limit=3&appid={1}";
        private const string WeatherUrl = "https://api.openweathermap.org/data/2.5/onecall?lat={0}&lon={1}&appid={2}&lang={3}&units={4}";

        public List<LocationData> GetLocation(string query, CultureInfo culture, string apiKey)
        {
            string response = Helper.GetNetContent(string.Format(LocationUrl, query, apiKey));

            JArray cityList;

            try
            {
                cityList = JArray.Parse(response);
            }
            catch
            {
                return null;
            }

            List<LocationData> locations = new List<LocationData>();

            try
            {
                for (int i = 0; i < cityList.Count; i++)
                {
                    LocationData location = new LocationData();
                    location.City = cityList[i]["local_names"][culture.TwoLetterISOLanguageName].ToObject<string>();
                    location.Country = cityList[i]["country"].ToObject<string>();
                    location.ID = cityList[i]["lat"].ToObject<string>() + ";" + cityList[i]["lon"].ToObject<string>();

                    locations.Add(location);
                }
            }
            catch { }

            return locations;
        }

        public WeatherData GetCurrentWeatherData(string id, CultureInfo culture, string apiKey)
        {
            string lat = id.Substring(0, id.IndexOf(";", StringComparison.Ordinal) - 1);
            string lon = id.Substring(id.IndexOf(";", StringComparison.Ordinal) + 1, id.Length - (id.IndexOf(";", StringComparison.Ordinal) + 1));

            string response = Helper.GetNetContent(string.Format(WeatherUrl, lat, lon, apiKey, culture.TwoLetterISOLanguageName, "metric"));

            dynamic current;
            dynamic hourly;
            dynamic daily;

            try
            {
                dynamic json = JObject.Parse(response);
                current = json.current;
                hourly = (JArray)json.hourly;
                daily = (JArray)json.daily;
            }
            catch
            {
                return null;
            }

            WeatherData weatherData = new WeatherData
            {
                Temperature = Round((double)current.temp),
                ApparentTemperature = Round((double)current.feels_like),
                Humidity = (byte)current.humidity,
                Pressure = (short)((double)current.pressure / 1.333),
                UVIndex = (byte)current.uvi,
                WindSpeed = Round((double)current.wind_speed),
                WindDirection = (double)current.wind_deg,
                WindUnit = WindUnitEnum.Ms,
                CloudCover = (byte)current.clouds,
                Sunrise = UnixTimeToDateTime((double)current.sunrise),
                Sunset = UnixTimeToDateTime((double)current.sunset),
                SkyCode = GetWeatherIcon((int)current.weather[0].id, (string)current.weather[0].icon),
                Text = (string)current.weather[0].description,
                Date = UnixTimeToDateTime((double)current.dt),
                ForecastHourly = ForecastHourly(hourly),
                ForecastDaily = ForecastDaily(daily)
            };
            weatherData.HighTemperature = weatherData.ForecastDaily[0].HighTemperature;
            weatherData.LowTemperature = weatherData.ForecastDaily[0].LowTemperature;

            weatherData.ForecastDaily.RemoveAt(0);

            return weatherData;
        }

        private List<WeatherData> ForecastHourly(dynamic hourly)
        {
            List<WeatherData> weatherList = new List<WeatherData>();

            foreach (dynamic hour in hourly)
            {
                DateTime dateTime = UnixTimeToDateTime((double)hour.dt);

                if (dateTime.Hour <= DateTime.Now.Hour) continue;

                WeatherData weatherData = new WeatherData
                {
                    Temperature = Round((double)hour.temp),
                    ApparentTemperature = Round((double)hour.feels_like),
                    Humidity = (byte)hour.humidity,
                    Pressure = (short)((double)hour.pressure / 1.333),
                    WindSpeed = Round((double)hour.wind_speed),
                    WindDirection = (double)hour.wind_deg,
                    WindUnit = WindUnitEnum.Ms,
                    SkyCode = GetWeatherIcon((int)hour.weather[0].id, (string)hour.weather[0].icon),
                    Text = (string)hour.weather[0].description,
                    Date = dateTime
                };

                weatherList.Add(weatherData);

                if (weatherList.Count == 4) break;
            }

            return weatherList;
        }

        private List<WeatherData> ForecastDaily(dynamic daily)
        {
            List<WeatherData> weatherList = new List<WeatherData>();

            for (byte b = 0; b < 5; b++)
            {
                WeatherData weatherData = new WeatherData
                {
                    Temperature = Round((double)daily[b].temp.day),
                    ApparentTemperature = Round((double)daily[b].feels_like.day),
                    HighTemperature = Round((double)daily[b].temp.max),
                    LowTemperature = Round((double)daily[b].temp.min),
                    Humidity = (byte)daily[b].humidity,
                    Pressure = (short)((double)daily[b].pressure / 1.333),
                    WindSpeed = Round((double)daily[b].wind_speed),
                    WindDirection = (double)daily[b].wind_deg,
                    WindUnit = WindUnitEnum.Ms,
                    Sunrise = UnixTimeToDateTime((double)daily[b].sunrise),
                    Sunset = UnixTimeToDateTime((double)daily[b].sunset),
                    SkyCode = GetWeatherIcon((int)daily[b].weather[0].id, (string)daily[b].weather[0].icon),
                    Text = (string)daily[b].weather[0].description,
                    Date = UnixTimeToDateTime((double)daily[b].dt)
                };

                weatherList.Add(weatherData);
            }

            return weatherList;
        }

        private byte GetWeatherIcon(int skycode, string icon)
        {
            bool isDay = icon[2] == 'd';

            switch (skycode)
            {
                case 200: return 13; //гроза, небольшой дождь
                case 201: return 13; //Гроза с дождем
                case 202: return 13; //Гроза, сильный дождь
                case 210: return 10; //Слабая гроза
                case 211: return 10; //Гроза
                case 212: return 10; //Сильная гроза
                case 221: return (byte)(isDay ? 7 : 25); //Местами гроза
                case 230: return 13; //Гроза, слабая морось
                case 231: return 13; //Гроза, морось
                case 232: return 13; //Гроза, сильная морось
                case 300: return 16; //Слабая морось
                case 301: return 16; //Морось
                case 302: return 16; //Сильная морось
                case 310: return 11; //Слабо моросящий дождь
                case 311: return 11; //Моросящий дождь
                case 312: return 11; //Сильная изморось
                case 313: return 12; //Сильный дождь и изморось
                case 314: return 12; //Ливень и изморось
                case 321: return 12; //Сильный, моросящий дождь
                case 500: return 11; //Слабый дождь
                case 501: return 11; //Мелкий дождь
                case 502: return 12; //Сильный дождь
                case 503: return 12; //Очень сильный дождь
                case 504: return 12; //Экстримальный дождь
                case 511: return 16; //Ледяной дождь
                case 520: return 11; //Умеренный дождь
                case 521: return 12; //Ливень
                case 522: return 12; //Сильный ливень
                case 531: return (byte)(isDay ? 3 : 21); //Местами ливень
                case 600: return 15; //Слабый снег
                case 601: return 15; //Снег
                case 602: return 15; //Сильный снег
                case 611: return 14; //Снег с дождем
                case 612: return 14; //Слабый снег с дождем
                case 613: return 14; //Сильный снег с дождем
                case 615: return 14; //Слабые осадки
                case 616: return 14; //Осадки
                case 620: return 15; //Снегопад
                case 622: return 15; //Сильный снегопад
                case 701: return 18; //Дымка
                case 711: return 18; //Смог
                case 721: return 18; //Мгла
                case 731: return 18; //Песок
                case 741: return 18; //Туман
                case 751: return 18; //Песок
                case 761: return 18; //Пыль
                case 762: return 18; //Вулканический пепел
                case 771: return 18; //Шквалистый ветер
                case 781: return 18; //Торнадо
                case 800: return (byte)(isDay ? 1 : 19); //Ясно
                case 801: return (byte)(isDay ? 2 : 20); //Малооблачно
                case 802: return (byte)(isDay ? 2 : 20); //Вренами облачно
                case 803: return 8; //Преимущественно облачно
                case 804: return 9; //Пасмурно
                default: return 1;
            }
        }

        private sbyte Round(double value)
        {
            return (sbyte)Math.Round(value, MidpointRounding.AwayFromZero);
        }

        private DateTime UnixTimeToDateTime(double seconds)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(seconds).ToLocalTime();
        }
    }
}
