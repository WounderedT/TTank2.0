using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;
using Resource = SharpDX.Direct3D11.Resource;

namespace TPresenter.Render.Resources.Buffers
{
    abstract class MyBuffer : IBuffer
    {
        #region Fields And Properties

        int elementCount;
        bool isDisposed = false;

        Buffer buffer;
        BufferDescription description;

        public Buffer Buffer { get { return buffer; } }

        public int ButeSize { get { return elementCount * description.StructureByteStride; } }

        public BufferDescription Description { get { return description; } }

        public int ElementCount { get { return elementCount; } }

        public string Name { get { return buffer.DebugName; } }

        public Resource Resource { get { return buffer; } }

        public Vector2 Size { get { return new Vector2(elementCount, 1); } }

        public Vector3 Size3 { get { return new Vector3(elementCount, 1, 1); } }

        #endregion

        internal void Init(string name, ref BufferDescription desc, IntPtr? initData)
        {
            description = desc;
            elementCount = desc.SizeInBytes / Math.Max(1, desc.StructureByteStride);

            try
            {
                 buffer = new Buffer(Render11.Direct3DDevice, initData ?? default(IntPtr), desc)
                 {
                    DebugName = name,
                };
            }
            catch (SharpDXException e)
            {
                //log this exception;
                throw;
            }
        }

        public void Dispose()
        {
            if (isDisposed)
                return;

            elementCount = 0;
            description = default(BufferDescription);
            if(buffer != null)
            {
                buffer.Dispose();
                buffer = null;
            }
        }
    }

    class ConstantBuffer: MyBuffer, IConstantBuffer { }

    class VertexBuffer: MyBuffer, IVertexBuffer { }

    class IndexBuffer : MyBuffer, IIndexBuffer
    {
        public IndexBufferFormat Format { get; set; }
    }
}
