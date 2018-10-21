using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPresenterMath;

namespace TPresenter.Utils
{
    public static class MyUtils
    {
        #region Utils Fields and Properties

        [ThreadStatic]
        static Random random;

        static Random Random
        {
            get
            {
                if(random == null)
                {
                    random = new Random();
                }
                return random;
            }
        } 

        #endregion

        #region Utils Assetions

        [Conditional("DEBUG")]
        public static void AssertLenghtIsValid(ref Vector3 vector)
        {
            Debug.Assert(vector.Length() > MathConstants.EPSILON10);
        }

        [Conditional("DEBUG")]
        public static void AssertIsValid(Matrix matrix)
        {
            Debug.Assert(matrix.Up.IsValid() && matrix.Left.IsValid() && matrix.Forward.IsValid() && matrix.TranslationVector.IsValid() && matrix != Matrix.Zero);
        }

        #endregion

        #region Utils Checks

        public static bool IsZero(float value, float epsilon = MathConstants.EPSILON10)
        {
            return (value > -epsilon) && (value < epsilon);
        }

        #endregion

        #region Utils Vector3

        public static Vector3 Normilize(Vector3 vector)
        {
            AssertLenghtIsValid(ref vector);
            return Vector3.Normalize(vector);
        }

        //TODO: all math must be done in double.
        /// <summary>
        /// Performs vector rotation around (xVector x yVector)-axis using Rodrigues' rotation formula.
        /// </summary>
        public static void VectorPlaneRotation(Vector3 xVector, Vector3 yVector, float angle, out Vector3 xOut, out Vector3 yOut)
        {
            //xOut = Vector3D.Multiply(xVector, Math.Cos(angle)) + Vector3D.Multiply(yVector, Math.Sin(angle));
            //yOut = Vector3D.Multiply(xVector, -Math.Sin(angle)) + Vector3D.Multiply(yVector, Math.Cos(angle));
            xOut = xVector * (float)Math.Cos(angle) + yVector * (float)Math.Sin(angle);
            yOut = xVector * (float)Math.Cos(angle + Math.PI / 2.0) + yVector * (float)Math.Sin(angle + Math.PI / 2.0);
        }

        public static BoundingSphere GetBoundingSphereFromBoundingBox(ref BoundingBox box)
        {
            BoundingSphere sphere;
            sphere.Center = Vector3D.Divide((box.Maximum + box.Minimum), 2.0);
            sphere.Radius = Vector3.Distance(sphere.Center, box.Maximum);
            return sphere;
        }

        #endregion

        #region Utils Random

        public static float GetRandomSign()
        {
            return Math.Sign((float)Random.NextDouble() - 0.5f);
        }

        #endregion

    }
}
