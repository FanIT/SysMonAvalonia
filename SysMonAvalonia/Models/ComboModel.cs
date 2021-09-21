using System;
using SysMonAvalonia.Data;
using Hardware;
using Hardware.Windows.Network;
using SysMonAvalonia.Localization;

namespace SysMonAvalonia.Models
{
    public class ComboModel : IDisposable
    {
        public ComboData ComboData { get; }

        public ComboModel()
        {
            this.ComboData = new();
            
            Computer.Open();

            NetworkInfo.SpeedUpdate();

            ComboData.GetTotalByteNet = NetworkInfo.ByteReceived;
            ComboData.SentTotalByteNet = NetworkInfo.ByteSend;

            ComboData.NetUnit = SettingData.CurrentSetting.NetworkUnit == SettingData.NetUnit.Megabits
                ? Language.Locale.Mbit
                : Language.Locale.Mbyte;
        }

        public void ComputerDataUpdate()
        {
            Computer.Update();

            ComboData.CpuUsedPercent = Computer.CPUPercent;
            ComboData.CpuTemp = Computer.CPUTemp;
            
            ComboData.GpuUsedPercent = Computer.GPUPercent;
            ComboData.GpuTemp = Computer.GPUTemp;
            ComboData.GpuTotalRam = Computer.GPUTotalRam;
            ComboData.GpuUsedRam = Computer.GPUUsedRam;
            ComboData.GpuUsedPercentRam = Computer.GPUPercentRam;
            
            ComboData.RamTotal = Computer.RamTotal;
            ComboData.RamUsed = Computer.RamUsed;
            ComboData.RamPercent = Computer.RamPercent;
            
            NetworkInfo.SpeedUpdate();

            ComboData.GetByteNet = NetworkInfo.ByteReceived - ComboData.GetTotalByteNet;
            ComboData.SentByteNet = NetworkInfo.ByteSend - ComboData.SentTotalByteNet;

            ComboData.GetTotalByteNet = NetworkInfo.ByteReceived;
            ComboData.SentTotalByteNet = NetworkInfo.ByteSend;
        }

        public void Dispose()
        {
            Computer.Close();
        }
    }
}
