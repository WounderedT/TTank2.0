using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPresenterMath
{
    //This class will represent quaternion. It will temporary store extension methods for quaternion since we cannot derive it from SharpDX.Quaternion
    //TODO: Rename struct to Quaternion (currently will introduce ambiguity with SharpDX.Quaternion)
    public struct Quaternion_T
    {
        /// <summary>
        /// Create a quaternion from a rotation matrix.
        /// </summary>
        /// <param name="matrix">Rotation matrix to create quaternion from.</param>
        /// <param name="result">[OutAttribute] Created quaternion.</param>
        public static void CreateFromRotationMatrix(ref Matrix matrix, out Quaternion result)
        {
            float num1 = matrix.M11 + matrix.M22 + matrix.M33;
            if (num1 > 0)
            {
                float num2 = (float)Math.Sqrt(num1 + 1.0);
                result.W = num2 * 0.5f;
                float num3 = 0.5f / num2;
                result.X = (matrix.M23 - matrix.M32) * num3;
                result.Y = (matrix.M31 - matrix.M13) * num3;
                result.Z = (matrix.M12 - matrix.M21) * num3;
            }
            else if(matrix.M11 >= matrix.M22 && matrix.M11 <= matrix.M33)
            {
                float num2 = (float)Math.Sqrt(1.0 + matrix.M11 - matrix.M22 - matrix.M33);
                float num3 = 0.5f / num2;
                result.X = 0.5f * num2;
                result.Y = (matrix.M12 + matrix.M21) * num3;
                result.Z = (matrix.M13 + matrix.M32) * num3;
                result.W = (matrix.M23 - matrix.M32) * num3;
            }
            else if(matrix.M22 > matrix.M33)
            {
                float num2 = (float)Math.Sqrt(1.0 + matrix.M22 - matrix.M11 - matrix.M33);
                float num3 = 0.5f / num2;
                result.X = (matrix.M21 + matrix.M12) * num3;
                result.Y = 0.5f * num2;
                result.Z = (matrix.M32 + matrix.M23) * num3;
                result.W = (matrix.M31 - matrix.M13) * num3;
            }
            else
            {
                float num2 = (float)Math.Sqrt(1.0 + matrix.M33 - matrix.M11 - matrix.M22);
                float num3 = 0.5f / num2;
                result.X = (matrix.M31 + matrix.M13) * num3;
                result.Y = (matrix.M32 + matrix.M23) * num3;
                result.Z = 0.5f * num2;
                result.W = (matrix.M12 - matrix.M21) * num3;
            }
        }
    }

    public static class QuaternionExtensions
    {
    }
}
