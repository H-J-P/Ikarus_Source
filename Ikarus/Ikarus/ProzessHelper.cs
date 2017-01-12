using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace Ikarus
{
    class ProzessHelper
    {
        public static void SetFocusToExternalApp(string strProcessName)
        {
            Process[] arrProcesses = Process.GetProcessesByName(strProcessName);
            
            if (arrProcesses.Length > 0)
            {
                IntPtr ipHwnd = arrProcesses[0].MainWindowHandle;
                Thread.Sleep(100);
                SetForegroundWindow(ipHwnd);
            }
        }

    // API-declaration
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern bool SetForegroundWindow(IntPtr hWnd);

    }
}
