using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPresenterMath
{
    //This class will represent BoundingFrustum. It will temporary store extension methods for matrix since we cannot derive it from SharpDX.BoundingFrustum
    //TODO: All return types must be BoundingFrustumD (not BoundingFrustum)
    //TODO: All math must be performed in doubles.
    public static class BoundingFrustumExtensions
    {

        public static unsafe void GetCornersUnsafe(this BoundingFrustum frustum, Vector3* corners)
        {
            Vector3[] frustumCorners = frustum.GetCorners();
            corners[0] = frustumCorners[0];
            corners[1] = frustumCorners[1];
            corners[2] = frustumCorners[2];
            corners[3] = frustumCorners[3];
            corners[4] = frustumCorners[4];
            corners[5] = frustumCorners[5];
            corners[6] = frustumCorners[6];
            corners[7] = frustumCorners[7];
        }
    }
}
