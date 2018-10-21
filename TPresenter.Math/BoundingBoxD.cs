using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPresenterMath
{
    public struct BoundingBoxD
    {
        //This class will represent BoundingBox. It will temporary store extension methods for matrix since we cannot derive it from SharpDX.BoundingBox
        //TODO: All return types must be BoundingBoxD (not BoundingBox)
        //TODO: All math must be performed in doubles.
        public static BoundingBox CreateInvalid()
        {
            return new BoundingBox(new Vector3((float)double.MaxValue), new Vector3((float)double.MinValue));
        }
    }

    public static class BoundingBoxExtensions
    {
        public static unsafe BoundingBox Include(this BoundingBox box, ref BoundingFrustum frustum)
        {
            Vector3* temporaryCorners = stackalloc Vector3[8];
            frustum.GetCornersUnsafe(temporaryCorners);

            box.Include(ref temporaryCorners[0]);
            box.Include(ref temporaryCorners[1]);
            box.Include(ref temporaryCorners[2]);
            box.Include(ref temporaryCorners[3]);
            box.Include(ref temporaryCorners[4]);
            box.Include(ref temporaryCorners[5]);
            box.Include(ref temporaryCorners[6]);
            box.Include(ref temporaryCorners[7]);

            return box;
        }

        public static BoundingBox Include(this BoundingBox box, ref Vector3 point)
        {
            box.Minimum.X = Math.Min(point.X, box.Minimum.X);
            box.Minimum.Y = Math.Min(point.Y, box.Minimum.Y);
            box.Minimum.Z = Math.Min(point.Z, box.Minimum.Z);

            box.Maximum.X = Math.Max(point.X, box.Maximum.X);
            box.Maximum.Y = Math.Max(point.Y, box.Maximum.Y);
            box.Maximum.Z = Math.Max(point.Z, box.Maximum.Z);

            return box;
        }
    }
}
