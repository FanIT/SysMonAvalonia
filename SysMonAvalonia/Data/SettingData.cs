using System;
using System.Globalization;
using Newtonsoft.Json;
using Weather.Providers;

namespace SysMonAvalonia.Data
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SettingData
    {
        public enum NetUnit : int
        {
            Megabits = 0,
            Megabytes = 1
        }
        public enum Theme : int
        {
            Dark = 0,
            Light = 1
        }
        [JsonObject(MemberSerialization.OptIn)]
        public class WidgetProperties
        {
            [JsonProperty("IsStartup")]
            public bool IsStartup { get; set; }
            [JsonProperty("X")]
            public int X { get; set; }
            [JsonProperty("Y")]
            public int Y { get; set; }

            public WidgetProperties()
            {
                IsStartup = true;
                X = 0;
                Y = 0;
            }
        }

        private readonly string SettingFile;
        [JsonProperty("Language")] 
        public string Language { get; set; }
        public CultureInfo Culture { get; set; }
        [JsonProperty("Theme")]
        public Theme Style { get; set; }
        [JsonProperty("WeatherProvider")]
        public Providers Provider { get; set; }
        [JsonProperty("WeatherWidget")]
        public WidgetProperties WeatherWidget { get; set; }
        [JsonProperty("DeviceIoWidget")]
        public WidgetProperties DeviceIoWidget { get; set; }
        [JsonProperty("ComboWidget")]
        public WidgetProperties ComboWidget { get; set; }
        [JsonProperty("WeatherInterval")]
        public int WeatherInterval { get; set; }
        [JsonProperty("WeatherLastUpdate")]
        public DateTime WeatherLastUpdate { get; set; }
        [JsonProperty("WeatherApiKey")]
        public string WeatherApiKey { get; set; }
        [JsonProperty("WeatherID")]
        public string WeatherID { get; set; }
        [JsonProperty("WeatherCity")]
        public string WeatherCity { get; set; }
        [JsonProperty("NetworkAdapter")]
        public string AdapterName { get; set; }
        public string AdapterID { get; set; }
        [JsonProperty("NetworkUnit")]
        public NetUnit NetworkUnit { get; set; }
        public static SettingData CurrentSetting { get; set; }
    }
}
