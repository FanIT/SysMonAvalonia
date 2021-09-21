using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Weather
{
    public class WeatherData
    {
        public sbyte Temperature { set; get; }
        public sbyte ApparentTemperature { set; get; }
        public sbyte HighTemperature { set; get; }
        public sbyte LowTemperature { set; get; }
        public short WindSpeed { set; get; }
        public double WindDirection { set; get; }
        public byte CloudCover { set; get; }
        public byte PrecipChange { set; get; }
        public WindUnitEnum WindUnit { set; get; }
        public byte Humidity { set; get; }
        public short Pressure { set; get; }
        public byte UVIndex { set; get; }
        public byte SkyCode { set; get; }
        public string Text { set; get; }
        public DateTime Date { set; get; }
        public DateTime Sunrise { set; get; }
        public DateTime Sunset { set; get; }
        public string City { get; set; }
        public List<WeatherData> ForecastDaily { set; get; }
        public List<WeatherData> ForecastHourly { set; get; }

        private readonly string _serializePath;

        public WeatherData()
        {
#if DEBUG
            _serializePath = AppContext.BaseDirectory + @"\WeatherData.json";
#else
            _serializePath = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + @"\WeatherData.json";
#endif

            Temperature = 0;
            ApparentTemperature = 0;
            HighTemperature = 0;
            LowTemperature = 0;
            WindSpeed = 0;
            WindDirection = 0;
            CloudCover = 120;
            PrecipChange = 120;
            WindUnit = WindUnitEnum.Ms;
            Humidity = 0;
            Pressure = 0;
            UVIndex = 0;
            SkyCode = 0;
            Text = null;
            ForecastDaily = null;
            ForecastHourly = null;
        }

        public void Serialize()
        {
            dynamic jObject = new JObject();
            jObject.Temperature = Temperature;
            jObject.ApparentTemperature = ApparentTemperature;
            jObject.HightTemperature = HighTemperature;
            jObject.LowTemperature = LowTemperature;
            jObject.WindSpeed = WindSpeed;
            jObject.CloudCover = CloudCover;
            jObject.PrecipChange = PrecipChange;
            jObject.WindUnit = WindUnit;
            jObject.WindDirection = WindDirection;
            jObject.Humidity = Humidity;
            jObject.Pressure = Pressure;
            jObject.UVIndex = UVIndex;
            jObject.SkyCode = SkyCode;
            jObject.Text = Text;
            jObject.Date = Date;
            jObject.Sunrise = Sunrise;
            jObject.Sunset = Sunset;
            jObject.City = City;
            jObject.ForecastHourly = JArray.FromObject(ForecastHourly);
            jObject.ForecastDaily = JArray.FromObject(ForecastDaily);

            try
            {
                string json = JsonConvert.SerializeObject(jObject);

                File.WriteAllText(_serializePath, json);
            }
            catch
            {
                
            }
        }

        public static WeatherData Deserialize()
        {
            dynamic jObject;

            try
            {
                jObject = JsonConvert.DeserializeObject(File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"\WeatherData.json"));
            }
            catch
            {
                return null;
            }

            WeatherData weatherData = new WeatherData();

            try { weatherData.Temperature = jObject.Temperature.ToObject<int>(); } catch { /* ignored */ }
            try { weatherData.ApparentTemperature = jObject.ApparentTemperature.ToObject<int>(); } catch { /* ignored */ }
            try { weatherData.HighTemperature = jObject.HightTemperature.ToObject<int>(); } catch { /* ignored */ }
            try { weatherData.LowTemperature = jObject.LowTemperature.ToObject<int>(); } catch { /* ignored */ }
            try { weatherData.WindSpeed = jObject.WindSpeed.ToObject<int>(); } catch { /* ignored */ }
            try { weatherData.WindUnit = jObject.WindUnit.ToObject<WindUnitEnum>(); } catch { /* ignored */ }
            try { weatherData.WindDirection = jObject.WindDirection.ToObject<double>(); } catch { /* ignored */ }
            try { weatherData.CloudCover = jObject.CloudCover.ToObject<sbyte>(); } catch { /* ignored */ }
            try { weatherData.PrecipChange = jObject.PrecipChange.ToObject<sbyte>(); } catch { /* ignored */ }
            try { weatherData.Humidity = jObject.Humidity.ToObject<int>(); } catch { /* ignored */ }
            try { weatherData.Pressure = jObject.Pressure.ToObject<int>(); } catch { /* ignored */ }
            try { weatherData.UVIndex = jObject.UVIndex.ToObject<byte>(); } catch { /* ignored */ }
            try { weatherData.SkyCode = jObject.SkyCode.ToObject<int>(); } catch { /* ignored */ }
            try { weatherData.Text = jObject.Text.ToObject<string>(); } catch { /* ignored */ }
            try { weatherData.Date = jObject.Date.ToObject<DateTime>(); } catch { /* ignored */ }
            try { weatherData.Sunrise = jObject.Sunrise.ToObject<DateTime>(); } catch { /* ignored */ }
            try { weatherData.Sunset = jObject.Sunset.ToObject<DateTime>(); } catch { /* ignored */ }
            try { weatherData.City = jObject.City.ToObject<string>(); } catch { /* ignored */ }
            try { weatherData.ForecastHourly = ((JArray)jObject.ForecastHourly).ToObject<List<WeatherData>>(); } catch { /* ignored */ }
            try { weatherData.ForecastDaily = ((JArray)jObject.ForecastDaily).ToObject<List<WeatherData>>(); } catch { /* ignored */ }

            return weatherData;
        }
    }
}
