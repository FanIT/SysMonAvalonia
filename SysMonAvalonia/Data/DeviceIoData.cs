using ReactiveUI;
using SysMonAvalonia.Models;

namespace SysMonAvalonia.Data
{
    public class DeviceIoData : ReactiveObject
    {
        private string _model;
        private string _letter;
        private long _totalSize;
        private long _usedSize;
        private byte _percentUsed;
        private byte _health;
        private bool _isHealth;
        private float _totalWrite;
        private bool _isTotalWrite;
        private float _writeIO;
        private float _readIO;
        private readonly ChartModel _chartModel = new();

        public string Model
        {
            get => _model;
            set => this.RaiseAndSetIfChanged(ref _model, value);
        }
        public string Letter
        {
            get => _letter;
            set => this.RaiseAndSetIfChanged(ref _letter, value);
        }
        public long TotalSize
        {
            get => _totalSize;
            set => this.RaiseAndSetIfChanged(ref _totalSize, value);
        }
        public long UsedSize
        {
            get => _usedSize;
            set => this.RaiseAndSetIfChanged(ref _usedSize, value);
        }
        public byte PercentUsed
        {
            get => _percentUsed;
            set => this.RaiseAndSetIfChanged(ref _percentUsed, value);
        }
        public byte Health
        {
            get => _health;
            set => this.RaiseAndSetIfChanged(ref _health, value);
        }
        public bool IsHealth
        {
            get => _isHealth;
            set => this.RaiseAndSetIfChanged(ref _isHealth, value);
        }
        public float TotalWrite
        {
            get => _totalWrite;
            set => this.RaiseAndSetIfChanged(ref _totalWrite, value);
        }
        public bool IsTotalWrite
        {
            get => _isTotalWrite;
            set => this.RaiseAndSetIfChanged(ref _isTotalWrite, value);
        }
        public float WriteIO
        {
            get => _writeIO;
            set => this.RaiseAndSetIfChanged(ref _writeIO, value);
        }
        public float ReadIO
        {
            get => _readIO;
            set => this.RaiseAndSetIfChanged(ref _readIO, value);
        }

        public ChartModel DeviceChart
        {
            get => _chartModel;
        }
    }
}
