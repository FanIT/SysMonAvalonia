using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Globalization;

namespace Weather.Providers
{
    public class Gismeteo : IProvider
    {
        private const string UrlLocation = "https://services.gismeteo.ru/inform-service/inf_chrome/cities/?startsWith={0}&search_all=1&lang={1}";
        private const string UrlWeatherForecast = "https://services.gismeteo.ru/inform-service/inf_chrome/forecast/?city={0}&lang={1}";

        public WeatherData GetCurrentWeatherData(string id, CultureInfo culture, string apiKey)
        {
            string result = Helper.GetNetContent(string.Format(UrlWeatherForecast, id, culture.TwoLetterISOLanguageName));

            XDocument xdoc;

            try
            {
                xdoc = XDocument.Parse(result);
            }
            catch
            {                
                return null;
            }

            DateTime unixTime = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            string city = xdoc.Element("weather")?.Element("location")?.Attribute("name")?.Value;
            var fact = xdoc.Element("weather")?.Element("location")?.Element("fact");
            var days = xdoc.Element("weather")?.Element("location")?.Elements("day").ToArray();

            if (fact == null || days == null) return null;

            var currentDay = days[1];

            WeatherData weatherData = new();
            weatherData.Date = DateTime.Now;
            weatherData.Temperature = Convert.ToSByte(fact.Element("values")?.Attribute("t")?.Value);
            weatherData.ApparentTemperature = (sbyte)Math.Round(
                Convert.ToDouble(fact.Element("values")?.Attribute("tcflt")?.Value.Replace('.', ',')),
                MidpointRounding.AwayFromZero);
            weatherData.HighTemperature = Convert.ToSByte(currentDay?.Attribute("tmax")?.Value);
            weatherData.LowTemperature = Convert.ToSByte(currentDay?.Attribute("tmin")?.Value);
            weatherData.Text = fact.Element("values")?.Attribute("descr")?.Value;
            weatherData.Humidity = Convert.ToByte(fact.Element("values")?.Attribute("hum")?.Value);
            weatherData.Pressure = Convert.ToInt16(fact.Element("values")?.Attribute("p")?.Value);
            weatherData.WindSpeed = Convert.ToInt16(fact.Element("values")?.Attribute("ws")?.Value);
            weatherData.WindDirection = WindDirectionConvert(Convert.ToByte(fact.Element("values")?.Attribute("wd")?.Value));
            weatherData.WindUnit = WindUnitEnum.Ms;
            weatherData.Sunrise = unixTime.AddSeconds(Convert.ToDouble(fact.Attribute("sunrise")?.Value));
            weatherData.Sunset = unixTime.AddSeconds(Convert.ToDouble(fact.Attribute("sunset")?.Value));
            weatherData.ForecastDaily = GetForecastDaily(days);
            weatherData.ForecastHourly = GetForecastHourly(days);
            weatherData.City = city;
            weatherData.SkyCode = GetWeatherIcon(byte.Parse(fact.Element("values")?.Attribute("cl")?.Value),
                                                 byte.Parse(fact.Element("values")?.Attribute("pt")?.Value),
                                                 byte.Parse(fact.Element("values")?.Attribute("pr")?.Value),
                                                 byte.Parse(fact.Element("values")?.Attribute("ts")?.Value),
                                                 weatherData.Sunrise, weatherData.Sunset);

            return weatherData;
        }

        private static List<WeatherData> GetForecastHourly(XElement[] days)
        {
            List<WeatherData> weatherDataList = new();

            DateTime unixTime = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            for (byte b = 1; weatherDataList.Count != 4; b++)
            {
                var hours = days[b].Elements("forecast");

                foreach (XElement h in hours)
                {
                    if (b == 1 && DateTime.Parse(h.Attribute("valid")?.Value).Hour <= DateTime.Now.Hour)
                        continue;

                    WeatherData weatherData = new()
                    {
                        Date = DateTime.Parse(h.Attribute("valid")?.Value),
                        Temperature = Convert.ToSByte(h.Element("values")?.Attribute("t")?.Value),
                        Text = h.Element("values")?.Attribute("descr")?.Value,
                        Humidity = Convert.ToByte(h.Element("values")?.Attribute("hum")?.Value),
                        Pressure = Convert.ToInt16(h.Element("values")?.Attribute("p")?.Value),
                        WindSpeed = Convert.ToInt16(h.Element("values")?.Attribute("ws")?.Value),
                        WindUnit = WindUnitEnum.Mh,
                        SkyCode = GetWeatherIcon(byte.Parse(h.Element("values")?.Attribute("cl")?.Value),
                                                 byte.Parse(h.Element("values")?.Attribute("pt")?.Value),
                                                 byte.Parse(h.Element("values")?.Attribute("pr")?.Value),
                                                 byte.Parse(h.Element("values")?.Attribute("ts")?.Value),
                                                 unixTime.AddSeconds(double.Parse(days[b]?.Attribute("sunrise")?.Value)),
                                                 unixTime.AddSeconds(double.Parse(days[b]?.Attribute("sunset")?.Value))) //GetWeatherIcon(h.Element("values")?.Attribute("icon")?.Value)
                    };

                    weatherDataList.Add(weatherData);

                    if (weatherDataList.Count == 4) break;
                }
            }

            return weatherDataList;
        }

        private List<WeatherData> GetForecastDaily(XElement[] days)
        {
            List<WeatherData> weatherList = new(4);

            for (byte b = 2; b < 6; b++)
            {
                var day = days[b];

                WeatherData weatherData = new()
                {
                    Date = DateTime.Parse(day.Attribute("date")?.Value),
                    Temperature = Convert.ToSByte(day.Attribute("tmax")?.Value),
                    HighTemperature = Convert.ToSByte(day.Attribute("tmax")?.Value),
                    LowTemperature = Convert.ToSByte(day.Attribute("tmin")?.Value),
                    Text = day.Attribute("descr")?.Value,
                    Humidity = Convert.ToByte(day.Attribute("hummax")?.Value),
                    Pressure = Convert.ToInt16(day.Attribute("pmax")?.Value),
                    WindSpeed = Convert.ToInt16(day.Attribute("wsmax")?.Value),
                    WindUnit = WindUnitEnum.Mh,
                    SkyCode = GetWeatherIcon(byte.Parse(day.Attribute("cl")?.Value), byte.Parse(day.Attribute("pt")?.Value),
                                             byte.Parse(day.Attribute("pr")?.Value), byte.Parse(day.Attribute("ts")?.Value)) //GetWeatherIcon(day.Attribute("icon")?.Value)
                };

                weatherList.Add(weatherData);
            }

            return weatherList;
        }

        public List<LocationData> GetLocation(string query, CultureInfo culture, string apiKey)
        {
            List<LocationData> locationList = new();

            string result = Helper.GetNetContent(string.Format(UrlLocation, query, culture.TwoLetterISOLanguageName));

            XElement xdoc;

            try
            {
                xdoc = XDocument.Parse(result).Element("document");
            }
            catch
            {
                return locationList;
            }

            if (xdoc != null)
            {
                foreach (XElement item in xdoc.Elements("item"))
                {
                    LocationData location = new()
                    {
                        City = item.Attribute("n")?.Value,
                        Country = item.Attribute("country_name")?.Value,
                        ID = item.Attribute("id")?.Value
                    };

                    locationList.Add(location);
                }
            }

            return locationList;
        }

        private static byte GetWeatherIcon(byte cloud, byte precipType, byte preciptation, byte tstorm, DateTime? sunrise = null, DateTime? sunset = null)
        {
            int code = cloud * 100;

            code += (precipType * 10) + preciptation;

            if (tstorm == 1) code *= 10;

            bool isDay = true;

            if (sunrise != null) isDay = DateTime.Now > sunrise && DateTime.Now < sunset;

            return code switch
            {
                0 => (byte)(isDay ? 1 : 19),//Ясно
                100 => (byte)(isDay ? 2 : 20),//Малооблачно
                1000 => (byte)(isDay ? 7 : 25),//Малооблачно, гроза
                111 => (byte)(isDay ? 3 : 21),//Малооблачно, небольшой дождь
                1110 => (byte)(isDay ? 4 : 22),//Малооблачно, небольшой дождь, гроза
                112 => (byte)(isDay ? 3 : 21),//Малооблачно, дождь
                1120 => (byte)(isDay ? 4 : 22),//Малооблачно, дождь, гроза
                113 => (byte)(isDay ? 3 : 21),//Малооблачно, сильный дождь
                1130 => (byte)(isDay ? 4 : 22),//Малооблачно, сильный дождь, гроза
                121 => (byte)(isDay ? 6 : 24),//Малооблачно, небольшой снег
                122 => (byte)(isDay ? 6 : 24),//Малооблачно, снег
                123 => (byte)(isDay ? 6 : 24),//Малооблачно, сильный снег
                131 => (byte)(isDay ? 5 : 23),//Малооблачно, небольшие осадки
                1310 => (byte)(isDay ? 4 : 22),//Малооблачно, небольшие осадки, гроза
                132 => (byte)(isDay ? 5 : 23),//Малооблачно, осадки
                1320 => (byte)(isDay ? 4 : 22),//Малооблачно, осадки, гроза
                133 => (byte)(isDay ? 5 : 23),//Малооблачно, сильные осадки
                1330 => (byte)(isDay ? 4 : 22),//Малооблачно, сильные осадки, гроза
                200 => 8,//Облачно
                2000 => 10,//Облачно, гроза
                211 => 11,//Облачно, небольшой дождь
                2110 => 13,//Облачно, небольшой дождь, гроза
                212 => 12,//Облачно, дождь
                2120 => 13,//Облачно, дождь, гроза
                213 => 12,//Облачно, сильный дождь
                2130 => 13,//Облачно, сильный дождь, гроза
                221 => 15,//Облачно, небольшой снег
                222 => 15,//Облачно, снег
                223 => 15,//Облачно, сильный снег
                231 => 14,//Облачно, небольшие осадки
                2310 => 13,//Облачно, небольшие осадки, гроза
                232 => 14,//Облачно, осадки
                2320 => 13,//Облачно, осадки, гроза
                233 => 14,//Облачно, сильные осадки
                2330 => 13,//Облачно, сильные осадки, гроза
                300 => 9,//Пасмурно
                3000 => 10,//Пасмурно, гроза
                311 => 11,//Пасмурно, небольшой дождь
                3110 => 13,//Пасмурно, небольшой дождь, гроза
                312 => 11,//Пасмурно, дождь
                3120 => 13,//Пасмурно, дождь, гроза
                313 => 12,//Пасмурно, сильный дождь
                3130 => 13,//Пасмурно, сильный дождь, гроза
                321 => 15,//Пасмурно, небольшой снег
                322 => 15,//Пасмурно, снег
                323 => 15,//Пасмурно, сильный снег
                331 => 14,//Пасмурно, небольшие осадки
                3310 => 13,//Пасмурно, небольшие осадки, гроза
                332 => 14,//Пасмурно, осадки
                3320 => 13,//Пасмурно, осадки, гроза
                333 => 14,//Пасмурно, сильные осадки
                3330 => 13,//Пасмурно, сильные осадки, гроза
                10100 => (byte)(isDay ? 2 : 20),//Переменная облачность
                101000 => (byte)(isDay ? 7 : 25),//Переменная облачность, гроза
                10111 => (byte)(isDay ? 3 : 21),//Переменная облачность, небольшой дождь
                101110 => (byte)(isDay ? 4 : 22),//Переменная облачность, небольшой дождь, гроза
                10112 => (byte)(isDay ? 3 : 21),//Переменная облачность, дождь
                101120 => (byte)(isDay ? 4 : 22),//Переменная облачность, дождь, гроза
                10113 => (byte)(isDay ? 3 : 21),//Переменная облачность, сильный дождь
                101130 => (byte)(isDay ? 4 : 22),//Переменная облачность, сильный дождь, гроза
                10121 => (byte)(isDay ? 6 : 24),//Переменная облачность, небольшой снег
                10122 => (byte)(isDay ? 6 : 24),//Переменная облачность, снег
                10123 => (byte)(isDay ? 6 : 24),//Переменная облачность, сильный снег
                10131 => (byte)(isDay ? 5 : 23),//Переменная облачность, небольшие осадки
                101310 => (byte)(isDay ? 4 : 22),//Переменная облачность, небольшие осадки, гроза
                10132 => (byte)(isDay ? 5 : 23),//Переменная облачность, осадки
                101320 => (byte)(isDay ? 4 : 22),//Переменная облачность, осадки, гроза
                10133 => (byte)(isDay ? 5 : 23),//Переменная облачность, сильные осадки
                101330 => (byte)(isDay ? 4 : 22),//Переменная облачность, сильные осадки, гроза
                _ => 18,
            };
        }

        private double WindDirectionConvert(byte direction)
        {
            return direction switch
            {
                1 => 0,
                2 => 45,
                3 => 90,
                4 => 135,
                5 => 180,
                6 => 225,
                7 => 270,
                8 => 315,
                _ => 0,
            };
        }
    }
}
