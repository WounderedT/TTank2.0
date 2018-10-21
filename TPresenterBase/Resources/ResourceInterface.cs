using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buffer = SharpDX.Direct3D11.Buffer;
using Resource = SharpDX.Direct3D11.Resource;

namespace TPresenter.Render.Resources
{
    internal interface IResource
    {
        string Name { get; }
        Resource Resource { get; }
        Vector3 Size3 { get; }
        Vector2 Size { get; }
    }

    internal interface IBuffer : IResource, IDisposable
    {
        int ButeSize { get; }
        int ElementCount { get; }

        BufferDescription Description { get; }
        Buffer Buffer { get; }
    }

    internal interface ISrvBindable : IResource
    {
        ShaderResourceView Srv { get; }
    }
}
