using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Generic
{
    static class ListInternalAccessor<T>
    {
        public static Func<List<T>, T[]> GetArray;
        public static Action<List<T>, int> SetSize;

        static ListInternalAccessor()
        {
            var dm = new DynamicMethod("get", MethodAttributes.Static | MethodAttributes.Public, CallingConventions.Standard, typeof(T[]), new Type[] { typeof(List<T>) }, typeof(ListInternalAccessor<T>), true);
            var il = dm.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);   // Load List<T> argument
            il.Emit(OpCodes.Ldfld, typeof(List<T>).GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance)); // Find _field value and push it onto the stack.
            il.Emit(OpCodes.Ret); // Return field
            GetArray = (Func<List<T>, T[]>)dm.CreateDelegate(typeof(Func<List<T>, T[]>));

            var dm2 = new DynamicMethod("set", MethodAttributes.Static | MethodAttributes.Public, CallingConventions.Standard, null, new Type[] { typeof(List<T>), typeof(int) }, typeof(ListInternalAccessor<T>), true);
            var il2 = dm2.GetILGenerator();
            il2.Emit(OpCodes.Ldarg_0); // Load List<T> argument
            il2.Emit(OpCodes.Ldarg_1); // Load new value on stack
            il2.Emit(OpCodes.Stfld, typeof(List<T>).GetField("_size", BindingFlags.NonPublic | BindingFlags.Instance)); // Store new value into _size field of the List<T>

            // Increment version
            var versionField = typeof(List<T>).GetField("_version", BindingFlags.NonPublic | BindingFlags.Instance);
            il2.Emit(OpCodes.Ldarg_0); // Load List<T> argument
            il2.Emit(OpCodes.Dup); // Duplicate
            il2.Emit(OpCodes.Ldfld, versionField); // Replace second List<T> by List<T>._version field value
            il2.Emit(OpCodes.Ldc_I4_1); // Load 1 onto stack
            il2.Emit(OpCodes.Add); // Replace List<T>._version value and 1 with it's sum
            il2.Emit(OpCodes.Stfld, versionField); // Load first List<T> and sum, write new sum

            il2.Emit(OpCodes.Ret);
            SetSize = (Action<List<T>, int>)dm2.CreateDelegate(typeof(Action<List<T>, int>));
        }
    }

    public static class ListExtensions
    {
        public static T[] GetInternalArray<T>(this List<T> self)
        {
            return ListInternalAccessor<T>.GetArray(self);
        }

        public static void AddArray<T>(this List<T> self, T[] array)
        {
            AddArray(self, array, array.Length);
        }

        public static void AddArray<T>(this List<T> self, T[] array, int itemCount)
        {
            if (self.Capacity < self.Count + itemCount)
                self.Capacity = self.Count + itemCount;

            Array.Copy(array, 0, self.GetInternalArray(), self.Count, itemCount);
            ListInternalAccessor<T>.SetSize(self, self.Count + itemCount);
        }
    }
}
