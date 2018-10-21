using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace TPresenter.Input
{
    static class WindowsKeyboard
    {
        [SuppressUnmanagedCodeSecurity]
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern unsafe bool GetKeyboardState(byte* data);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(int keyCode);

        public static MyKeyboardState GetCurrentState()
        {
            MyKeyboardBuffer buffer = new MyKeyboardBuffer();

            for(int ind = 0; ind < 256; ind++)
            {
                if ((((ushort)GetAsyncKeyState(ind)) >> 15) != 0)
                    buffer.SetBit((byte)ind, true);
            }

            return MyKeyboardState.FromBuffer(buffer);
        }

        static unsafe void CopyBuffer(byte* windowsKeyData, ref MyKeyboardBuffer buffer)
        {
            for(int ind = 0; ind < 256; ind++)
            {
                if ((windowsKeyData[ind] & 0x80) != 0)
                    buffer.SetBit((byte)ind, true);
            }
        }
    }
}
