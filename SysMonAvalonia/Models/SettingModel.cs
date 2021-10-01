using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SysMonAvalonia.Data;
using SysMonAvalonia.Services;
using Weather;
using Weather.Providers;
using Hardware.Windows.Network;

namespace SysMonAvalonia.Models
{
    public class SettingModel
    {
        public List<Adapter> NetworkAdapters
        {
            get => NetworkInfo.GetInterface();
        }

        public bool AutostartEnable
        {
            get => AppService.FindTaskService(AppDomain.CurrentDomain.FriendlyName);
            set
            {
                if (value)
                    AppService.CreateTaskService($"{AppContext.BaseDirectory}{AppDomain.CurrentDomain.FriendlyName}.exe");
                else 
                    AppService.DeleteTaskService(AppDomain.CurrentDomain.FriendlyName);
            }
        }

        public static List<LocationData> FindCities(string query)
        {
            if (query == String.Empty) return null;

            IProvider _weatherProvider;

            List<LocationData> cityList = new();

            switch (SettingData.CurrentSetting.Provider)
            {
                case Providers.AccuWeather: _weatherProvider = new AccuWeather(); break;
                case Providers.Gismeteo: _weatherProvider = new Gismeteo(); break;
                case Providers.OpenWeatherMap: _weatherProvider = new OpenWeatherMap(); break;
                case Providers.WorldWeatherOnline: _weatherProvider = new WorldWeatherOnline(); break;
                case Providers.Wunderground: _weatherProvider = new Wunderground(); break;
                default: return cityList;
            }

            return _weatherProvider.GetLocation(query, SettingData.CurrentSetting.Culture, SettingData.CurrentSetting.WeatherApiKey);
        }

        public static void Load()
        {
            if (SettingData.CurrentSetting != null) return;

            string settingFile;
#if DEBUG
            settingFile = AppContext.BaseDirectory + @"\Settings.json";
#else
            settingFile = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + @"\Settings.json";
#endif

            string settingStr;

            try
            {
                settingStr = File.ReadAllText(settingFile);
            }
            catch
            {
                SettingData.CurrentSetting = new();
                SettingData.CurrentSetting.Culture = CultureInfo.CurrentCulture;
                SettingData.CurrentSetting.Language =
                    CultureInfo.CurrentCulture.ThreeLetterISOLanguageName.Contains("rus") ? "ru-RU" : "en-US";

                return;
            }

            SettingData.CurrentSetting = JsonConvert.DeserializeObject<SettingData>(settingStr);
            SettingData.CurrentSetting.Culture = SettingData.CurrentSetting.Language != null
                ? CultureInfo.GetCultureInfo(SettingData.CurrentSetting.Language)
                : CultureInfo.CurrentCulture;
        }

        public static void Save()
        {
            string settingFile;
#if DEBUG
            settingFile = AppContext.BaseDirectory + @"\Settings.json";
#else
            settingFile = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + @"\Settings.json";
#endif
            File.WriteAllText(settingFile, JsonConvert.SerializeObject(JObject.FromObject(SettingData.CurrentSetting)));
        }
    }
}
