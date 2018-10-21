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
    //This class will represent double Matrix.
    //TODO: All return types must be MatrixD (not Matrix)
    public struct MatrixD
    {
        /// <summary>
        /// Creates a world matrix with the specified parameters.
        /// </summary>
        /// <param name="position">Position of the object. This value is used in translation operations.</param><param name="forward">Forward direction of the object.</param><param name="up">Upward direction of the object; usually [0, 1, 0].</param>
        public static Matrix CreateWorld(Vector3 position, Vector3 forward, Vector3 up)
        {
            Vector3 vector3_1 = Vector3.Normalize(-forward);
            Vector3 vector2 = Vector3.Normalize(Vector3.Cross(up, vector3_1));
            Vector3 vector3_2 = Vector3.Cross(vector3_1, vector2);
            Matrix matrix;
            matrix.M11 = vector2.X;
            matrix.M12 = vector2.Y;
            matrix.M13 = vector2.Z;
            matrix.M14 = 0.0f;
            matrix.M21 = vector3_2.X;
            matrix.M22 = vector3_2.Y;
            matrix.M23 = vector3_2.Z;
            matrix.M24 = 0.0f;
            matrix.M31 = vector3_1.X;
            matrix.M32 = vector3_1.Y;
            matrix.M33 = vector3_1.Z;
            matrix.M34 = 0.0f;
            matrix.M41 = position.X;
            matrix.M42 = position.Y;
            matrix.M43 = position.Z;
            matrix.M44 = 1f;
            return matrix;
        }

        /// <summary>
        /// Returns a matrix that can be used to rotate a set of verteces around the x-axis.
        /// </summary>
        /// <param name="radians">The amount of radians by which to rotate around the x-axis.</param>
        /// <returns></returns>
        //TODO: All math must be performed in doubles.
        public static Matrix CreateRotationX(double radians)
        {
            var num1 = (float)Math.Cos(radians);
            var num2 = (float)Math.Sin(radians);

            Matrix matrix;
            matrix.M11 = 1f;
            matrix.M12 = 0.0f;
            matrix.M13 = 0.0f;
            matrix.M14 = 0.0f;
            matrix.M21 = 0.0f;
            matrix.M22 = num1;
            matrix.M23 = num2;
            matrix.M24 = 0.0f;
            matrix.M31 = 0.0f;
            matrix.M32 = -num2;
            matrix.M33 = num1;
            matrix.M34 = 0.0f;
            matrix.M41 = 0.0f;
            matrix.M42 = 0.0f;
            matrix.M43 = 0.0f;
            matrix.M44 = 1f;
            return matrix;
        }

        /// <summary>
        /// Returns a matrix that can be used to rotate a set of verteces around the y-axis.
        /// </summary>
        /// <param name="radians">The amount of radians by which to rotate around the y-axis.</param>
        /// <returns></returns>
        //TODO: All math must be performed in doubles.
        public static Matrix CreateRotationY(double radians)
        {
            var num1 = (float)Math.Cos(radians);
            var num2 = (float)Math.Sin(radians);

            Matrix matrix;
            matrix.M11 = num1;
            matrix.M12 = 0.0f;
            matrix.M13 = -num2;
            matrix.M14 = 0.0f;
            matrix.M21 = 0.0f;
            matrix.M22 = 1f;
            matrix.M23 = 0.0f;
            matrix.M24 = 0.0f;
            matrix.M31 = num2;
            matrix.M32 = 0.0f;
            matrix.M33 = num1;
            matrix.M34 = 0.0f;
            matrix.M41 = 0.0f;
            matrix.M42 = 0.0f;
            matrix.M43 = 0.0f;
            matrix.M44 = 1f;
            return matrix;
        }

        /// <summary>
        /// Returns a matrix that can be used to rotate a set of verteces around the z-axis.
        /// </summary>
        /// <param name="radians">The amount of radians by which to rotate around the z-axis.</param>
        /// <returns></returns>
        //TODO: All math must be performed in doubles.
        public static Matrix CreateRotationZ(double radians)
        {
            var num1 = (float)Math.Cos(radians);
            var num2 = (float)Math.Sin(radians);

            Matrix matrix;
            matrix.M11 = num1;
            matrix.M12 = num2;
            matrix.M13 = 0.0f;
            matrix.M14 = 0.0f;
            matrix.M21 = -num2;
            matrix.M22 = num1;
            matrix.M23 = 0.0f;
            matrix.M24 = 0.0f;
            matrix.M31 = 0.0f;
            matrix.M32 = 0.0f;
            matrix.M33 = 1f;
            matrix.M34 = 0.0f;
            matrix.M41 = 0.0f;
            matrix.M42 = 0.0f;
            matrix.M43 = 0.0f;
            matrix.M44 = 1f;
            return matrix;
        }

        /// <summary>
        /// Builds perspective right handed projection matrix based on a field of view.
        /// </summary>
        /// <param name="fovWithZoom"></param>
        /// <param name="aspectRatio"></param>
        /// <param name="nearPlaneDistance"></param>
        /// <param name="farPlaneDistance"></param>
        public static Matrix CreatePerspectiveFieldOfViewRh(double fovWithZoom, double aspectRatio, double nearPlaneDistance, double farPlaneDistance)
        {
            if (fovWithZoom <= 0.0 || fovWithZoom >= 3.14159274101257)
                throw new ArgumentOutOfRangeException("fovWithZoom", string.Format(CultureInfo.CurrentCulture, "OutRangeFieldOfViewWithZoom", new object[1] { "fovWithZoom" }));
            else if (nearPlaneDistance <= 0.0)
                throw new ArgumentOutOfRangeException("nearPlaneDistance", string.Format(CultureInfo.CurrentCulture, "NegativePlaneDistance", new object[1] { "nearPlaneDistace" }));
            else if (farPlaneDistance <= 0.0)
                throw new ArgumentOutOfRangeException("farPlaneDistance", string.Format(CultureInfo.CurrentCulture, "NegativePlaneDistance", new object[1] { "farPlaneDistance" }));

            if (nearPlaneDistance >= farPlaneDistance)
                throw new ArgumentOutOfRangeException("nearPlaneDistance", "OppositePlanes");
            double num1 = 1f / Math.Tan(fovWithZoom * 0.5);
            double num2 = num1 / aspectRatio;

            Matrix matrix;
            matrix.M11 = (float)num2;
            matrix.M12 = matrix.M13 = matrix.M14 = 0.0f;
            matrix.M22 = (float)num1; ;
            matrix.M21 = matrix.M23 = matrix.M24 = 0.0f;
            matrix.M31 = matrix.M32 = 0.0f;
            matrix.M33 = (float)(farPlaneDistance / (farPlaneDistance - nearPlaneDistance));
            matrix.M34 = 1.0f;
            matrix.M41 = matrix.M42 = matrix.M44 = 0.0f;
            matrix.M43 = (float)(nearPlaneDistance * farPlaneDistance / (nearPlaneDistance - farPlaneDistance));
            return matrix;
        }

        /// <summary>
        /// Builds perspective right handed projection matrix based on a field of view.
        /// </summary>
        /// <param name="fovWithZoom"></param>
        /// <param name="aspectRatio"></param>
        /// <param name="nearPlaneDistance"></param>
        /// <param name="farPlaneDistance"></param>
        public static Matrix CreatePerspectiveFieldOfViewLh(double fovWithZoom, double aspectRatio, double nearPlaneDistance, double farPlaneDistance)
        {
            if (fovWithZoom <= 0.0 || fovWithZoom >= 3.14159274101257)
                throw new ArgumentOutOfRangeException("fovWithZoom", string.Format(CultureInfo.CurrentCulture, "OutRangeFieldOfViewWithZoom", new object[1] { "fovWithZoom" }));
            else if (nearPlaneDistance <= 0.0)
                throw new ArgumentOutOfRangeException("nearPlaneDistance", string.Format(CultureInfo.CurrentCulture, "NegativePlaneDistance", new object[1] { "nearPlaneDistace" }));
            else if (farPlaneDistance <= 0.0)
                throw new ArgumentOutOfRangeException("farPlaneDistance", string.Format(CultureInfo.CurrentCulture, "NegativePlaneDistance", new object[1] { "farPlaneDistance" }));

            if (nearPlaneDistance >= farPlaneDistance)
                throw new ArgumentOutOfRangeException("nearPlaneDistance", "OppositePlanes");
            double num1 = 1f / Math.Tan(fovWithZoom * 0.5);
            double num2 = num1 / aspectRatio;

            Matrix matrix;
            matrix.M11 = (float)num2;
            matrix.M12 = matrix.M13 = matrix.M14 = 0.0f;
            matrix.M22 = (float)num1; ;
            matrix.M21 = matrix.M23 = matrix.M24 = 0.0f;
            matrix.M31 = matrix.M32 = 0.0f;
            matrix.M33 = (float)(farPlaneDistance / (farPlaneDistance - nearPlaneDistance));
            matrix.M34 = 1.0f;
            matrix.M41 = matrix.M42 = matrix.M44 = 0.0f;
            matrix.M43 = (float)(nearPlaneDistance * farPlaneDistance / (nearPlaneDistance - farPlaneDistance));
            return matrix;
        }

        public static Matrix CreateFromDir(Vector3 dir, Vector3 suggestedUp)
        {
            Vector3 right = Vector3.Cross(dir, suggestedUp);
            Vector3 newUp = Vector3.Cross(right, dir);

            return MatrixD.CreateWorld(Vector3.Zero, dir, newUp);
        }

        //Not tested!
        public static Matrix CreateFromAxisAngle(Vector3 axis, double angle)
        {
            double num1 = axis.X;
            double num2 = axis.Y;
            double num3 = axis.Z;
            double num4 = Math.Sin(angle);
            double num5 = Math.Cos(angle);
            double num6 = num1 * num1;
            double num7 = num2 * num2;
            double num8 = num3 * num3;
            double num9 = num1 * num2;
            double num10 = num1 * num3;
            double num11 = num2 * num3;

            Matrix matrix;
            matrix.M11 = (float)(num6 + num5 * (1 - num6));
            matrix.M12 = (float)(num9 - num5 * num9 + num4 * num3);
            matrix.M13 = (float)(num10 - num5 * num10 - num4 * num2);
            matrix.M14 = 0.0f;
            matrix.M21 = (float)(num9 - num5 * num9 - num4 * num3);
            matrix.M22 = (float)(num7 + num5 * (1 - num7));
            matrix.M23 = (float)(num11 - num5 * num11 + num4 * num1);
            matrix.M24 = 0f;
            matrix.M31 = (float)(num10 - num5 * num10 + num4 * num2);
            matrix.M32 = (float)(num11 - num5 * num11 - num4 * num1);
            matrix.M33 = (float)(num8 + num5 * (1 - num8));
            matrix.M34 = 0.0f;
            matrix.M41 = 0.0f;
            matrix.M42 = 0.0f;
            matrix.M43 = 0.0f;
            matrix.M44 = 1f;
            return matrix;
        }

        //Not tested!
        /// <summary>
        /// Creates a rotation matrix from a Quaternion.
        /// </summary>
        /// <param name="quaternion">Quaternion to craete the matrix from.</param>
        /// <param name="result">[OutAttribute] Craete matrix.</param>
        public static void CreateFromQuaternion(ref Quaternion quaternion, out Matrix result)
        {
            double num1 = quaternion.X * quaternion.X;
            double num2 = quaternion.Y * quaternion.Y;
            double num3 = quaternion.Z * quaternion.Z;
            double num4 = quaternion.X * quaternion.Y;
            double num5 = quaternion.Z * quaternion.W;
            double num6 = quaternion.Z * quaternion.X;
            double num7 = quaternion.Y * quaternion.W;
            double num8 = quaternion.Y * quaternion.Z;
            double num9 = quaternion.X * quaternion.W;
            result.M11 = (float)(1.0 - 2.0 * (num2 + num3));
            result.M12 = (float)(2.0 * (num4 + num5));
            result.M13 = (float)(2.0 * (num6 - num7));
            result.M14 = 0.0f;
            result.M21 = (float)(2.0 * (num4 - num5));
            result.M22 = (float)(1.0 - 2.0 * (num3 + num1));
            result.M23 = (float)(2.0 * (num8 + num9));
            result.M24 = 0.0f;
            result.M31 = (float)(2.0 * (num6 + num7));
            result.M32 = (float)(2.0 * (num8 - num9));
            result.M33 = (float)(1.0 - 2.0 * (num2 + num1));
            result.M34 = 0.0f;
            result.M41 = 0.0f;
            result.M42 = 0.0f;
            result.M43 = 0.0f;
            result.M44 = 1f;
        }

        //Not tested!
        public static void Slerp(ref Matrix matrix1, ref Matrix matrix2, float amount, out Matrix result)
        {
            Quaternion a, b, c;
            Quaternion_T.CreateFromRotationMatrix(ref matrix1, out a);
            Quaternion_T.CreateFromRotationMatrix(ref matrix2, out b);

            Quaternion.Slerp(ref a, ref b, amount, out c);
            CreateFromQuaternion(ref c, out result);

            // Interpolate position
            result.M41 = matrix1.M41 + (matrix2.M41 - matrix1.M41) * amount;
            result.M42 = matrix1.M42 + (matrix2.M42 - matrix1.M42) * amount;
            result.M43 = matrix1.M43 + (matrix2.M43 - matrix1.M43) * amount;
        }

        /// <summary>
        /// Prforms spherical linear interpolation of position and rotation.
        /// </summary>
        public static Matrix Slerp(Matrix matrix1, Matrix matrix2, float amount)
        {
            Matrix result;
            Slerp(ref matrix1, ref matrix2, amount, out result);
            return result;
        }
    }
}
