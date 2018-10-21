using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPresenter.Library.Native
{
    public class NativeMethod
    {
        public static unsafe IntPtr CalculateAddress(IntPtr instance, int methodOffset)
        {
            return *(IntPtr*)instance.ToPointer() + methodOffset * sizeof(void*);
        }
    }
}
