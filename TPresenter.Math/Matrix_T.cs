using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPresenterMath
{
    //This struct will represent Matrix. It will temporary store extension methods for matrix since we cannot derive it from SharpDX.Matrix
    //TODO: Rename struct to Matrix (currently will introduce ambiguity with SharpDX.Matrix)
    public struct Matrix_T
    {
        public static Matrix Normalize(Matrix matrix)
        {
            Matrix m = matrix;
            m.Right = Vector3.Normalize(m.Right);
            m.Up = Vector3.Normalize(m.Up);
            m.Forward = Vector3.Normalize(m.Forward);
            Debug.Assert(!m.IsNan());
            return m;
        }

        //Should not be used. All calculations should performed for Left-Handed coordinate system (DirectX default)
        public static Matrix CreatePerspectiveForRhInfiniteComplemetary(float fov, float aspectRatio, float nearestPlaneDistance)
        {
            float num1 = 1f / (float)Math.Tan(fov * 0.5);
            float num2 = num1 / aspectRatio;
            Matrix matrix;
            matrix.M11 = num2;
            matrix.M12 = matrix.M13 = matrix.M14 = 0.0f;
            matrix.M22 = num1;
            matrix.M21 = matrix.M23 = matrix.M24 = 0.0f;
            matrix.M31 = matrix.M32 = 0.0f;
            matrix.M33 = 0;
            matrix.M34 = -1f;
            matrix.M41 = matrix.M42 = matrix.M44 = 0.0f;
            matrix.M43 = nearestPlaneDistance;

            return matrix;
        }

        public static Matrix CreatePerspectiveForLhInfiniteComplemetary(float fov, float aspectRatio, float nearestPlaneDistance)
        {
            float num1 = 1f / (float)Math.Tan(fov * 0.5);
            float num2 = num1 / aspectRatio;
            Matrix matrix;
            matrix.M11 = num2;
            matrix.M12 = matrix.M13 = matrix.M14 = 0.0f;
            matrix.M22 = num1;
            matrix.M21 = matrix.M23 = matrix.M24 = 0.0f;
            matrix.M31 = matrix.M32 = 0.0f;
            matrix.M33 = 1;
            matrix.M34 = 1f;
            matrix.M41 = matrix.M42 = matrix.M44 = 0.0f;
            matrix.M43 = -nearestPlaneDistance;

            return matrix;
        }

        /// <summary>
        /// Creates a rotation matrix around the X-axis
        /// </summary>
        /// <param name="angle">Angle of rotation in degrees</param>
        /// <returns></returns>
        public static Matrix RotationX(float angle)
        {
            angle = MathHelper.ToRadians(angle);
            Matrix matrix = Matrix.Zero;
            matrix.M11 = matrix.M44 = 1;
            matrix.M22 = (float)Math.Cos(angle);
            matrix.M23 = (float)Math.Sin(angle);
            matrix.M32 = -(float)Math.Sin(angle);
            matrix.M33 = (float)Math.Cos(angle);
            return matrix;
        }

        public static Matrix New(double[] values)
        {
            Matrix result = new Matrix();
            result.M11 = (float)values[0];
            result.M12 = (float)values[4];
            result.M13 = (float)values[8];
            result.M14 = (float)values[12];

            result.M21 = (float)values[1];
            result.M22 = (float)values[5];
            result.M23 = (float)values[9];
            result.M24 = (float)values[13];

            result.M31 = (float)values[2];
            result.M32 = (float)values[6];
            result.M33 = (float)values[10];
            result.M34 = (float)values[14];

            result.M41 = (float)values[3];
            result.M42 = (float)values[7];
            result.M43 = (float)values[11];
            result.M44 = (float)values[15];
            return result;
        }

        public static Matrix New(float[] values)
        {
            Matrix result = new Matrix();
            result.M11 = values[0];
            result.M12 = values[4];
            result.M13 = values[8];
            result.M14 = values[12];

            result.M21 = values[1];
            result.M22 = values[5];
            result.M23 = values[9];
            result.M24 = values[13];

            result.M31 = values[2];
            result.M32 = values[6];
            result.M33 = values[10];
            result.M34 = values[14];

            result.M41 = values[3];
            result.M42 = values[7];
            result.M43 = values[11];
            result.M44 = values[15];
            return result;
        }

        //This method exists because scenes and objects are currently stored in XML format.
        public static Matrix New(String values)
        {
            Matrix result = new Matrix();
            String[] valuesSplitted = values.Split(' ');
            result.M11 = Single.Parse(valuesSplitted[0]);
            result.M12 = Single.Parse(valuesSplitted[4]);
            result.M13 = Single.Parse(valuesSplitted[8]);
            result.M14 = Single.Parse(valuesSplitted[12]);

            result.M21 = Single.Parse(valuesSplitted[1]);
            result.M22 = Single.Parse(valuesSplitted[5]);
            result.M23 = Single.Parse(valuesSplitted[9]);
            result.M24 = Single.Parse(valuesSplitted[13]);

            result.M31 = Single.Parse(valuesSplitted[2]);
            result.M32 = Single.Parse(valuesSplitted[6]);
            result.M33 = Single.Parse(valuesSplitted[10]);
            result.M34 = Single.Parse(valuesSplitted[14]);

            result.M41 = Single.Parse(valuesSplitted[3]);
            result.M42 = Single.Parse(valuesSplitted[7]);
            result.M43 = Single.Parse(valuesSplitted[11]);
            result.M44 = Single.Parse(valuesSplitted[15]);
            return result;
        }
    }

    public static class MatrixExstensions
    {
        public static bool IsNan(this Matrix matrix)
        {
            return float.IsNaN(matrix.M11 + matrix.M12 + matrix.M13 + matrix.M14 + matrix.M21 + matrix.M22 + matrix.M23 + matrix.M24
                + matrix.M31 + matrix.M32 + matrix.M33 + matrix.M34 + matrix.M41 + matrix.M42 + matrix.M43 + matrix.M44);
        }
    }
}
