using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TPresenter.Input
{
    public static class WindowsMouse
    {
        public class MouseMessageFilter : IMessageFilter
        {
            private const int WmMouseWheel = 0x20a;

            public bool PreFilterMessage(ref Message message)
            {
                if (message.Msg == WmMouseWheel)
                {
                    unsafe
                    {
                        int num = GET_WHEEL_DELTA_WPARAM(message.WParam);
                        currentWheel += num;
                    }
                }
                return false;
            }
        }

        static int currentWheel;
        static IntPtr windowHandle;

        static ushort HIWORD(IntPtr dwValue)
        {
            return (ushort)((((long)dwValue) >> 0x10) & 0xffff);
        }

        static int GET_WHEEL_DELTA_WPARAM(IntPtr wParam)
        {
            return (short)HIWORD(wParam);
        }

        [NativeCppClass]
        [StructLayout(LayoutKind.Sequential, Size = 8)]
        struct POINT
        {
            public int X, Y;
            public POINT(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        [SuppressUnmanagedCodeSecurity]
        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(int keyCode);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("user32.dll")]
        static extern unsafe int GetCursorPos(POINT* point);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("user32.dll")]
        static extern unsafe int ScreenToClient(void* handle, POINT* point);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("user32.dll")]
        static extern unsafe bool ClienToScree(void* handle, POINT* lpPoint);

        [DllImport("user32.dll")]
        static extern IntPtr SetCapture(IntPtr hWnd);

        public static void SetWindow(IntPtr wHandle)
        {
            windowHandle = wHandle;
            MessageFilterHook.AddMessageFilter(windowHandle, new MouseMessageFilter());
        }

        public static void SetPosition(int x, int y)
        {
            POINT point = new POINT(x, y);
            if(windowHandle != IntPtr.Zero)
            {
                unsafe
                {
                    ClienToScree(windowHandle.ToPointer(), &point);
                }
            }
            SetCursorPos(point.X, point.Y);
        }

        public static void GetPosition(out int x, out int y)
        {
            POINT point;

            unsafe
            {
                GetCursorPos(&point);
                if (windowHandle != IntPtr.Zero)
                    ScreenToClient(windowHandle.ToPointer(), &point);
            }
            x = point.X;
            y = point.Y;
        }

        public static void SetMouseCapture(IntPtr window)
        {
            SetCapture(window);
        }
    }
}
