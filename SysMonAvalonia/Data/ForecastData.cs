using ReactiveUI;

namespace SysMonAvalonia.Data
{
    public class ForecastData : ReactiveObject
    {
        private string _dateTime;
        private string _icon;
        private sbyte _tempMin;
        private sbyte _tempMax;
        private string _phase;

        public string DateTime
        {
            get => _dateTime;
            set => this.RaiseAndSetIfChanged(ref _dateTime, value);
        }
        public string Icon
        {
            get => _icon;
            set => this.RaiseAndSetIfChanged(ref _icon, value);
        }
        public sbyte TempMin
        {
            get => _tempMin;
            set => this.RaiseAndSetIfChanged(ref _tempMin, value);
        }
        public sbyte TempMax
        {
            get => _tempMax;
            set => this.RaiseAndSetIfChanged(ref _tempMax, value);
        }
        public string Phase
        {
            get => _phase;
            set => this.RaiseAndSetIfChanged(ref _phase, value);
        }
    }
}
