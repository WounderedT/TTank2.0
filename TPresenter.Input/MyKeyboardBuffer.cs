using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPresenter.Input
{
    public struct MyKeyboardBuffer
    {
        unsafe fixed byte myData[32];

        public unsafe void SetBit(byte bit, bool value)
        {
            if (bit == 0)
                return;     //Zero key is reserved for system use

            int bitOffset = bit % 8;
            byte mask = (byte)(1 << bitOffset); // shifting by (bitoffset & (8-1))
            fixed (byte* data = myData) // Pin the buffer to a fixed location in memory. data has a pointer to the firts element.
            {
                if (value)
                    *(data + bit / 8) |= mask;
                else
                    *(data + bit / 8) &= (byte)~mask;
            }
        }

        public unsafe bool AnyBitSet()
        {
            fixed (byte* data = myData)
            {
                long* bigData = (long*)data;
                return bigData[0] + bigData[1] + bigData[2] + bigData[3] != 0;
            }
        }

        public unsafe bool GetBit(byte bit)
        {
            int bitOffset = (bit % 8);
            byte mask = (byte)(1 << bitOffset);
            fixed (byte* data = myData)
            {
                return ((*(data + bit / 8)) & mask) != 0;
            }
        }
    }
}
