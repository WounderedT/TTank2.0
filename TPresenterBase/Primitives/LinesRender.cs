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
    static class LinesRender
    {
        internal static List<VertexFormatPositionColor> pointsList = new List<VertexFormatPositionColor>();

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
            vertexShader = MyShaders.CreateVertexShader("Primitives/Line.hlsl");
            pixelShader = MyShaders.CreatePixelShader("Primitives/Line.hlsl");
            inputLayout = MyShaders.CreateInputLayout(vertexShader.Bytecode, MyVertexLayouts.GetLayout(VertexInputComponentType.POSITION3, VertexInputComponentType.COLOR_4));
            vertexBuffer = BufferManager.CreateVertexBuffer("LinesRenderer", currentBufferSize, sizeof(VertexFormatPositionColor), usage: ResourceUsage.Dynamic);

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

        internal static void DrawLine(Vector3 from, Vector3 to, Color color)
        {
            pointsList.Add(new VertexFormatPositionColor(from, color));
            pointsList.Add(new VertexFormatPositionColor(to, color));
        }

        internal static void DrawBoundingBox(BoundingBox box, Color color)
        {
            var corners = box.GetCorners();

            pointsList.Add(new VertexFormatPositionColor(corners[0], color));
            pointsList.Add(new VertexFormatPositionColor(corners[1], color));
            pointsList.Add(new VertexFormatPositionColor(corners[1], color));
            pointsList.Add(new VertexFormatPositionColor(corners[2], color));
            pointsList.Add(new VertexFormatPositionColor(corners[2], color));
            pointsList.Add(new VertexFormatPositionColor(corners[3], color));
            pointsList.Add(new VertexFormatPositionColor(corners[0], color));
            pointsList.Add(new VertexFormatPositionColor(corners[3], color));

            pointsList.Add(new VertexFormatPositionColor(corners[4], color));
            pointsList.Add(new VertexFormatPositionColor(corners[5], color));
            pointsList.Add(new VertexFormatPositionColor(corners[5], color));
            pointsList.Add(new VertexFormatPositionColor(corners[6], color));
            pointsList.Add(new VertexFormatPositionColor(corners[6], color));
            pointsList.Add(new VertexFormatPositionColor(corners[7], color));
            pointsList.Add(new VertexFormatPositionColor(corners[4], color));
            pointsList.Add(new VertexFormatPositionColor(corners[7], color));

            pointsList.Add(new VertexFormatPositionColor(corners[0], color));
            pointsList.Add(new VertexFormatPositionColor(corners[4], color));
            pointsList.Add(new VertexFormatPositionColor(corners[1], color));
            pointsList.Add(new VertexFormatPositionColor(corners[5], color));
            pointsList.Add(new VertexFormatPositionColor(corners[2], color));
            pointsList.Add(new VertexFormatPositionColor(corners[6], color));
            pointsList.Add(new VertexFormatPositionColor(corners[3], color));
            pointsList.Add(new VertexFormatPositionColor(corners[7], color));
        }

        internal static void DrawBoundingFrustum(BoundingFrustum frustum, Color color)
        {
            var corners = frustum.GetCorners();
            DrawLine(corners[0], corners[1], color);
            DrawLine(corners[1], corners[2], color);
            DrawLine(corners[2], corners[3], color);
            DrawLine(corners[3], corners[0], color);

            DrawLine(corners[4], corners[5], color);
            DrawLine(corners[5], corners[6], color);
            DrawLine(corners[6], corners[7], color);
            DrawLine(corners[7], corners[4], color);

            DrawLine(corners[0], corners[4], color);
            DrawLine(corners[1], corners[5], color);
            DrawLine(corners[2], corners[6], color);
            DrawLine(corners[3], corners[7], color);
        }

        internal unsafe static void Draw()
        {
            RenderContext.SetScreenViewport();
            RenderContext.SetPrimitiveTopology(PrimitiveTopology.LineList);
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

            if(pointsList.Count > 0)
            {
                mapping = Mapping.MapDiscard(vertexBuffer);
                mapping.WriteAndPosition(pointsList.GetInternalArray(), pointsList.Count);
                mapping.Unmap();
            }

            RenderContext.Draw(pointsList.Count, 0);

            pointsList.Clear();
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

    static class DebugLinesRender
    {
        internal static void AddAxisLinesFull(float scale = 1)
        {
            LinesRender.DrawLine(new Vector3(-1f * scale, 0f, 0f), new Vector3(1f * scale, 0f, 0f), Color.DarkRed);
            LinesRender.DrawLine(new Vector3(0.9f * scale, -0.05f * scale, 0f), new Vector3(1f * scale, 0f, 0f), Color.DarkRed);
            LinesRender.DrawLine(new Vector3(0.9f * scale, 0.05f * scale, 0f), new Vector3(1f * scale, 0f, 0f), Color.DarkRed);

            LinesRender.DrawLine(new Vector3(0f, -1f * scale, 0f), new Vector3(0f, 1f * scale, 0f), Color.DarkGreen);
            LinesRender.DrawLine(new Vector3(-0.05f * scale, 0.9f * scale, 0f), new Vector3(0f, 1f * scale, 0f), Color.DarkGreen);
            LinesRender.DrawLine(new Vector3(0.05f * scale, 0.9f * scale, 0f), new Vector3(0f, 1f * scale, 0f), Color.DarkGreen);

            LinesRender.DrawLine(new Vector3(0f, 0f, -1f * scale), new Vector3(0f, 0f, 1f * scale), Color.DarkBlue);
            LinesRender.DrawLine(new Vector3(0f, -0.05f * scale, 0.9f * scale), new Vector3(0f, 0f, 1f * scale), Color.DarkBlue);
            LinesRender.DrawLine(new Vector3(0f, 0.05f * scale, 0.9f * scale), new Vector3(0f, 0f, 1f * scale), Color.DarkBlue);
        }

        internal static void AddGrid()
        {
            for(int i = -5; i < 6; i++)
            {
                LinesRender.DrawLine(new Vector3(-5f, 0f, i), new Vector3(5f, 0f, i), Color.DarkGray);
                LinesRender.DrawLine(new Vector3(i, 0f, -5f), new Vector3(i, 0f, 5f), Color.DarkGray);
            }
        }
    }
}
