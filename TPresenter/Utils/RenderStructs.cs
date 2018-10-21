using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPresenterMath;

namespace TPresenter.Utils
{
    public struct TriangleNormals
    {
        public Vector3 Normal0;
        public Vector3 Normal1;
        public Vector3 Normal2;
    }

    public struct TriagleVertices
    {
        public Vector3 Vertex0;
        public Vector3 Vertex1;
        public Vector3 Vertex2;

        public void Transform(ref Matrix transform)
        {
            Vertex0 = Vector3_T.Transform(Vertex0, ref transform);
            Vertex1 = Vector3_T.Transform(Vertex1, ref transform);
            Vertex2 = Vector3_T.Transform(Vertex2, ref transform);
        }
    }
}
