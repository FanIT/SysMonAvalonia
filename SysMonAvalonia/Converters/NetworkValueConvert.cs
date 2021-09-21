using System;
using System.Globalization;
using Avalonia.Data.Converters;
using SysMonAvalonia.Data;

namespace SysMonAvalonia.Converters
{
    public class NetworkValueConvert : IValueConverter
    {
        public static NetworkValueConvert Instange = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is string and "gb")
                return (long)value / 1073741824.0;

            if (SettingData.CurrentSetting.NetworkUnit == SettingData.NetUnit.Megabits)
                return (long)value / 125000.0;
            else
                return (long)value / 1048576.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
