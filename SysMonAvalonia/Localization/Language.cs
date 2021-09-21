using System;
using System.Globalization;
using System.IO;
using Avalonia;
using Avalonia.Platform;
using Newtonsoft.Json;

namespace SysMonAvalonia.Localization
{
    public class Language
    {
        public string Close { get; set; }
        public string Settings { get; set; }
        public string Mb { get; set; }
        public string Gb { get; set; }
        public string Tb { get; set; }
        public string Mbit { get; set; }
        public string Mbyte { get; set; }
        public string Health { get; set; }
        public string Write { get; set; }
        public string ReadIO { get; set; }
        public string WriteIO { get; set; }
        public string CpuHeader { get; set; }
        public string GpuHeader { get; set; }
        public string RamHeader { get; set; }
        public string Adapters { get; set; }
        public string ApparTempDesc { get; set; }
        public string WindSpeedDesc { get; set; }
        public string CloudCoverDesc { get; set; }
        public string ChanceOfRain { get; set; }
        public string HumidityDesc { get; set; }
        public string Pressure { get; set; }
        public string PressureDesc { get; set; }
        public string UVIndexDesc { get; set; }
        public string UVIndexLow { get; set; }
        public string UVIndexMedium { get; set; }
        public string UVIndexHigh { get; set; }
        public string UVIndexVeryHigh { get; set; }
        public string UVIndexExtrem { get; set; }
        public string WindUnitKmH { get; set; }
        public string WindUnitMH { get; set; }
        public string WindUnitMS { get; set; }
        public string WindDirDesc0 { get; set; }
        public string WindDirDesc22 { get; set; }
        public string WindDirDesc45 { get; set; }
        public string WindDirDesc67 { get; set; }
        public string WindDirDesc90 { get; set; }
        public string WindDirDesc112 { get; set; }
        public string WindDirDesc135 { get; set; }
        public string WindDirDesc157 { get; set; }
        public string WindDirDesc180 { get; set; }
        public string WindDirDesc202 { get; set; }
        public string WindDirDesc225 { get; set; }
        public string WindDirDesc247 { get; set; }
        public string WindDirDesc270 { get; set; }
        public string WindDirDesc292 { get; set; }
        public string WindDirDesc315 { get; set; }
        public string WindDirDesc337 { get; set; }
        public string WaterTempDesc { get; set; }
        public string SunsetDesc { get; set; }
        public string SunriseDesc { get; set; }
        public string CityDesc { get; set; }
        public string LastUpdateDesc { get; set; }
        public string Hourly { get; set; }
        public string Daily { get; set; }
        public string Update { get; set; }
        public string Providers { get; set; }
        public string Time { get; set; }
        public string GenTabSetting { get; set; }
        public string Network { get; set; }
        public string LangSetting { get; set; }
        public string Russia { get; set; }
        public string English { get; set; }
        public string WidgSetting { get; set; }
        public string DiscsSetting { get; set; }
        public string ComboSetting { get; set; }
        public string WeatherSetting { get; set; }
        public string ApiKeySettings { get; set; }
        public string EnterCityWM { get; set; }
        public string AutostartHeaderSetting { get; set; }
        public string AutostartTogle { get; set; }
        public string LocationSetting { get; set; }
        public string Themes { get; set; }
        public string Dark { get; set; }
        public string Light { get; set; }
        public string NetUnit { get; set; }
        public string MbitSettings { get; set; }
        public string MbyteSettings { get; set; }
        public string Cancel { get; set; }
        public static Language Locale { get; set; }

        public static void Load(CultureInfo culture)
        {
            IAssetLoader asset = AvaloniaLocator.Current.GetService<IAssetLoader>();

            string lngFile = culture.ThreeLetterISOLanguageName.ToLower() == "rus" ? "ru.json" : "en.json";

            using (StreamReader reader =
                new StreamReader(asset.Open(new Uri($"avares://SysMonAvalonia/Assets/Locale/{lngFile}"))))
            {
                try
                {
                    Locale = JsonConvert.DeserializeObject<Language>(reader.ReadToEnd());
                }
                catch
                {
                    Locale = new();
                }
            }
        }
    }
}