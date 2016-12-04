using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Ikarus
{
    class MemoryManagement
    {
        [DllImport("kernel32.dll")]
        private static extern bool SetProcessWorkingSetSize(IntPtr hProcess,
        int dwMinimumWorkingSetSize, int dwMaximumWorkingSetSize);

        public static void Reduce()
        {
            ReduceMemoryUsage();
        }

        private static void ReduceMemoryUsage()
        {
            try
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                    SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);
            }
            catch { }
        }
    }
}
