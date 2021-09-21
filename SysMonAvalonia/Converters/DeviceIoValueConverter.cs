using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace SysMonAvalonia.Converters
{
    public class DeviceIoValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is long)
                return ((long)value / 1073741824.0).ToString("0.#");

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
