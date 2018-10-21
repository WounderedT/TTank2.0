using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TPresenter.Render.Resources;
using TPresenter.Render.Resources.Buffers;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace TPresenter.Render
{
    internal interface IConstantBuffer : IBuffer { }

    internal interface IVertexBuffer : IBuffer { }

    internal interface IIndexBuffer : IBuffer
    {
        IndexBufferFormat Format { get; set; }
    }

    internal enum IndexBufferFormat
    {
        UShort = SharpDX.DXGI.Format.R16_UInt,
        UInt = SharpDX.DXGI.Format.R32_UInt,
    }

    //This class should not be static.
    static class BufferManager
    {
        public static IConstantBuffer CreateConstantBuffer(string name, int byteSize, IntPtr? initData = null, ResourceUsage usage = ResourceUsage.Default)
        {
            BufferDescription description = new BufferDescription(
                byteSize,
                usage,
                BindFlags.ConstantBuffer,
                usage == ResourceUsage.Dynamic ? CpuAccessFlags.Write : CpuAccessFlags.None,
                ResourceOptionFlags.None,
                0);
            return CreateInternal<ConstantBuffer>(name, ref description, initData);
        }

        public static IVertexBuffer CreateVertexBuffer(String name, int elements, int byteStride, IntPtr? initData = null, ResourceUsage usage = ResourceUsage.Default, bool isStreamOutput = false)
        {
            BufferDescription description = new BufferDescription(
                elements * byteStride,
                usage,
                BindFlags.VertexBuffer | (isStreamOutput ? BindFlags.StreamOutput : BindFlags.None),
                usage == ResourceUsage.Dynamic ? CpuAccessFlags.Write : CpuAccessFlags.None,
                ResourceOptionFlags.None,
                byteStride);

            return CreateInternal<VertexBuffer>(name, ref description, initData);
        }

        public static IIndexBuffer CreateIndexBuffer(string name, int elements, IntPtr? initData = null, IndexBufferFormat format = IndexBufferFormat.UShort, ResourceUsage usage = ResourceUsage.Default)
        {
            int stride = 0;
            switch (format)
            {
                case IndexBufferFormat.UShort:
                    stride = 2;
                    break;
                case IndexBufferFormat.UInt:
                    stride = 4;
                    break;
            }

            BufferDescription description = new BufferDescription(
                elements * stride,
                usage,
                BindFlags.IndexBuffer,
                usage == ResourceUsage.Dynamic ? CpuAccessFlags.Write : CpuAccessFlags.None,
                ResourceOptionFlags.None,
                stride);

            return CreateInternal<IndexBuffer>(name, ref description, initData, b => b.Format = format);
        }

        static TBuffer CreateInternal<TBuffer>(String name, ref BufferDescription desc, IntPtr? initData, Action<TBuffer> initializer = null) where TBuffer : MyBuffer, new()
        {
            TBuffer buffer = new TBuffer();

            initializer?.Invoke(buffer);

            buffer.Init(name, ref desc, initData);
            return buffer;
        }
    }
}
