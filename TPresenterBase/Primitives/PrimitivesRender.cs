using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPresenter.Filesystem;
using TPresenter.Render.Resources;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace TPresenter.Render
{
    static class PrimitivesRender
    {
        internal static List<VertexFormatPositionColor> vertexList = new List<VertexFormatPositionColor>();

        static int currentBufferSize = 100000;

        static VertexShaderId vertexShader;
        static PixelShaderId pixelShader;
        static IVertexBuffer vertexBuffer;
        static RasterizerState rasterizerState;
        static DepthStencilState depthStencilState;
        static BlendState blendState;

        static MyRenderContext RenderContext { get { return Render11.Direct3DContext; } }

        static InputLayout inputLayout;

        internal unsafe static void Init()
        {
            vertexShader = MyShaders.CreateVertexShader("Primitives\\Primitives.hlsl");
            pixelShader = MyShaders.CreatePixelShader("Primitives\\Primitives.hlsl");
            inputLayout = MyShaders.CreateInputLayout(vertexShader.Bytecode, MyVertexLayouts.GetLayout(VertexInputComponentType.POSITION3, VertexInputComponentType.COLOR_4));
            vertexBuffer = BufferManager.CreateVertexBuffer("PrimitivesRender", currentBufferSize, sizeof(VertexFormatPositionColor), usage: ResourceUsage.Dynamic);

            CreateRasterizerState();
            CreateDepthStencilState();
            CreateBlendState();
        }

        internal static void Dispose()
        {
            rasterizerState.Dispose();
            depthStencilState.Dispose();
            blendState.Dispose();
            inputLayout.Dispose();
        }

        internal static void DrawTriangle(Vector3 v0, Vector3 v1, Vector3 v2, Color color)
        {
            vertexList.Add(new VertexFormatPositionColor(v0, color));
            vertexList.Add(new VertexFormatPositionColor(v1, color));
            vertexList.Add(new VertexFormatPositionColor(v2, color));
        }

        internal static void DrawTriangle(Vector3 v0, Vector3 v1, Vector3 v2, Color c0, Color c1, Color c2)
        {
            vertexList.Add(new VertexFormatPositionColor(v0, c0));
            vertexList.Add(new VertexFormatPositionColor(v1, c1));
            vertexList.Add(new VertexFormatPositionColor(v2, c2));
        }

        internal static void DrawQuadClockWise(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, Color color)
        {
            DrawTriangle(v0, v1, v2, color);
            DrawTriangle(v1, v3, v2, color);
        }

        internal static void DrawQuadClockWise(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, Color c0, Color c1, Color c2, Color c3)
        {
            DrawTriangle(v0, v1, v2, c0, c1, c2);
            DrawTriangle(v1, v3, v2, c1, c3, c2);
        }

        internal static void DrawBoundingBox(BoundingBox boundingBox, Color color)
        {
            var corners = boundingBox.GetCorners();
            DrawTriangle(corners[0], corners[1], corners[2], color);
            DrawTriangle(corners[2], corners[3], corners[0], color);

            DrawTriangle(corners[6], corners[2], corners[1], color);
            DrawTriangle(corners[1], corners[5], corners[6], color);

            DrawTriangle(corners[7], corners[6], corners[5], color);
            DrawTriangle(corners[5], corners[4], corners[7], color);

            DrawTriangle(corners[3], corners[7], corners[4], color);
            DrawTriangle(corners[4], corners[0], corners[3], color);

            DrawTriangle(corners[5], corners[1], corners[0], color);
            DrawTriangle(corners[0], corners[4], corners[5], color);

            DrawTriangle(corners[6], corners[7], corners[3], color);
            DrawTriangle(corners[3], corners[2], corners[6], color);
        }

        internal unsafe static void Draw()
        {
            RenderContext.SetScreenViewport();
            RenderContext.SetPrimitiveTopology(PrimitiveTopology.TriangleList);
            RenderContext.SetInputLayout(inputLayout);

            RenderContext.SetRasterizerState(rasterizerState);
            RenderContext.SetDepthStencilState(depthStencilState);

            RenderContext.VertexShader.Set(vertexShader);
            RenderContext.PixelShader.Set(pixelShader);

            RenderContext.SetConstantBuffer(MyCommon.PROJECTION_SLOT, MyCommon.ProjectionConstants);

            RenderContext.SetRenderTargetView(Render11.DepthStencilView, Render11.RenderTargetView);

            RenderContext.SetBlendState(blendState);

            var transpose = Matrix.Transpose(Render11.Environment.Matrices.ViewProjection);
            var mapping = Mapping.MapDiscard(MyCommon.ProjectionConstants);
            mapping.WriteAndPosition(ref transpose);
            mapping.Unmap();

            RenderContext.SetVertexBuffer(0, vertexBuffer);

            if(vertexList.Count > 0)
            {
                mapping = Mapping.MapDiscard(vertexBuffer);
                mapping.WriteAndPosition(vertexList.GetInternalArray(), vertexList.Count);
                mapping.Unmap();
            }

            RenderContext.Draw(vertexList.Count, 0);

            vertexList.Clear();
        }

        static void CreateRasterizerState()
        {
            RasterizerStateDescription descr = new RasterizerStateDescription();
            descr.FillMode = FillMode.Solid;
            descr.CullMode = CullMode.Front;
            descr.IsFrontCounterClockwise = true;
            rasterizerState = new RasterizerState(Render11.Direct3DDevice, descr);
        }

        static void CreateDepthStencilState()
        {
            depthStencilState = new DepthStencilState(Render11.Direct3DDevice,
                new DepthStencilStateDescription()
                {
                    IsDepthEnabled = true,              //enable depth
                    DepthComparison = Comparison.Less,
                    DepthWriteMask = DepthWriteMask.All,
                    IsStencilEnabled = false,           //enable stencil
                    StencilReadMask = 0xff,             //0xff - no mask
                    StencilWriteMask = 0xff,            //0xff - no mask
                    FrontFace = new DepthStencilOperationDescription()  //Configure FrontFace depth/stencil operations
                    {
                        Comparison = Comparison.Always,
                        PassOperation = StencilOperation.Keep,
                        FailOperation = StencilOperation.Keep,
                        DepthFailOperation = StencilOperation.Increment,
                    },
                    BackFace = new DepthStencilOperationDescription()   //Configure BackFace depth/stencil operations
                    {
                        Comparison = Comparison.Always,
                        PassOperation = StencilOperation.Keep,
                        FailOperation = StencilOperation.Keep,
                        DepthFailOperation = StencilOperation.Decrement,
                    },
                });
        }

        static void CreateBlendState()
        {
            BlendStateDescription desc = new BlendStateDescription();
            desc.RenderTarget[0].IsBlendEnabled = true;
            desc.RenderTarget[0].RenderTargetWriteMask = ColorWriteMaskFlags.All;
            desc.RenderTarget[0].BlendOperation = BlendOperation.Add;
            desc.RenderTarget[0].AlphaBlendOperation = BlendOperation.Add;
            desc.RenderTarget[0].DestinationBlend = BlendOption.InverseSourceAlpha;
            desc.RenderTarget[0].DestinationAlphaBlend = BlendOption.InverseSourceAlpha;
            desc.RenderTarget[0].SourceBlend = BlendOption.SourceAlpha;
            desc.RenderTarget[0].SourceAlphaBlend = BlendOption.SourceAlpha;
            blendState = new BlendState(Render11.Direct3DDevice, desc);
        }
    }
}
