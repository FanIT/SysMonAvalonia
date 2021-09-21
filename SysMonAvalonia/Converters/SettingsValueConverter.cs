using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Data.Converters;
using Weather.Providers;
using Hardware.Windows.Network;
using SysMonAvalonia.Data;

namespace SysMonAvalonia.Converters
{
    public class SettingsValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter as string == "lang" && value is string lang)
            {
                if (lang == "en-US") return 0;
                if (lang == "ru-RU") return 1;
            }

            if (parameter as string == "theme")
                return (int)value;

            if (parameter as string == "wprovider")
                return (int)SettingData.CurrentSetting.Provider;

            if (parameter as string == "apikey")
            {
                int val = (int)value;

                if (val == 2 || val == 3) return true;
                else return false;
            }

            if (parameter as string == "netadapter" && value is string adapterID)
            {
                List<Adapter> adapterList = NetworkInfo.GetInterface();

                for (int i = 0; i < adapterList.Count; i++)
                {
                    if (adapterList[i].ID.Contains(adapterID)) return i;
                }
            }

            if (parameter as string == "netunit" && value is SettingData.NetUnit)
                return (int)value;

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter as string == "lang" && value is int lang)
            {
                if (lang == 0) return "en-US";
                if (lang == 1) return "ru-RU";
            }

            if (parameter as string == "theme" && value is int theme)
                return (SettingData.Theme)theme;

            if (parameter as string == "wprovider" && value is int provider)
                return (Providers)provider;

            if (parameter as string == "netadapter" && value is int adapter)
                return NetworkInfo.GetInterface()[adapter].ID;

            if (parameter as string == "netunit" && value is int unit)
                return (SettingData.NetUnit)unit;

            return null;
        }
    }
}
