using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPresenterMath
{
    public static class MathHelper
    {
        public const float TwoPi = 2 * (float)Math.PI;

        /// <summary>
        /// Restrict a value to be within a specified range.
        /// </summary>
        /// <param name="value">The value to clamp</param>
        /// <param name="min">Minimum value. Returned if specified value is less than min.</param>
        /// <param name="max">Maximum value. Returned if specified value is greater than max.</param>
        /// <returns></returns>
        public static float Clamp(float value, float min, float max)
        {
            if (value > max)
                value = max;
            else if (value < min)
                value = min;

            return value;
        }

        public static double Clamp(double value, double min, double max)
        {
            if (value > max)
                value = max;
            else if (value < min)
                value = min;

            return value;
        }

        /// <summary>
        /// Converts radians to degrees.
        /// </summary>
        /// <param name="radians">The angle in radians.</param>
        public static float ToDegrees(float radians)
        {
            return radians * 57.29578f;
        }

        public static double ToDegrees(double radians)
        {
            return radians * 57.29578;
        }

        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns>The angle in degrees to convert.</returns>
        public static float ToRadians(float degrees)
        {
            return (degrees / 360.0f) * TwoPi;
        }

        /// <summary>
        /// Perform linearly interpolation of two values.
        /// </summary>
        /// <param name="value1">Source value 1.</param>
        /// <param name="value2">Source value 2.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of value2.</param>
        /// <returns></returns>
        public static float Lerp(float value1, float value2, float amount)
        {
            return value1 + (value2 - value1) * amount;
        }

        /// <summary>
        /// Perform linearly interpolation of two values.
        /// </summary>
        /// <param name="value1">Source value 1.</param>
        /// <param name="value2">Source value 2.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of value2.</param>
        /// <returns></returns>
        public static double Lerp(double value1, double value2, double amount)
        {
            return value1 + (value2 - value1) * amount;
        }

        /// <summary>
        /// Perform linearly interpolation of two vectors.
        /// </summary>
        /// <param name="value1">Source vector 1.</param>
        /// <param name="value2">Source vector 2.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of value2.</param>
        /// <returns></returns>
        public static Vector3 Lerp(Vector3 value1, Vector3 value2, float amount)
        {
            return value1 + (value2 - value1) * amount;
        }

        /// <summary>
        /// Returns angle in range [0, 2*PI]
        /// </summary>
        /// <param name="angle"></param>
        public static void LimitRadians2PI(ref double angle)
        {
            if (angle > TwoPi)
                angle = angle % TwoPi;
            else if (angle < 0)
                angle = angle % TwoPi + TwoPi;
        }

        /// <summary>
        /// Interpolates between zero and one by using cubix equation solved by de Casteljau. (http://www.malinc.se/m/DeCasteljauAndBezier.php)
        /// </summary>
        /// <param name="amount">Weighting value [0, 1].</param>
        public static float SmoothStepStable(float amount)
        {
            Debug.Assert(amount >= 0f && amount <= 1f, "Amout value for SmoothStep must be in [0, 1] range.");
            float invAmount = 1 - amount;
            float y23 = amount;
            float y123 = y23 * amount;
            float y234 = y23 * invAmount + amount;
            float y1234 = y123 * invAmount + y234 * amount;
            return y1234;
        }
    }
}
