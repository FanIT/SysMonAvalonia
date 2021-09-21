using System;
using System.Globalization;
using Avalonia.Data.Converters;
using SysMonAvalonia.Localization;

namespace SysMonAvalonia.Converters
{
    class DataValueConverter : IValueConverter
    {
        public static DataValueConverter Instance = new DataValueConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string param = (string)parameter;

            if (param.Contains("temp"))
            {
                if (value is sbyte)
                {
                    sbyte temp = (sbyte)value;

                    if (temp == -100) return "--";

                    if (temp >= 0) return $"+{temp}°";
                    else return $"{temp}°";
                }
            }

            if (param.Contains("percent")) return $"{value}%";
            if (param.Contains("pressure")) return $"{value} {Language.Locale.Pressure}";
            if (param.Contains("uvindex")) return $"{GetNameUVIndex((byte)value)} ({value})";

                return null;
        }

        private string GetNameUVIndex(byte index)
        {
            if (index < 3) return Language.Locale.UVIndexLow;
            else if (index > 2 && index < 6) return Language.Locale.UVIndexMedium;
            else if (index > 5 && index < 8) return Language.Locale.UVIndexHigh;
            else if (index > 7 && index < 11) return Language.Locale.UVIndexVeryHigh;
            else return Language.Locale.UVIndexExtrem;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
