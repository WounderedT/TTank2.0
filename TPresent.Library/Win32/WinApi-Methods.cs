using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TPresenter.Library.Win32
{
    public static partial class WinApi
    {
        [DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto, SetLastError = true)]
        public extern static IntPtr LoadKeyboardLayout(string keyboardLayoutID, uint flags);

        [DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto, SetLastError = true)]
        public extern static bool UnloadKeyboardLayout(IntPtr handle);

        [DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto, SetLastError = true)]
        public extern static IntPtr GetKeyboardLayout(IntPtr threadId);

        [DllImport("user32.dll")]
        public extern static IntPtr DefWindowProc(IntPtr hWnd, int uMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern bool EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE devMode);
    }
}
