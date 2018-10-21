using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPresenter.Input
{
    public struct MyKeyboardState
    {
        MyKeyboardBuffer buffer;

        public void GetPressedKeys(List<Keys> keys)
        {
            keys.Clear();

            for(int ind = 1; ind < 255; ind++)
            {
                if (buffer.GetBit((byte)ind))
                    keys.Add((Keys)ind);
            }
        }

        public bool IsAnyKeyPressed()
        {
            return buffer.AnyBitSet();
        }

        public void SetKey(Keys key, bool value)
        {
            buffer.SetBit((byte)key, value);
        }

        public static MyKeyboardState FromBuffer(MyKeyboardBuffer buffer)
        {
            return new MyKeyboardState() { buffer = buffer };
        }

        public bool IsKeyDown(Keys key)
        {
            return buffer.GetBit((byte)key);
        }

        public bool IsKeyUp(Keys key)
        {
            return !IsKeyDown(key);
        }
    }
}
