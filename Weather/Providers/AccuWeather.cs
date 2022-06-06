using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Weather.Providers
{
    public class AccuWeather : IProvider
    {
        private const string UrlLocation = "http://dataservice.accuweather.com/locations/v1/cities/search?apikey={0}&q={1}&language={2}&details=true&offset=5";
        // private const string UrlCurrent = "http://dataservice.accuweather.com/currentconditions/v1/{0}?apikey={1}&language={2}&details=true";
        // private const string UrlForecast5Day = "http://dataservice.accuweather.com/forecasts/v1/daily/5day/{0}?apikey={1}&language={2}&details=true&metric=true";
        // private const string UrlForecast12Hour = "http://dataservice.accuweather.com/forecasts/v1/hourly/12hour/{0}?apikey={1}&language={2}&details=true&metric=true";
        private const string UrlCurrent = "https://accuweatherext.srch0.com/weather/currentconditions/v1/{0}?details=true&language={1}&metric=true";
        private const string UrlForecast5Day = "https://accuweatherext.srch0.com/weather/forecasts/v1/daily/5day/{0}?details=true&language={1}&metric=true";
        private const string UrlForecast12Hour = "https://accuweatherext.srch0.com/weather/forecasts/v1/hourly/12hour/{0}?details=true&language={1}&metric=true";

        public WeatherData GetCurrentWeatherData(string id, CultureInfo culture, string apiKey = null)
        {
            string result = Helper.GetNetContent(string.Format(UrlCurrent, id, culture.Name)).Result;

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

            var weather = new WeatherData();
            weather.Date = (DateTime)jArray[0].LocalObservationDateTime;
            weather.Temperature = Round((double)jArray[0].Temperature.Metric.Value);
            weather.ApparentTemperature = Round((double)jArray[0].RealFeelTemperature.Metric.Value);
            weather.Text = jArray[0].WeatherText;
            weather.Humidity = jArray[0].RelativeHumidity;
            weather.Pressure = (short)((double)jArray[0].Pressure.Metric.Value / 1.333);
            weather.UVIndex = (byte)jArray[0].UVIndex;
            weather.SkyCode = GetWeatherIcon((int)jArray[0].WeatherIcon);
            weather.LowTemperature = Round((double)jArray[0].TemperatureSummary.Past24HourRange.Minimum.Metric.Value);
            weather.HighTemperature = Round((double)jArray[0].TemperatureSummary.Past24HourRange.Maximum.Metric.Value);
            weather.WindSpeed = (short)((double)jArray[0].Wind.Speed.Metric.Value / 3.6);
            weather.WindDirection = (double)jArray[0].Wind.Direction.Degrees;
            weather.WindUnit = WindUnitEnum.Ms;
            weather.CloudCover = (byte)jArray[0].CloudCover;

            weather.ForecastDaily = GetForecastDaily(id, culture);

            weather.ForecastHourly = GetForecastHourly(id, culture);

            weather.PrecipChange = weather.ForecastHourly[0].PrecipChange;
            
            weather.Sunrise = weather.ForecastDaily[0].Sunrise;
            weather.Sunset = weather.ForecastDaily[0].Sunset;

            weather.ForecastDaily.RemoveAt(0);

            return weather;
        }

        private List<WeatherData> GetForecastHourly(string id, CultureInfo culture)
        {
            List<WeatherData> weatherDataList = new(4);

            string result = Helper.GetNetContent(string.Format(UrlForecast12Hour, id, culture.Name)).Result;

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
                    PrecipChange = (byte)jArray[b].PrecipitationProbability,
                    WindUnit = WindUnitEnum.KmH,
                    SkyCode = GetWeatherIcon((int)jArray[b].WeatherIcon)
                };

                weatherDataList.Add(weatherData);
            }

            return weatherDataList;
        }

        private List<WeatherData> GetForecastDaily(string id, CultureInfo culture)
        {
            string result = Helper.GetNetContent(string.Format(UrlForecast5Day, id, culture.Name)).Result;

            dynamic jArray;

            try
            {
                jArray = JObject.Parse(result);
            }
            catch
            {
                return null;
            }

            List<WeatherData> weatherDataList = new(5);
                        
            for (byte b = 0; b < 5; b++)
            {
                WeatherData weatherData = new();
                weatherData.Date = (DateTime)jArray.DailyForecasts[b].Date;
                weatherData.Temperature = Round((double)jArray.DailyForecasts[b].Temperature.Maximum.Value);
                weatherData.HighTemperature = Round((double)jArray.DailyForecasts[b].Temperature.Maximum.Value);
                weatherData.LowTemperature = Round((double)jArray.DailyForecasts[b].Temperature.Minimum.Value);
                weatherData.Text = jArray.DailyForecasts[b].Day.ShortPhrase;
                weatherData.SkyCode = GetWeatherIcon((int)jArray.DailyForecasts[b].Day.Icon);
                weatherData.Sunrise = (DateTime)jArray.DailyForecasts[b].Sun.Rise;
                weatherData.Sunset = (DateTime)jArray.DailyForecasts[b].Sun.Set;

                weatherDataList.Add(weatherData);
            }

            return weatherDataList;
        }

        public List<LocationData> GetLocation(string query, CultureInfo culture, string apiKey)
        {
            string request = Helper.GetNetContent(string.Format(UrlLocation, apiKey, query, culture.Name)).Result;

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
                12 => 11,//Ливень (Showers)
                13 => 3,//Преимущественно облачно, ливень (Mostly cloudy with showers)
                14 => 3,//Местами солнечно, ливень (Partly sunny with showers)
                15 => 10,//Гроза (T-Storm)
                16 => 4,//Преимущественно облачно, гроза (Mostly cloudy with T-Storm)
                17 => 4,//Местами солнечно, гроза (Partly sunny with T-Storm)
                18 => 11,//Дождь (Rain)
                19 => 15,//Порывы ветра (Flurries)
                20 => 2,//Преимущественно облачно, ветер (Mostly cloudy with Flurries)
                21 => 2,//Местами солнечно, ветер (Partly sunny with Flurries)
                22 => 13,//Снег (Snow)
                23 => 6,//Преимущественно облачно, снег (Mostly cloudy with Snow)
                24 => 16,//Лед (Ice)
                25 => 14,//Снег с дождем (Sleet)
                26 => 16,//Ледяной дождь (Freezing rain)
                29 => 14,//Дождь и снег (Rain and Snow)
                30 => 1,//Жара (Hot)
                31 => 1,//Мороз (Cold)
                32 => 15,//Ветренно (Windly)
                33 => 17,//Ясно (Clear night)
                34 => 18,//Преимущественно ясно (Mostly clear night)
                35 => 18,//Местами облачно (Partly cloudy night)
                36 => 18,//Переменная облачность (Intermittent clouds night)
                37 => 9,//Туман (Hazy night)
                38 => 18,//Преимущественно облачно (Mostly cloudy night)
                39 => 19,//Местами облачно, дождь (Partly cloudy with Showers night)
                40 => 19,//Преимущественно облачно, дождь (Mostly cloudy with Showers night)
                41 => 20,//Местами облачно, гроза (Partly cloudy with T-Storm night)
                42 => 20,//Преимущественно облачно, гроза (Mostly cloudy with T-Storm night)
                43 => 18,//Преимущественно облачно, порывестый ветер (Mostly cloudy with Flurries night)
                44 => 22,//Преимущественно облачно, снег (Mostly cloudy with Snow night)
                _ => 0,
            };
        }

        private sbyte Round(double value)
        {
            return (sbyte)Math.Round(value, MidpointRounding.AwayFromZero);
        }
    }
}
