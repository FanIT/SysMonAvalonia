using System.Collections.Generic;
using System.Globalization;

namespace Weather.Providers
{
    public interface IProvider
    {
        public List<LocationData> GetLocation(string query, CultureInfo culture, string apiKey);
        public WeatherData GetCurrentWeatherData(string id, CultureInfo culture, string apiKey);
    }
}