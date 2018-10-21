using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPresenterMath
{
    /// <summary>
    /// Represent packed 4 uint values.
    /// </summary>
    public struct Byte4 : IEquatable<Byte4>
    {
        private uint number;

        public uint Number
        {
            get { return number; }
            set { number = value; }
        }

        public Byte4(uint value0, uint value1, uint value2, uint value3)
        {
            number = Byte4.PackValue(value0, value1, value2, value3);
        }

        public Byte4(Vector4 value)
        {
            number = Byte4.PackValue(value.X, value.Y, value.Z, value.W);
        }

        public Vector4 ToVector4()
        {
            return new Vector4(number & byte.MaxValue,
                number >> 8 & byte.MaxValue,
                number >> 16 & byte.MaxValue,
                number >> 24 & byte.MaxValue);
        }

        static uint PackValue(float value0, float value1, float value2, float value3)
        {
            return PackageUtils.PackUnsigned(value0, byte.MaxValue) 
                | PackageUtils.PackUnsigned(value1, byte.MaxValue) << 8
                | PackageUtils.PackUnsigned(value2, byte.MaxValue) << 16
                | PackageUtils.PackUnsigned(value3, byte.MaxValue) << 24;
        }

        public static bool operator ==(Byte4 left, Byte4 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Byte4 left, Byte4 right)
        {
            return !left.Equals(right);
        }

        public override int GetHashCode()
        {
            return number.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is Byte4)
                return this.Equals(obj);
            else
                return false;
        }

        public bool Equals(Byte4 other)
        {
            return number.Equals(other);
        }

        public override string ToString()
        {
            return this.ToVector4().ToString();
        }
    }
}
