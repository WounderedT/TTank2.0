using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class DoubleExtensions
    {
        /// <summary>
        /// Returns true if double is valid
        /// </summary>
        public static bool IsValid(this double f)
        {
            return !double.IsNaN(f) && !double.IsInfinity(f);
        }

        ///// <summary>
        ///// Returns true if double is valid.
        ///// </summary>
        //public static bool IsValid(this double d)
        //{
        //    return !double.IsNaN(d) && !double.IsInfinity(d);
        //}

        [Conditional("DEBUG")]
        public static void AssertIsValid(this double d)
        {
            Debug.Assert(IsValid(d));
        }
    }
}
