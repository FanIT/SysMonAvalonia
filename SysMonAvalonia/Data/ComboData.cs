using ReactiveUI;

namespace SysMonAvalonia.Data
{
    public class ComboData : ReactiveObject
    {
        private double _ramTotal;
        private double _ramUsed;
        private byte _ramPercent;

        private float _cpuUsedPercent;
        private float _cpuTemp;

        private float _gpuUsedPercent;
        private float _gpuTemp;

        private float _gpuTotalRam;
        private float _gpuUsedRam;
        private float _gpuUsedPercentRam;

        private long _netByteReceived;
        private long _netByteSend;
        private long _netTotalByteReceived;
        private long _netTotalByteSend;

        private string _netUnit;

        public double RamTotal
        {
            get => _ramTotal; 
            set => this.RaiseAndSetIfChanged(ref _ramTotal, value);
        }
        public double RamUsed
        {
            get => _ramUsed;
            set => this.RaiseAndSetIfChanged(ref _ramUsed, value);
        }
        public byte RamPercent
        {
            get => _ramPercent;
            set => this.RaiseAndSetIfChanged(ref _ramPercent, value);
        }
        public float CpuUsedPercent
        {
            get => _cpuUsedPercent;
            set => this.RaiseAndSetIfChanged(ref _cpuUsedPercent, value);
        }
        public float CpuTemp
        {
            get => _cpuTemp;
            set => this.RaiseAndSetIfChanged(ref _cpuTemp, value);
        }
        public float GpuUsedPercent
        {
            get => _gpuUsedPercent;
            set => this.RaiseAndSetIfChanged(ref _gpuUsedPercent, value);
        }
        public float GpuTemp
        {
            get => _gpuTemp;
            set => this.RaiseAndSetIfChanged(ref _gpuTemp, value);
        }
        public float GpuTotalRam
        {
            get => _gpuTotalRam;
            set => this.RaiseAndSetIfChanged(ref _gpuTotalRam, value);
        }
        public float GpuUsedRam
        {
            get => _gpuUsedRam;
            set => this.RaiseAndSetIfChanged(ref _gpuUsedRam, value);
        }
        public float GpuUsedPercentRam
        {
            get => _gpuUsedPercentRam;
            set => this.RaiseAndSetIfChanged(ref _gpuUsedPercentRam, value);
        }
        public long GetByteNet
        {
            get => _netByteReceived;
            set => this.RaiseAndSetIfChanged(ref _netByteReceived, value);
        }
        public long GetTotalByteNet
        {
            get => _netTotalByteReceived;
            set => this.RaiseAndSetIfChanged(ref _netTotalByteReceived, value);
        }
        public long SentByteNet
        {
            get => _netByteSend;
            set => this.RaiseAndSetIfChanged(ref _netByteSend, value);
        }
        public long SentTotalByteNet
        {
            get => _netTotalByteSend;
            set => this.RaiseAndSetIfChanged(ref _netTotalByteSend, value);
        }
        public string NetUnit
        {
            get => _netUnit;
            set => this.RaiseAndSetIfChanged(ref _netUnit, value);
        }
    }
}
