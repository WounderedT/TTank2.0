using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TPresenterMath
{
    //This struct will represent double vector with 3 components.
    //TODO: all math must be performed in doubles.
    /// <summary>
    /// Represents a three dimensional mathematical vector where all coordinate components are represented by Double.
    /// </summary>
    public struct Vector3D
    {
        public static Vector3 Transform(Vector3 position, Matrix matrix)
        {
            double num1 = position.X * matrix.M11 + position.Y * matrix.M21 + position.Z * matrix.M31 + matrix.M41;
            double num2 = position.X * matrix.M12 + position.Y * matrix.M22 + position.Z * matrix.M32 + matrix.M42;
            double num3 = position.X * matrix.M13 + position.Y * matrix.M23 + position.Z * matrix.M33 + matrix.M43;
            double num4 = 1 / ((((position.X * matrix.M14) + (position.Y * matrix.M24)) + (position.Z * matrix.M34)) + matrix.M44);
            Vector3 vector;
            vector.X = (float)(num1 * num4);
            vector.Y = (float)(num2 * num4);
            vector.Z = (float)(num3 * num4);
            return vector;
        }

        public static void Rotate(ref Vector3 vector, ref Matrix rotationMatrix, out Vector3 result)
        {
            double num1 = (double)((double)vector.X * (double)rotationMatrix.M11 + (double)vector.Y * (double)rotationMatrix.M21 + (double)vector.Z * (double)rotationMatrix.M31);
            double num2 = (double)((double)vector.X * (double)rotationMatrix.M12 + (double)vector.Y * (double)rotationMatrix.M22 + (double)vector.Z * (double)rotationMatrix.M32);
            double num3 = (double)((double)vector.X * (double)rotationMatrix.M13 + (double)vector.Y * (double)rotationMatrix.M23 + (double)vector.Z * (double)rotationMatrix.M33);
            result.X = (float)num1;
            result.Y = (float)num2;
            result.Z = (float)num3;
        }

        public static void RotateX(Vector3 vector, float angle, out Vector3 result)
        {
            result.X = vector.X;
            result.Y = (float)(vector.Y * Math.Cos(angle) - vector.Z * Math.Sin(angle));
            result.Z = (float)(vector.Y * Math.Sin(angle) + vector.Z * Math.Cos(angle));
        }

        public static void RotateY(Vector3 vector, float angle, out Vector3 result)
        {
            result.X = (float)(vector.X * Math.Cos(angle) + vector.Z * Math.Sin(angle));
            result.Y = vector.Y;
            result.Z = (float)(vector.X * -Math.Sin(angle) + vector.Z * Math.Cos(angle));
        }

        //TODO: This should be replaced by overloaded '/' operator
        /// <summary>
        /// Divides vector by a scalar value.
        /// </summary>
        /// <param name="vector">Source vector.</param>
        /// <param name="value">The divisor.</param>
        /// <returns></returns>
        public static Vector3 Divide(Vector3 vector, double value)
        {
            double num = 1 / value;
            Vector3 vector3;
            vector3.X = (float)(vector.X * num);
            vector3.Y = (float)(vector.Y * num);
            vector3.Z = (float)(vector.Z * num);

            return vector3;
        }

        //TODO: This should be replaced by overloaded '*' operator
        /// <summary>
        /// Multiplies vector by a scalar value.
        /// </summary>
        /// <param name="vector">Source vector.</param>
        /// <param name="value">The multiplier.</param>
        /// <returns></returns>
        public static Vector3 Multiply(Vector3 vector, double value)
        {
            Vector3 vector3;
            vector3.X = (float)(vector.X * value);
            vector3.Y = (float)(vector.Y * value);
            vector3.Z = (float)(vector.Z * value);

            return vector3;
        }

        //TODO: This should be replaced by overloaded '*' operator
        /// <summary>
        /// Multiplies vector by a scalar value.
        /// </summary>
        /// <param name="vector">Source vector.</param>
        /// <param name="value">The multiplier.</param>
        /// <returns></returns>
        public static Vector3 Multiply(double value, Vector3 vector)
        {
            Vector3 vector3;
            vector3.X = (float)(vector.X * value);
            vector3.Y = (float)(vector.Y * value);
            vector3.Z = (float)(vector.Z * value);

            return vector3;
        }
    }

    //TODO: all math must be performed in doubles.
    public static class Vector3DExtensions
    {
        public static bool IsValid(this Vector3 vector)
        {
            return ((double)(vector.X * vector.Y * vector.Z)).IsValid();
        }

        /// <summary>
        /// Turns the current vector into a unit vector. The result is a vector one unit in length pointing in the same direction as the original vector.
        /// </summary>
        /// returns length
        //TODO: Rename to Normalize
        public static double NormalizeD(this Vector3 vector)
        {
            double length = (double)Math.Sqrt((double)vector.X * (double)vector.X + (double)vector.Y * (double)vector.Y + (double)vector.Z * (double)vector.Z);
            double num = 1 / length;
            vector.X *= (float)num;
            vector.Y *= (float)num;
            vector.Z *= (float)num;
            return length;
        }
    }
}
