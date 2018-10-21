using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPresenter.Render.Resources;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace TPresenter.Render
{
    internal class MyRenderContext: IDisposable
    {
        #region Fields And Properties

        DeviceContext deviceContext;
        PixelShaderStage pixelShaderStage;
        VertexShaderStage vertexShaderStage;
        RenderContextState state = new RenderContextState();
        bool isDisposed = false;

        internal PixelShaderStage PixelShader
        {
            get { return pixelShaderStage; }
        }

        internal VertexShaderStage VertexShader
        {
            get { return vertexShaderStage; }
        }

        #endregion

        #region Initialize And Dispose

        internal void Initialize(DeviceContext context)
        {
            Debug.Assert(this.deviceContext == null, "Initialize called on already initialized object");
            deviceContext = context;

            vertexShaderStage = new VertexShaderStage(deviceContext.NativePointer);
            pixelShaderStage = new PixelShaderStage(deviceContext.NativePointer);

            state.Init(deviceContext);
        }

        public void Dispose()
        {
            pixelShaderStage = null;
            vertexShaderStage = null;
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed)
                return;
            if (disposing)
            {
                deviceContext.Dispose();
            }
            isDisposed = true;
        }

        #endregion

        internal bool IsInitialized()
        {
            return deviceContext != null ? true : false;
        }

        internal void ClearDepthStencilView(DepthStencilView dsv, DepthStencilClearFlags flags, float depth, byte stencil)
        {
            deviceContext.ClearDepthStencilView(dsv, flags, depth, stencil);
        }

        internal void ClearRenderTargetView(RenderTargetView rtv, Color color)
        {
            deviceContext.ClearRenderTargetView(rtv, color);
        }

        internal void ClearState()
        {
            state.Clear();
        }

        internal void Draw(int vertexCount, int startVertexLocation)
        {
            deviceContext.Draw(vertexCount, startVertexLocation);
        }

        internal void Draw(int vertexCount, int startIndexLocation, int startVertexLocation)
        {
            deviceContext.DrawIndexed(vertexCount, startIndexLocation, startIndexLocation);
        }

        internal void DrawAuto()
        {
            deviceContext.DrawAuto();
        }

        internal DataBox MapSubresource(IResource resourceRef, int subresource, MapMode mapType, MapFlags mapFlags)
        {
            return deviceContext.MapSubresource(resourceRef.Resource, subresource, mapType, mapFlags);
        }

        internal void SetBlendState(BlendState bs)
        {
            state.SetBlendState(bs);
        }

        internal void SetConstantBuffer(int slot, IConstantBuffer buffer)
        {
            deviceContext.VertexShader.SetConstantBuffer(slot, buffer.Buffer);
            deviceContext.PixelShader.SetConstantBuffer(slot, buffer.Buffer);
        }

        internal void SetDepthStencilState(DepthStencilState dss, int stencilRef = 0)
        {
            //DepthStencilStateDescription desc = new DepthStencilStateDescription();
            //desc.DepthComparison = Render11.UseComplementaryDepthBuffer ? Comparison.Greater : Comparison.Less;
            //desc.DepthWriteMask = DepthWriteMask.All;
            //desc.IsDepthEnabled = true;
            //desc.IsStencilEnabled = false;
            //DepthStencilState dss = new DepthStencilState(Render11.Direct3DDevice, desc);

            state.SetDepthStencilState(dss, stencilRef);
        }

        internal void SetInputLayout(InputLayout il)
        {
            state.SetInputLayout(il);
        }

        internal void SetPrimitiveTopology(PrimitiveTopology pt)
        {
            state.SetPrimitiveTopology(pt);
        }

        internal void SetRasterizerState(RasterizerState rState)
        {
            state.SetRasterizerState(rState);
        }

        internal void SetRenderTargetView(DepthStencilView dsv, RenderTargetView rtv)
        {
            state.SetTargets(dsv, rtv);
        }

        internal void SetScreenViewport()
        {
            SetViewport(new ViewportF(0, 0, Render11.CurrentBounds.Width, Render11.CurrentBounds.Height));
        }

        internal void SetIndexBuffer(IIndexBuffer indexBuffer, int offset = 0)
        {
            state.SetIndexBuffer(indexBuffer, indexBuffer != null ? indexBuffer.Format : 0, offset);
        }

        internal void SetVertexBuffer(int slot, IVertexBuffer vertexBuffer, int stride = -1)
        {
            if (vertexBuffer != null && stride < 0)
                stride = vertexBuffer.Description.StructureByteStride;

            state.SetVertexBuffer(slot, vertexBuffer, stride);
        }

        internal void SetVertexBuffers(int slot, IVertexBuffer[] vertexBuffers, int[] strides = null)
        {
            strides = strides ?? vertexBuffers.Select(vb => vb != null ? vb.Description.StructureByteStride : -1).ToArray();

            state.SetVertexBuffers(slot, vertexBuffers, strides);
        }
        
        internal void SetViewport(ViewportF viewport)
        {
            state.SetViewport(viewport);
        }

        internal void UpdateSubresource<T>(ref T data, Resource resource)where T : struct
        {
            deviceContext.UpdateSubresource(ref data, resource);
        }

        internal void UnmapSubresource(Resource resourceRef, int subresource)
        {
            deviceContext.UnmapSubresource(resourceRef, subresource);
        }
    }
}
