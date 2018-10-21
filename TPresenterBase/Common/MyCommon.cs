using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPresenter.Render.Resources;

namespace TPresenter.Render
{
    static class MyCommon
    {
        //constant buffers
        internal const int FRAME_SLOT = 0;
        internal const int PROJECTION_SLOT = 1;
        internal const int OBJECT_SLOT = 2;
        internal const int MATERIALS_SLOT = 3;
        internal const int FOLIAGE_SLOT = 4;
        internal const int ALPHAMASK_VIEWS_SLOT = 5;
        internal const int SKELETON_SLOT = 6;

        internal static IConstantBuffer ProjectionConstants { get; set; }

        // folowing buffers could be moved to another class.
        internal static IConstantBuffer PerObjectConstants { get; set; }
        internal static IConstantBuffer PerFrameConstants { get; set; }
        internal static IConstantBuffer PerMaterialConstants { get; set; }
        internal static IConstantBuffer PerSkeletonConstants { get; set; }

        static Dictionary<int, IConstantBuffer> constantBuffers = new Dictionary<int, IConstantBuffer>();

        internal static unsafe void Init()
        {
            ProjectionConstants = BufferManager.CreateConstantBuffer("ProjectionConstants", sizeof(Matrix), usage: ResourceUsage.Dynamic);
            PerObjectConstants = BufferManager.CreateConstantBuffer("PerObjectConstants", sizeof(PerObject), usage: ResourceUsage.Dynamic);
            PerFrameConstants = BufferManager.CreateConstantBuffer("PerFrameConstants", sizeof(PerFrame), usage: ResourceUsage.Dynamic);
            PerMaterialConstants = BufferManager.CreateConstantBuffer("PerMaterialConstants", sizeof(PerMaterial), usage: ResourceUsage.Dynamic);
        }

        internal static IConstantBuffer GetObjectConstantBuffer(int size)
        {
            size = ((size + 15) / 16) * 16; // 16-byte allignement.
            if (!constantBuffers.ContainsKey(size))
            {
                constantBuffers[size] = BufferManager.CreateConstantBuffer("ConstantObjectBuffer-" + size, size, usage: ResourceUsage.Dynamic);
            }
            return constantBuffers[size];
        }
    }
}
