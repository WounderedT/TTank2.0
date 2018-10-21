using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPresenterMath
{
    //This class will represent vector with 3 components. It will temporary store extension methods for vector3 since we cannot derive it from SharpDX.Vector3
    //TODO: Rename struct to Vector3 (currently will introduce ambiguity with SharpDX.Vector3)
    public struct Vector3_T
    {
        public static Vector3 Transform(Vector3 position, ref Matrix transform)
        {
            Vector3.Transform(ref position, ref transform, out position);
            return position;
        }

        public static float Dot(ref Vector3 vector1, ref Vector3 vector2)
        {
            float result = 0f;
            Vector3.Dot(ref vector1, ref vector2, out result);
            return result;
        }

        public static Vector4 Multiply(Vector4 vector, Matrix matrix)
        {
            Vector4 res = new Vector4();
            res.X = vector.X * matrix.M11 + vector.Y * matrix.M21 + vector.Z * matrix.M31 + vector.W * matrix.M41;
            res.Y = vector.X * matrix.M12 + vector.Y * matrix.M22 + vector.Z * matrix.M32 + vector.W * matrix.M42;
            res.Z = vector.X * matrix.M13 + vector.Y * matrix.M23 + vector.Z * matrix.M33 + vector.W * matrix.M43;
            res.W = vector.X * matrix.M14 + vector.Y * matrix.M24 + vector.Z * matrix.M34 + vector.W * matrix.M44;
            return res;
        }

        public static Vector3 Multiply(Vector3 vector, Matrix3x3 matrix)
        {
            Vector3 res = new Vector3();
            res.X = vector.X * matrix.M11 + vector.Y * matrix.M21 + vector.Z * matrix.M31;
            res.Y = vector.X * matrix.M12 + vector.Y * matrix.M22 + vector.Z * matrix.M32;
            res.Z = vector.X * matrix.M13 + vector.Y * matrix.M23 + vector.Z * matrix.M33;
            return res;
        }

        public static Vector3 New(double v0, double v1, double v2)
        {
            Vector3 result = new Vector3((float)v0, (float)v1, (float)v2);
            return result;
        }
    }

    
    public static class Vectror3Extensions
    {
    }
}
