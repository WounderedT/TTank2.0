using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPresenter.Library.Native
{
    public static partial class NativeCall<TResult>
    {
		public static TResult Method(IntPtr instance, int methodOffset)
        {
            return NativeCallHelper<Func<IntPtr, IntPtr, TResult>>.Invoke(NativeMethod.CalculateAddress(instance, methodOffset), instance);
        }
    }
}
