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
    /// Represent packed 4 float values.
    /// </summary>
    public struct PackedVector4 : IEquatable<PackedVector4>
    {
        public ulong packedValue;

        public ulong PackedValue
        {
            get { return packedValue; }
            set { packedValue = value; }
        }

        public PackedVector4(float value0, float value1, float value2, float value3)
        {
            packedValue = PackedVector4.PackValue(value0, value1, value2, value3);
        }

        public PackedVector4(Vector4 value)
        {
            packedValue = PackedVector4.PackValue(value.X, value.Y, value.Z, value.W);
        }

        public Vector4 ToVector4()
        {
            Vector4 vector = new Vector4();
            vector.X = PackageUtils.Unpack((ushort)packedValue);
            vector.Y = PackageUtils.Unpack((ushort)(packedValue >> 16));
            vector.Z = PackageUtils.Unpack((ushort)(packedValue >> 32));
            vector.W = PackageUtils.Unpack((ushort)(packedValue >> 48));
            return vector;
        }

        static ulong PackValue(float value0, float value1, float value2, float value3)
        {
            return (ulong)(PackageUtils.Pack(value0) | PackageUtils.Pack(value1) << 16 | PackageUtils.Pack(value2) << 32 | PackageUtils.Pack(value3) << 48);
        }

        public static bool operator ==(PackedVector4 left, PackedVector4 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PackedVector4 left, PackedVector4 right)
        {
            return !left.Equals(right);
        }

        public override int GetHashCode()
        {
            return packedValue.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is PackedVector4)
                return this.Equals(obj);
            else
                return false;
        }

        public bool Equals(PackedVector4 other)
        {
            return packedValue.Equals(other);
        }

        public override string ToString()
        {
            return this.ToVector4().ToString();
        }
    }
}
