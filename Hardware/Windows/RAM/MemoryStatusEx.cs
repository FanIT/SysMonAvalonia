using System.Runtime.InteropServices;

namespace Hardware.Windows.RAM
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    class MemoryStatusEx
    {
        public uint dwLength;
        public uint dwMemoryLoad;
        public ulong TotalPhys;
        public ulong AvailPhys;
        public ulong TotalPageFile;
        public ulong AvailPageFile;
        public ulong TotalVirtual;
        public ulong AvailVirtual;
        public ulong AvailExtendedVirtual;

        public MemoryStatusEx()
        {
            dwLength = (uint)Marshal.SizeOf(typeof(MemoryStatusEx));
        }
    }
}
