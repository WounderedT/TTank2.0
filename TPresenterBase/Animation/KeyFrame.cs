using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPresenter.Render.Animation
{
    public enum TransformationType
    {
        LINEAR
    }

    public class KeyFrame
    {
        public double timestamp;
        public float scaling;
        public Quaternion rotation;
        public Vector3 position;
        public TransformationType TransformationType;

        public Matrix Transoformation
        {
            get
            {
                return Matrix.Scaling(scaling) * Matrix.RotationQuaternion(rotation) * Matrix.Translation(position);
            }
        }

        public KeyFrame() { }

        public KeyFrame(Matrix transformation, String transformationType)
        {
            transformation.DecomposeUniformScale(out scaling, out rotation, out position);
            TransformationType = (TransformationType)TransformationType.Parse(typeof(TransformationType), transformationType);
        }
    }
}
