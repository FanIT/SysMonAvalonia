using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace Weather.Providers
{
    public class Wunderground : IProvider
    {
        private const string ADL_LOCATION_URL = "https://services.gismeteo.ru/inform-service/inf_chrome/cities/?startsWith={0}&search_all=1&lang={1}";
        private const string ADL_WEATHER_URL = "https://weather.com/{0}/weather/today/l/{1}";

        public WeatherData GetCurrentWeatherData(string id, CultureInfo culture, string apiKey)
        {
            string lng = culture.Name;

            RegionInfo regionInfo = new(culture.Name);

            string unit = regionInfo.IsMetric ? "m" : "e";

            JObject jsonObject;

            try
            {
                string json = Helper.GetNetContent(string.Format(ADL_WEATHER_URL, lng, id)).Result;
                json = ParserFixerJson(json);
                
                jsonObject = JObject.Parse(json);
            }
            catch { return null; }

            try
            {
                WeatherData currentWeather = new();
                currentWeather.City = jsonObject["dal"]?["getSunV3LocationPointUrlConfig"]?[$"geocode:{id};language:{lng}"]?["data"]?["location"]?["city"]?.ToObject<string>();
                currentWeather.Temperature = (sbyte)jsonObject["dal"]?["getSunV3CurrentObservationsUrlConfig"]?[$"geocode:{id};language:{lng};units:{unit}"]?["data"]?["temperature"];
                currentWeather.ApparentTemperature = (sbyte)jsonObject["dal"]?["getSunV3CurrentObservationsUrlConfig"]?[$"geocode:{id};language:{lng};units:{unit}"]?["data"]?["temperatureFeelsLike"];
                currentWeather.Pressure = Convert.ToInt16(((int)jsonObject["dal"]?["getSunV3CurrentObservationsUrlConfig"]?[$"geocode:{id};language:{lng};units:{unit}"]?["data"]?["pressureAltimeter"]) / 1.333);
                currentWeather.Humidity = (byte)jsonObject["dal"]?["getSunV3CurrentObservationsUrlConfig"]?[$"geocode:{id};language:{lng};units:{unit}"]?["data"]?["relativeHumidity"];
                currentWeather.WindSpeed = Convert.ToInt16(((int)jsonObject["dal"]?["getSunV3CurrentObservationsUrlConfig"]?[$"geocode:{id};language:{lng};units:{unit}"]?["data"]?["windSpeed"]) / 3.6);
                currentWeather.WindDirection = (int)jsonObject["dal"]?["getSunV3CurrentObservationsUrlConfig"]?[$"geocode:{id};language:{lng};units:{unit}"]?["data"]?["windDirection"];
                currentWeather.WindUnit = WindUnitEnum.Ms;
                currentWeather.UVIndex = (byte)jsonObject["dal"]?["getSunV3CurrentObservationsUrlConfig"]?[$"geocode:{id};language:{lng};units:{unit}"]?["data"]?["uvIndex"];
                currentWeather.Sunrise = (DateTime)jsonObject["dal"]?["getSunV3CurrentObservationsUrlConfig"]?[$"geocode:{id};language:{lng};units:{unit}"]?["data"]?["sunriseTimeLocal"];
                currentWeather.Sunset = (DateTime)jsonObject["dal"]?["getSunV3CurrentObservationsUrlConfig"]?[$"geocode:{id};language:{lng};units:{unit}"]?["data"]?["sunsetTimeLocal"];
                currentWeather.SkyCode = WeatherIconCode((byte)jsonObject["dal"]?["getSunV3CurrentObservationsUrlConfig"]?[$"geocode:{id};language:{lng};units:{unit}"]?["data"]?["iconCode"]);
                currentWeather.Text = (string)jsonObject["dal"]?["getSunV3CurrentObservationsUrlConfig"]?[$"geocode:{id};language:{lng};units:{unit}"]?["data"]?["wxPhraseLong"];
                currentWeather.Date = (DateTime)jsonObject["dal"]?["getSunV3CurrentObservationsUrlConfig"]?[$"geocode:{id};language:{lng};units:{unit}"]?["data"]?["validTimeLocal"];
                currentWeather.ForecastHourly = ForecastHourly((JObject)jsonObject["dal"]?["getSunV3HourlyForecastUrlConfig"], id, lng, unit);
                currentWeather.ForecastDaily = ForecastDaily((JObject)jsonObject["dal"]?["getSunV3DailyForecastWithHeadersUrlConfig"]?[$"duration:7day;geocode:{id};language:{lng};units:{unit}"]);
                currentWeather.HighTemperature = currentWeather.ForecastDaily[0].HighTemperature;
                currentWeather.LowTemperature = currentWeather.ForecastDaily[0].LowTemperature;
                currentWeather.CloudCover = currentWeather.ForecastHourly[0].CloudCover;
                currentWeather.PrecipChange = currentWeather.ForecastHourly[0].PrecipChange;

                currentWeather.ForecastDaily.RemoveAt(0);

                return currentWeather;
            }
            catch { return null; }
        }

        private List<WeatherData> ForecastHourly(JObject hourlyJson, string latlon, string lang, string unit)
        {
            List<WeatherData> weatherDataList = new();

            for (byte b = 1; b < 2; b++)
            {
                JArray dateArray = hourlyJson[$"duration:{b}day;geocode:{latlon};language:{lang};units:{unit}"]["data"]?["validTimeLocal"]?.ToObject<JArray>();
                JArray iconArray = hourlyJson[$"duration:{b}day;geocode:{latlon};language:{lang};units:{unit}"]["data"]?["iconCode"]?.ToObject<JArray>();
                JArray tempArray = hourlyJson[$"duration:{b}day;geocode:{latlon};language:{lang};units:{unit}"]["data"]?["temperature"]?.ToObject<JArray>();
                JArray phraseArray = hourlyJson[$"duration:{b}day;geocode:{latlon};language:{lang};units:{unit}"]["data"]?["wxPhraseLong"]?.ToObject<JArray>();
                JArray cloudCoverArray = hourlyJson[$"duration:{b}day;geocode:{latlon};language:{lang};units:{unit}"]["data"]?["cloudCover"]?.ToObject<JArray>();
                JArray precipChanceArray = hourlyJson[$"duration:{b}day;geocode:{latlon};language:{lang};units:{unit}"]["data"]?["precipChance"]?.ToObject<JArray>();

                if (dateArray == null || tempArray == null || cloudCoverArray == null || precipChanceArray == null || iconArray == null || phraseArray == null) continue;

                for (byte c = 0; c < dateArray.Count; c++)
                {
                    WeatherData hourlyData = new();
                    hourlyData.Date = dateArray[c].ToObject<DateTime>();
                    hourlyData.Temperature = tempArray[c].ToObject<sbyte>();
                    hourlyData.CloudCover = cloudCoverArray[c].ToObject<byte>();
                    hourlyData.PrecipChange = precipChanceArray[c].ToObject<byte>();
                    hourlyData.SkyCode = WeatherIconCode(iconArray[c].ToObject<byte>());
                    hourlyData.Text = phraseArray[c].ToObject<string>();

                    weatherDataList.Add(hourlyData);

                    if (weatherDataList.Count == 4) break;
                }

                if (weatherDataList.Count == 4) break;
            }

            return weatherDataList;
        }

        private List<WeatherData> ForecastDaily(JObject dailyJson)
        {
            List<WeatherData> weatherDataList = new();

            JArray dateArray = dailyJson["data"]["validTimeLocal"] as JArray;
            JArray iconArray = dailyJson["data"]["daypart"]?[0]?["iconCode"] as JArray;
            JArray phraseArray = dailyJson["data"]["daypart"]?[0]?["wxPhraseLong"] as JArray;
            JArray tempMinArray = dailyJson["data"]["temperatureMin"] as JArray;
            JArray tempMaxArray = dailyJson["data"]["temperatureMax"] as JArray;

            if (dateArray == null || iconArray == null || phraseArray == null || tempMinArray == null ||
                tempMaxArray == null) return weatherDataList;

            byte[] icons = new byte[(iconArray.Count / 2)];
            string[] phases = new string[(phraseArray.Count / 2)];

            for (byte b = 0, c = 0; b < iconArray.Count; b += 2, c++)
            {
                try { icons[c] = iconArray[b].ToObject<byte>(); } catch { icons[c] = 41; }
                phases[c] = phraseArray[b].ToObject<string>();
            }

            for (byte b = 0; b < dateArray.Count; b++)
            {
                WeatherData dailyData = new();
                dailyData.Date = dateArray[b].ToObject<DateTime>();
                try { dailyData.HighTemperature = tempMaxArray[b].ToObject<sbyte>(); } catch { dailyData.HighTemperature = -100; }
                try { dailyData.LowTemperature = tempMinArray[b].ToObject<sbyte>(); } catch { dailyData.LowTemperature = -100; }
                dailyData.SkyCode = WeatherIconCode(icons[b]);
                dailyData.Text = phases[b];

                weatherDataList.Add(dailyData);

                if (weatherDataList.Count == 5) break;
            }

            return weatherDataList;
        }

        public List<LocationData> GetLocation(string query, CultureInfo culture, string apiKey)
        {
            string result = Helper.GetNetContent(string.Format(ADL_LOCATION_URL, query, culture.TwoLetterISOLanguageName)).Result;

            XElement xdoc;

            try
            {
                xdoc = XDocument.Parse(result).Element("document");
            }
            catch { return null; }

            List<LocationData> locationList = new List<LocationData>();

            if (xdoc != null)
            {
                foreach (XElement item in xdoc.Elements("item"))
                {
                    LocationData location = new LocationData
                    {
                        City = item.Attribute("n")?.Value,
                        Country = item.Attribute("country_name")?.Value,
                        ID = $"{item.Attribute("lat")?.Value},{item.Attribute("lng")?.Value}"
                    };

                    locationList.Add(location);
                }
            }

            return locationList;
        }

        private string ParserFixerJson(string json)
        {
            Regex regex = new Regex(@"window\.__data=JSON\.parse(.*);</script><script>window\.__i18n", RegexOptions.IgnoreCase);

            Match match = regex.Match(json);

            json = match.Groups[1].Value;

            json = json.Substring(2, json.Length - 4);

            Regex rx = new Regex(@"\\[uU]([0-9A-F]{4})");
            json = rx.Replace(json, match1 => ((char)int.Parse(match1.Value.Substring(2), NumberStyles.HexNumber)).ToString());
            json = json.Replace("getSunV3TidalPredictionsUrlConfig", "#");

            StringBuilder stringBuilder = new();

            foreach (char j in json.Where(j => j != '\\').TakeWhile(j => j != '#'))
            {
                stringBuilder.Append(j);
            }

            stringBuilder.Replace("<?xml version=\"1.0\" encoding=\"UTF-8\"?>", "n");

            stringBuilder.Remove(stringBuilder.Length - 2, 2);
            stringBuilder.Append("}}");

            return stringBuilder.ToString();
        }

        private byte WeatherIconCode(byte code)
        {
            switch (code)
            {
                case 0: return 15; //Tornado (Торнадо)
                case 1: return 15; //Tropical Storm (Тропический ураган)
                case 2: return 15; //Hurricane (Ураган)
                case 3: return 15; //Strong Storms (Сильный ураган)
                case 4: return 10; //Thunderstorms (Гроза)
                case 5: return 14; //Rain/Snow (Осадки)
                case 6: return 14; //Rain/Sleet (Дождь, мокрый снег)
                case 7: return 14; //Wintry Mix (Снег с дождем)
                case 8: return 16; //Freezing Drizzle (Замержающая морось)
                case 9: return 16; //Drizzle (Морось)
                case 10: return 16; //Freezing Rain (Ледяной дождь)
                case 11: return 11; //Showers (Ливень)
                case 12: return 11; //Rain (Дождь)
                case 13: return 15; //Flurries (Метель)
                case 14: return 13; //Snow Showers (Снегопад)
                case 15: return 16; //Blowing / Drifting Snow (Позёмок)
                case 16: return 13; //Snow (Снег)
                case 17: return 16; //Hail (Град)
                case 18: return 14; //Sleet (Мокрый снег)
                case 19: return 15; //Blowing Dust/Sandstorm (Песчаная буря)
                case 20: return 15; //Foggy (Туман)
                case 21: return 15; //Haze (Мгла)
                case 22: return 15; //Smoke (Дымка)
                case 23: return 15; //Breezy (Бриз)
                case 24: return 15; //Windy (Ветренно)
                case 25: return 16; //Frigid/Ice Crystals (Град)
                case 26: return 8; //Cloudy (Облачно)
                case 27: return 18; //Mostly Cloudy (Преимущественно облачно - ночь)
                case 28: return 2; //Mostly Cloudy (Преимущественно облачно - день)
                case 29: return 18; //Partly Cloudy (Временами облачно - ночь)
                case 30: return 2; //Partly Cloudy (Временами облачно - день)
                case 31: return 17; //Clear (Ясно - ночь)
                case 32: return 1; //Sunny (Солнечно - день)
                case 33: return 18; //Fair/Mostly Clear (Премищественно ясно - ночь)
                case 34: return 2; //Fair/Mostly Sunny (Преимущественно солнечно - день)
                case 35: return 14; //Mixed Rain and Hail (Град с дождем)
                case 36: return 1; //Hot (Жара)
                case 37: return 4; //Isolated Thunderstorms (Местами гроза - день)
                case 38: return 4; //Scattered Thunderstorms (Временами гроза - день)
                case 39: return 3; //Scattered Showers (Временами ливень - день)
                case 40: return 11; //Heavy Rain (Сильный дождь)
                case 41: return 6; //Scattered Snow Showers (Временами сильный снег - день)
                case 42: return 13; //Heavy Snow (Сильный снег)
                case 43: return 13; //Blizzard (Снежная буря)
                case 44: return 0; //Not Available(N/ A) (Без иконки)
                case 45: return 19; //Scattered Showers (Временами ливень - ночь)
                case 46: return 22; //Scattered Snow Showers (Временами сильный снег - ночь)
                case 47: return 20; //Scattered Thunderstorms (Временами гроза - ночь)
                default: return 0;
            }
        }
    }
}
