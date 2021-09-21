using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json.Linq;

namespace Weather.Providers
{
    public class AccuWeather : IProvider
    {
        private const string UrlLocation = "http://dataservice.accuweather.com/locations/v1/cities/search?apikey={0}&q={1}&language={2}&details=true&offset=5";
        // private const string UrlCurrent = "http://dataservice.accuweather.com/currentconditions/v1/{0}?apikey={1}&language={2}&details=true";
        // private const string UrlForecast5Day = "http://dataservice.accuweather.com/forecasts/v1/daily/5day/{0}?apikey={1}&language={2}&details=true&metric=true";
        // private const string UrlForecast12Hour = "http://dataservice.accuweather.com/forecasts/v1/hourly/12hour/{0}?apikey={1}&language={2}&details=true&metric=true";
        private const string UrlCurrent = "https://ext-accuweather.extensionhandler.com/weather/currentconditions/v1/{0}?&language={1}&metric=true&details=true";
        private const string UrlForecast5Day = "https://ext-accuweather.extensionhandler.com/weather/forecasts/v1/daily/5day/{0}?&language={1}&metric=true&details=true";
        private const string UrlForecast12Hour = "https://ext-accuweather.extensionhandler.com/weather/forecasts/v1/hourly/12hour/{0}?&language={1}&metric=true&details=true";

        public WeatherData GetCurrentWeatherData(string id, CultureInfo culture, string apiKey = null)
        {
            string result = Helper.GetNetContent(string.Format(UrlCurrent, id, culture.Name));

            dynamic jArray;

            // DateTime unixTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            try
            {
                jArray = JArray.Parse(result);
            }
            catch
            {
                return null;
            }

            var weather = new WeatherData
            {
                Date = (DateTime)jArray[0].LocalObservationDateTime,
                Temperature = Round((double)jArray[0].Temperature.Metric.Value),
                ApparentTemperature = Round((double)jArray[0].RealFeelTemperature.Metric.Value),
                Text = jArray[0].WeatherText,
                Humidity = jArray[0].RelativeHumidity,
                Pressure = (short)((double)jArray[0].Pressure.Metric.Value / 1.333),
                UVIndex = (byte)jArray[0].UVIndex,
                SkyCode = GetWeatherIcon((int)jArray[0].WeatherIcon),
                LowTemperature = Round((double)jArray[0].TemperatureSummary.Past24HourRange.Minimum.Metric.Value),
                HighTemperature = Round((double)jArray[0].TemperatureSummary.Past24HourRange.Maximum.Metric.Value),
                WindSpeed = (short)((double)jArray[0].Wind.Speed.Metric.Value / 3.6),
                WindDirection = (double)jArray[0].Wind.Direction.Degrees,
                WindUnit = WindUnitEnum.Ms,
                CloudCover = (byte)jArray[0].CloudCover
            };

            result = Helper.GetNetContent(string.Format(UrlForecast5Day, id, culture));

            try
            {
                jArray = JObject.Parse(result);
            }
            catch
            {
                return null;
            }

            dynamic days = jArray.DailyForecasts;

            weather.Sunrise = (DateTime)days[0].Sun.Rise;
            weather.Sunset = (DateTime)days[0].Sun.Set;

            weather.ForecastDaily = GetForecastDaily(days);
            weather.ForecastHourly = GetForecastHourly(id, culture);

            return weather;
        }

        private List<WeatherData> GetForecastHourly(string id, CultureInfo culture)
        {
            List<WeatherData> weatherDataList = new(4);

            string result = Helper.GetNetContent(string.Format(UrlForecast12Hour, id, culture.Name));

            dynamic jArray;

            try
            {
                jArray = JArray.Parse(result);
            }
            catch
            {
                return null;
            }

            for (byte b = 0; b < 4; b++)
            {
                WeatherData weatherData = new()
                {
                    Date = (DateTime)jArray[b].DateTime,
                    Temperature = Round((double)jArray[b].Temperature.Value),
                    HighTemperature = Round((double)jArray[b].Temperature.Value),
                    LowTemperature = Round((double)jArray[b].Temperature.Value),
                    Text = (string)jArray[b].IconPhrase,
                    WindSpeed = (short)jArray[b].Wind.Speed.Value,
                    WindUnit = WindUnitEnum.KmH,
                    SkyCode = GetWeatherIcon((int)jArray[b].WeatherIcon)
                };

                weatherDataList.Add(weatherData);
            }

            return weatherDataList;
        }

        private List<WeatherData> GetForecastDaily(dynamic days)
        {
            List<WeatherData> weatherDataList = new(4);

            //    DateTime unixTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            for (byte b = 1; b < 5; b++)
            {
                WeatherData weatherData = new()
                {
                    Date = (DateTime)days[b].Date,
                    Temperature = Round((double)days[b].Temperature.Maximum.Value),
                    HighTemperature = Round((double)days[b].Temperature.Maximum.Value),
                    LowTemperature = Round((double)days[b].Temperature.Minimum.Value),
                    Text = days[b].Day.ShortPhrase,
                    Humidity = 0,
                    Pressure = 0,
                    WindSpeed = (short)days[b].Day.Wind.Speed.Value,
                    WindUnit = WindUnitEnum.KmH,
                    SkyCode = GetWeatherIcon((int)days[b].Day.Icon)
                };

                weatherDataList.Add(weatherData);
            }

            return weatherDataList;
        }

        public List<LocationData> GetLocation(string query, CultureInfo culture, string apiKey)
        {
            string request = Helper.GetNetContent(string.Format(UrlLocation, apiKey, query, culture.Name));

            List<LocationData> locationList = new();

            JArray jArray;

            try
            {
                jArray = JArray.Parse(request);
            }
            catch
            {
                return locationList;
            }

            foreach (dynamic o in jArray)
            {
                var location = new LocationData
                {
                    City = o.LocalizedName,
                    Country = o.Country.LocalizedName,
                    ID = o.Key
                };

                locationList.Add(location);
            }

            return locationList;
        }

        private byte GetWeatherIcon(int skyCode)
        {
            return skyCode switch
            {
                1 => 1,//Солнечно (Sunny)
                2 => 2,//Преимуществонно солнечно (Mostly sunny)
                3 => 2,//Местами солнечно (Partly sunny)
                4 => 2,//Переменная облачность (Intermittent clouds)
                5 => 8,//Дымка (Hazy sunshine)
                6 => 2,//Преимущественно облачно (Mostly cloudy)
                7 => 8,//Облачно (Cloudy)
                8 => 9,//Пасмурно (Overcast)
                11 => 8,//Туман (Fog)
                12 => 12,//Ливень (Showers)
                13 => 3,//Преимущественно облачно, ливень (Mostly cloudy with showers)
                14 => 3,//Местами солнечно, ливень (Partly sunny with showers)
                15 => 10,//Гроза (T-Storm)
                16 => 7,//Преимущественно облачно, гроза (Mostly cloudy with T-Storm)
                17 => 7,//Местами солнечно, гроза (Partly sunny with T-Storm)
                18 => 11,//Дождь (Rain)
                19 => 18,//Порывы ветра (Flurries)
                20 => 17,//Преимущественно облачно, ветер (Mostly cloudy with Flurries)
                21 => 2,//Местами солнечно, ветер (Partly sunny with Flurries)
                22 => 15,//Снег (Snow)
                23 => 6,//Преимущественно облачно, снег (Mostly cloudy with Snow)
                24 => 16,//Лед (Ice)
                25 => 14,//Снег с дождем (Sleet)
                26 => 16,//Ледяной дождь (Freezing rain)
                29 => 14,//Дождь и снег (Rain and Snow)
                30 => 26,//Жара (Hot)
                31 => 27,//Мороз (Cold)
                32 => 18,//Ветренно (Windly)
                33 => 19,//Ясно (Clear night)
                34 => 20,//Преимущественно ясно (Mostly clear night)
                35 => 20,//Местами облачно (Partly cloudy night)
                36 => 20,//Переменная облачность (Intermittent clouds night)
                37 => 8,//Туман (Hazy night)
                38 => 20,//Преимущественно облачно (Mostly cloudy night)
                39 => 21,//Местами облачно, дождь (Partly cloudy with Showers night)
                40 => 21,//Преимущественно облачно, дождь (Mostly cloudy with Showers night)
                41 => 25,//Местами облачно, гроза (Partly cloudy with T-Storm night)
                42 => 25,//Преимущественно облачно, гроза (Mostly cloudy with T-Storm night)
                43 => 17,//Преимущественно облачно, порывестый ветер (Mostly cloudy with Flurries night)
                44 => 24,//Преимущественно облачно, снег (Mostly cloudy with Snow night)
                _ => 1,
            };
        }

        private sbyte Round(double value)
        {
            return (sbyte)Math.Round(value, MidpointRounding.AwayFromZero);
        }
    }
}
