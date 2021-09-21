using System.Runtime.InteropServices;

namespace Hardware.Windows.RAM
{
    static class NativeMethods
    {
        const string KernelDll = "kernel32.dll";

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(KernelDll, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool GlobalMemoryStatusEx([In, Out] MemoryStatusEx lpBuffer);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(KernelDll)]
        public static extern bool GetPhysicallyInstalledSystemMemory(out ulong totalMemoryInKilobytes);
    }
}
