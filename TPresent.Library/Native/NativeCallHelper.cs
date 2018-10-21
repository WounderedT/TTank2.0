using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TPresenter.Library.Native
{
    public class NativeCallHelper<TDelegate> where TDelegate : class
    {
        public static readonly TDelegate Invoke = Create();

        static TDelegate Create()
        {
            var type = typeof(TDelegate);
            var invoke = type.GetMethod("Invoke");

            Type[] parameters = invoke.GetParameters().Select(s => s.ParameterType).ToArray();
            if (parameters.Length == 0 || parameters[0] != typeof(IntPtr))
                throw new InvalidOperationException("First parameter must be function pointer");

            //Parameter 0 - pointer
            //Parameter 1 - instance
            //Parameter x - args

            var callParameters = parameters.Skip(1).Select(s => s == typeof(IntPtr) ? typeof(void*) : s).ToArray();

            DynamicMethod method = new DynamicMethod(String.Empty, invoke.ReturnType, parameters, Assembly.GetExecutingAssembly().ManifestModule);
            var generator = method.GetILGenerator();

            //arguments
            for (int ind = 1; ind < parameters.Length; ind++)
            {
                generator.Emit(OpCodes.Ldarg, ind);
            }

            //function pointer
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldind_I);

            //call
            generator.EmitCalli(OpCodes.Calli, CallingConvention.StdCall, invoke.ReturnType, callParameters);
            generator.Emit(OpCodes.Ret);
            return method.CreateDelegate<TDelegate>();
        }
    }
}
