using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using SharpDX.DirectInput;
using TPresenter.Library.Native;

namespace TPresenter.Input
{
    static class DirectInputExtensions
    {
        public static unsafe Result TryAcquire(this Device device)
        {
            // Number 7 is offset in member function pointer table, it's same number as found in SharpDX.DirectInput.Device.Acquire()
            return (Result)NativeCall<int>.Method(device.NativePointer, 7);
        }
    }
}
