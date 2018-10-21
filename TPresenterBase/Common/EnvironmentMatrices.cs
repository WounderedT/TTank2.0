using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPresenter.Render
{
    class EnvironmentMatrices
    {
        internal Vector3 CameraPosition;
        internal Matrix ViewAt0;
        internal Matrix InvViewAt0;
        internal Matrix ViewProjectionAt0;
        internal Matrix InvViewProjectionAt0;

        internal Matrix View;
        internal Matrix InvView;
        internal Matrix Projection;
        internal Matrix InvProjection;
        internal Matrix ViewProjection;
        internal Matrix InvViewProjection;

        internal Matrix WorldViewProjection;

        internal float FovH;
        internal float FovL;

        internal float NearClipping;
        internal float FarClipping;
    }

    class MyEnvironment
    {
        internal EnvironmentMatrices Matrices = new EnvironmentMatrices();
    }
}
