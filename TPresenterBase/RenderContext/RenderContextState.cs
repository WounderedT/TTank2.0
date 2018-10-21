using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace TPresenter.Render
{
    class RenderContextState
    {
        #region Fields

        BlendState blendState;
        DeviceContext deviceContext;
        DepthStencilState depthStencilState;
        int stencilRef;
        ViewportF viewport;
        PrimitiveTopology primitiveTopology;
        InputLayout inputLayout;
        RasterizerState rasterizerState;

        IIndexBuffer indexBuffer;
        IndexBufferFormat indexBufferFormat;
        int indexBufferOffset;

        readonly IVertexBuffer[] vertexBuffers = new IVertexBuffer[8];
        readonly int[] vertexBuffersStrides = new int[8];

        #endregion

        internal void Init(DeviceContext deviceContext)
        {
            this.deviceContext = deviceContext;
        }

        internal void Clear()
        {
            deviceContext.ClearState();
            viewport = default(ViewportF);
        }

        internal void SetBlendState(BlendState bs)
        {
            if (blendState == bs)
                return;

            blendState = bs;
            deviceContext.OutputMerger.SetBlendState(blendState);
        }

        internal void SetDepthStencilState(DepthStencilState dss, int stencilRef)
        {
            if (depthStencilState == dss && this.stencilRef == stencilRef)
                return;

            depthStencilState = dss;
            this.stencilRef = stencilRef;

            deviceContext.OutputMerger.SetDepthStencilState(depthStencilState, this.stencilRef);
        }

        internal void SetInputLayout(InputLayout il)
        {
            if (inputLayout == il)
                return;

            inputLayout = il;
            deviceContext.InputAssembler.InputLayout = il;
        }

        internal void SetPrimitiveTopology(PrimitiveTopology pt)
        {
            if (primitiveTopology == pt)
                return;
            primitiveTopology = pt;
            deviceContext.InputAssembler.PrimitiveTopology = pt;
        }

        internal void SetRasterizerState(RasterizerState rs)
        {
            if (rasterizerState == rs)
                return;

            rasterizerState = rs;
            deviceContext.Rasterizer.State = rasterizerState;
        }

        internal void SetTargets(DepthStencilView dsv, RenderTargetView rtv)
        {
            deviceContext.OutputMerger.SetTargets(dsv, rtv);
        }

        internal void SetIndexBuffer(IIndexBuffer buffer, IndexBufferFormat format, int offset)
        {
            if (buffer == indexBuffer && format == indexBufferFormat && offset == indexBufferOffset)
                return;

            indexBuffer = buffer;
            indexBufferFormat = format;
            indexBufferOffset = offset;
            deviceContext.InputAssembler.SetIndexBuffer(buffer.Buffer, (Format)format, offset);
        }

        internal void SetVertexBuffer(int startSlot, IVertexBuffer vertexBuffer, int stride)
        {
            Debug.Assert(startSlot < vertexBuffers.Length);

            if (vertexBuffers[startSlot] != null && vertexBuffers[startSlot] == vertexBuffer && vertexBuffersStrides[startSlot] == stride)
                return;

            vertexBuffers[startSlot] = vertexBuffer;
            vertexBuffersStrides[startSlot] = stride;

            deviceContext.InputAssembler.SetVertexBuffers(startSlot, new VertexBufferBinding(vertexBuffer != null ? vertexBuffer.Buffer : null, stride, 0));
        }

        internal void SetVertexBuffers(int startSlot, IVertexBuffer[] vertexBuffers, int[] strides)
        {
            Debug.Assert(startSlot + vertexBuffers.Length < vertexBuffers.Length);

            for (int i = startSlot; i < startSlot + vertexBuffers.Length; i++)
                SetVertexBuffer(i, vertexBuffers[i], strides[i]);
        }

        internal void SetViewport(ViewportF viewport)
        {
            if(viewport.X != this.viewport.X || viewport.Y != this.viewport.Y 
                || viewport.Height != this.viewport.Height || viewport.Width != this.viewport.Width
                || viewport.MinDepth != this.viewport.MinDepth || viewport.MaxDepth != this.viewport.MaxDepth)
            {
                this.viewport = viewport;
                deviceContext.Rasterizer.SetViewport(viewport);
            }
        }
    }
}
