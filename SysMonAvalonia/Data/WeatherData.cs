using ReactiveUI;

namespace SysMonAvalonia.Data
{
    public class WeatherData : ReactiveObject
    {
        private string _currentIcon;
        private string _currentPhase;
        private sbyte _currentTemp;
        private sbyte _minTemp;
        private sbyte _maxTemp;
        private sbyte _apparTemp;
        private string _windAbbr;
        private short _windSpeed;
        private double _windDegree;
        private string _windUnit;
        private byte _cloudCover;
        private bool _isCloudyCover;
        private byte _precipitation;
        private bool _isPreciptation;
        private byte _humidity;
        private short _pressure;
        private byte _uvIndex;
        private string _sunset;
        private string _sunrise;
        private string _city;
        private string _lastUpdate;

        public string CurrentIcon
        {
            get => _currentIcon;
            set => this.RaiseAndSetIfChanged(ref _currentIcon, value);
        }
        public string CurrentPhase
        {
            get => _currentPhase;
            set => this.RaiseAndSetIfChanged(ref _currentPhase, value);
        }
        public sbyte CurrentTemp
        {
            get => _currentTemp;
            set => this.RaiseAndSetIfChanged(ref _currentTemp, value);
        }
        public sbyte MinTemp
        {
            get => _minTemp;
            set => this.RaiseAndSetIfChanged(ref _minTemp, value);
        }
        public sbyte MaxTemp
        {
            get => _maxTemp;
            set => this.RaiseAndSetIfChanged(ref _maxTemp, value);
        }
        public sbyte ApparTemp
        {
            get => _apparTemp;
            set => this.RaiseAndSetIfChanged(ref _apparTemp, value);
        }
        public string WindAbbr
        {
            get => _windAbbr;
            set => this.RaiseAndSetIfChanged(ref _windAbbr, value);
        }
        public short WindSpeed
        {
            get => _windSpeed;
            set => this.RaiseAndSetIfChanged(ref _windSpeed, value);
        }
        public double WindDegree
        {
            get => _windDegree;
            set => this.RaiseAndSetIfChanged(ref _windDegree, value);
        }
        public string WindUnit
        {
            get => _windUnit;
            set => this.RaiseAndSetIfChanged(ref _windUnit, value);
        }
        public byte CloudCover
        {
            get => _cloudCover;
            set => this.RaiseAndSetIfChanged(ref _cloudCover, value);
        }
        public bool IsCloudyCover
        {
            get => _isCloudyCover;
            set => this.RaiseAndSetIfChanged(ref _isCloudyCover, value);
        }
        public byte Precipitation
        {
            get => _precipitation;
            set => this.RaiseAndSetIfChanged(ref _precipitation, value);
        }
        public bool IsPreciptation
        {
            get => _isPreciptation;
            set => this.RaiseAndSetIfChanged(ref _isPreciptation, value);
        }
        public byte Humidity
        {
            get => _humidity;
            set => this.RaiseAndSetIfChanged(ref _humidity, value);
        }
        public short Pressure
        {
            get => _pressure;
            set => this.RaiseAndSetIfChanged(ref _pressure, value);
        }
        public byte UVIndex
        {
            get => _uvIndex;
            set => this.RaiseAndSetIfChanged(ref _uvIndex, value);
        }
        public string Sunset
        {
            get => _sunset;
            set => this.RaiseAndSetIfChanged(ref _sunset, value);
        }
        public string Sunrise
        {
            get => _sunrise;
            set => this.RaiseAndSetIfChanged(ref _sunrise, value);
        }
        public string City
        {
            get => _city;
            set => this.RaiseAndSetIfChanged(ref _city, value);
        }
        public string LastUpdate
        {
            get => _lastUpdate;
            set => this.RaiseAndSetIfChanged(ref _lastUpdate, value);
        }
    }
}
