using System;
using LibreHardwareMonitor.Hardware;
using Hardware.Windows.RAM;

namespace Hardware
{
    public static class Computer
    {
        private static LibreHardwareMonitor.Hardware.Computer pc;

        private static ISensor CpuPercentSensor;
        private static ISensor CpuTempSensor;

        private static ISensor GpuPercentSensor;
        private static ISensor GpuTempSensor;

        private static ISensor GpuRamPercentSensor;
        private static ISensor GpuRamUsedSenser;

        public static float CPUPercent { get; private set; }
        public static float CPUTemp { get; private set; }

        public static float GPUPercent { get; private set; }
        public static float GPUTemp { get; private set; }

        public static float GPUTotalRam { get; private set; }
        public static float GPUUsedRam { get; private set; }
        public static float GPUPercentRam { get; private set; }

        public static double RamTotal { get; private set; }
        public static double RamUsed { get; private set; }
        public static double RamFree { get; private set; }
        public static byte RamPercent { get; private set; }

        static Computer()
        {
            CPUPercent = 0;
            CPUTemp = 0;
            GPUPercent = 0;
            GPUTemp = 0;
            GPUPercentRam = 0;
            GPUUsedRam = 0;
        }

        private static void SetSensors()
        {
            foreach (IHardware hardware in pc.Hardware)
            {
                hardware.Update();

                if (hardware.HardwareType == HardwareType.Motherboard)
                {
                    foreach (IHardware sub in hardware.SubHardware)
                    {
                        sub.Update();

                        if (sub.HardwareType == HardwareType.SuperIO)
                        {
                            foreach (ISensor sensor in sub.Sensors)
                            {
                                if (sensor.SensorType == SensorType.Temperature && sensor.Index == 0)
                                {
                                    CpuTempSensor = sensor;
                                    CpuTempSensor.ValuesTimeWindow = TimeSpan.Zero;
                                    break;
                                }
                            }

                            break;
                        }
                    }
                }

                if (hardware.HardwareType == HardwareType.Cpu)
                {
                    foreach (ISensor sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Load && sensor.Index == 0)
                        {
                            CpuPercentSensor = sensor;
                            CpuPercentSensor.ValuesTimeWindow = TimeSpan.Zero;
                            break;
                        }
                    }
                }

                if (hardware.HardwareType == HardwareType.GpuNvidia || hardware.HardwareType == HardwareType.GpuAmd)
                {
                    foreach (ISensor sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Load && sensor.Index == 0)
                        {
                            GpuPercentSensor = sensor;
                            GpuPercentSensor.ValuesTimeWindow = TimeSpan.Zero;
                        }
                        if (sensor.SensorType == SensorType.Temperature)
                        {
                            GpuTempSensor = sensor;
                            GpuTempSensor.ValuesTimeWindow = TimeSpan.Zero;
                        }
                        if (sensor.SensorType == SensorType.SmallData && sensor.Index == 2) GPUTotalRam = sensor.Value.Value;
                        if (sensor.SensorType == SensorType.SmallData && sensor.Index == 1)
                        {
                            GpuRamUsedSenser = sensor;
                            GpuRamUsedSenser.ValuesTimeWindow = TimeSpan.Zero;
                        }
                        if (sensor.SensorType == SensorType.Load && sensor.Index == 4)
                        {
                            GpuRamPercentSensor = sensor;
                            GpuRamPercentSensor.ValuesTimeWindow = TimeSpan.Zero;
                        }
                    }
                }
            }
        }

        public static void Open()
        {
            if (pc == null)
            {
                pc = new();
                pc.IsCpuEnabled = true;
                pc.IsMotherboardEnabled = true;
                pc.IsGpuEnabled = true;
                pc.Open();
            }

            SetSensors();

            RamUpdate();
        }

        private static void RamUpdate()
        {
            double devider = 1073741824;

            MemoryStatusEx msu = new();

            try
            { NativeMethods.GlobalMemoryStatusEx(msu); }
            catch { return; }

            RamTotal = msu.TotalPhys / devider;
            RamFree = msu.AvailPhys / devider;
            RamUsed = (msu.TotalPhys - msu.AvailPhys) / devider;
            RamPercent = (byte)((msu.TotalPhys - msu.AvailPhys) / (msu.TotalPhys / 100));
        }

        public static void Update()
        {
            CpuPercentSensor.Hardware.Update();
            CPUPercent = CpuPercentSensor.Value.Value;

            CpuTempSensor.Hardware.Update();
            CPUTemp = CpuTempSensor.Value.Value;

            GpuPercentSensor.Hardware.Update();
            GPUPercent = GpuPercentSensor.Value.Value;

            GpuTempSensor.Hardware.Update();
            GPUTemp = GpuTempSensor.Value.Value;

            GpuRamUsedSenser.Hardware.Update();
            GPUUsedRam = GpuRamUsedSenser.Value.Value;

            GpuRamPercentSensor.Hardware.Update();
            GPUPercentRam = 100 - GpuRamPercentSensor.Value.Value;

            RamUpdate();
        }

        public static void Close()
        {
            pc.Close();
        }
    }
}
