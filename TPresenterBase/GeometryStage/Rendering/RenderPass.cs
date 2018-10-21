using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPresenter.Filesystem;
using TPresenter.Profiler;
using TPresenter.Render.GeometryStage.Model;
using TPresenter.Render.Resources;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace TPresenter.Render.GeometryStage.Rendering
{
    class RenderPass
    {
        internal static List<VertexFormatPositionTextureNormal> vertexList = new List<VertexFormatPositionTextureNormal>();

        static int currentBufferSize = 100000;

        static VertexShaderId vertexShader;
        static PixelShaderId pixelShader;
        static IVertexBuffer vertexBuffer;

        static ShaderResourceView textureView;
        static SamplerState samplerState;
        static RasterizerState rasterizerState;
        static DepthStencilState depthStencilState;
        static BlendState blendState;

        static MyRenderContext RenderContext { get { return Render11.Direct3DContext; } }

        static InputLayout inputLayout;

        Dictionary<StringId, DrawableGroup> drawableGroups = new Dictionary<StringId, DrawableGroup>();

        internal unsafe void Init()
        {
            //vertexShader = MyShaders.CreateVertexShader("Geometry\\VertexShader.hlsl");
            ////pixelShader = MyShaders.CreatePixelShader("Geometry\\SimplePixelShader.hlsl", "pixel_shader");
            ////pixelShader = MyShaders.CreatePixelShader("Geometry\\Diffuse.hlsl", "diffuse_shader");
            ////pixelShader = MyShaders.CreatePixelShader("Geometry\\Phong.hlsl", "phong_shader");
            //pixelShader = MyShaders.CreatePixelShader("Geometry\\BlinnPhong.hlsl");
            //inputLayout = MyShaders.CreateInputLayout(
            //    vertexShader.Bytecode,
            //    MyVertexLayouts.GetLayout(
            //        VertexInputComponentType.POSITION3,
            //        VertexInputComponentType.NORMAL,
            //        VertexInputComponentType.COLOR_4,
            //        VertexInputComponentType.TEXCOORD0,
            //        VertexInputComponentType.BLEND_INDICES,
            //        VertexInputComponentType.BLEND_WEIGHTS
            //        ));
            //vertexBuffer = BufferManager.CreateVertexBuffer("RenderPass", currentBufferSize, sizeof(VertexFormatPositionTextureNormal), usage: ResourceUsage.Dynamic);

            // This should not be done here. Move to appropriate class asap.
            //Resource texture = MyTexture.GetTextureFromFile(System.IO.Path.Combine(FileProvider.ContentPath, "Textures\\Texture2.png"));
            //textureView = new ShaderResourceView(Render11.Direct3DDevice, texture);
            //samplerState = new SamplerState(Render11.Direct3DDevice, new SamplerStateDescription()
            //{
            //    AddressU = TextureAddressMode.Wrap,
            //    AddressV = TextureAddressMode.Wrap,
            //    AddressW = TextureAddressMode.Wrap,
            //    Filter = Filter.MinMagMipLinear,
            //});

            samplerState = new SamplerState(Render11.Direct3DDevice, new SamplerStateDescription()
            {
                AddressU = TextureAddressMode.Clamp,
                AddressV = TextureAddressMode.Clamp,
                AddressW = TextureAddressMode.Clamp,
                BorderColor = new Color4(0, 0, 0, 0),
                ComparisonFunction = Comparison.Never,
                Filter = Filter.MinMagMipLinear,
                MaximumAnisotropy = 16,
                MaximumLod = float.MaxValue,
                MinimumLod = 0,
                MipLodBias = 0.0f,
            });

            CreateRasterizerState();
            CreateDepthStencilState();
            CreateBlendState();
        }

        internal void Dispose()
        {
            //textureView.Dispose();
            samplerState.Dispose();
            rasterizerState.Dispose();
            depthStencilState.Dispose();
            blendState.Dispose();
            //inputLayout.Dispose();
        }

        internal void DrawSphere()
        {
            VertexFormatPositionTextureNormal[] vertices;
            int[] indices;
            GenerateSphere(out vertices, out indices);
            for(int i = 0; i < indices.Length; i++)
            {
                vertexList.Add(vertices[indices[i]]);
            }
        }

        internal unsafe void Draw(List<InstanceComponent> visibleInstances)
        {
            ProfilerStatic.BeginSubstep("Preparations");
            RenderContext.SetScreenViewport();
            RenderContext.SetPrimitiveTopology(PrimitiveTopology.TriangleList);

            RenderContext.SetRasterizerState(rasterizerState);
            RenderContext.SetDepthStencilState(depthStencilState);

            //RenderContext.PixelShader.SetShaderResource(0, textureView);
            //RenderContext.PixelShader.SetSampler(0, samplerState);

            //RenderContext.SetConstantBuffer(MyCommon.PROJECTION_SLOT, MyCommon.ProjectionConstants);
            RenderContext.SetConstantBuffer(MyCommon.FRAME_SLOT, MyCommon.PerObjectConstants);
            RenderContext.SetConstantBuffer(MyCommon.PROJECTION_SLOT, MyCommon.PerFrameConstants);
            RenderContext.SetConstantBuffer(MyCommon.MATERIALS_SLOT, MyCommon.PerMaterialConstants);

            var skeletonBuffer = MyCommon.GetObjectConstantBuffer(sizeof(Matrix) * 256);
            RenderContext.SetConstantBuffer(MyCommon.SKELETON_SLOT, skeletonBuffer);

            RenderContext.SetRenderTargetView(Render11.DepthStencilView, Render11.RenderTargetView);

            RenderContext.SetBlendState(blendState);

            var perFrame = new PerFrame();
            perFrame.cameraPosition = Render11.Environment.Matrices.CameraPosition;
            perFrame.ViewProj = Matrix.Transpose(Render11.Environment.Matrices.ViewProjection);
            perFrame.light.color = Color.White;
            var lightDir = Vector3.Transform(new Vector3(50f, 50f, -50f), Matrix.Identity);
            perFrame.light.direction = new Vector3(lightDir.X, lightDir.Y, lightDir.Z);
            perFrame.tessellationFactor = 1;
            var mapping = Mapping.MapDiscard(MyCommon.PerFrameConstants);
            mapping.WriteAndPosition(ref perFrame);
            mapping.Unmap();

            PrepareDrawableGroups(visibleInstances);
            ProfilerStatic.EndSubstep();
            foreach (var group in drawableGroups.Values)
            {
                ProfilerStatic.BeginSubstep("Drawing group");
                var perMaterial = new PerMaterial(group.Material);

                //  Bind textures to the pixel shader
                perMaterial.hasTexture = (uint)(group.ResourceViews != null ? 1 : 0);
                RenderContext.PixelShader.SetShaderResources(0, group.ResourceViews.GetInternalArray());

                RenderContext.PixelShader.SetSampler(0, samplerState);
                
                mapping = Mapping.MapDiscard(MyCommon.PerMaterialConstants);
                mapping.WriteAndPosition(ref perMaterial);
                mapping.Unmap();

                ProfilerStatic.BeginSubstep("Drawing all parts", true);
                foreach (var part in group.DrawableParts)
                {
                    var perObject = new PerObject();
                    //perObject.World = part.WorldMatrix;
                    //perObject.InvWorldTranspose = Matrix.Transpose(Matrix.Invert(part.WorldMatrix));
                    
                    //Transpose the matrices so that they are in row major order for HLSL
                    perObject.World = Matrix.Transpose(part.WorldMatrix);
                    perObject.InvWorldTranspose = Matrix.Invert(part.WorldMatrix);

                    mapping = Mapping.MapDiscard(MyCommon.PerObjectConstants);
                    mapping.WriteAndPosition(ref perObject);
                    mapping.Unmap();

                    if (part is SkinnedDrawablePart)
                    {
                        mapping = Mapping.MapDiscard(skeletonBuffer);
                        for (int i = 0; i < ((SkinnedDrawablePart)part).SkinMatrices.Count(); i++)
                            mapping.WriteAndPosition(ref ((SkinnedDrawablePart)part).SkinMatrices[i]);

                        mapping.Unmap();
                    }

                    RenderContext.SetInputLayout(part.ShaderBundle.InputLayout);
                    RenderContext.VertexShader.Set(part.ShaderBundle.VertexShader);
                    RenderContext.PixelShader.Set(part.ShaderBundle.PixelShader);

                    //  Retrive and set the vertex and index buffers
                    RenderContext.SetVertexBuffer(0, part.VertexBuffer);

                    RenderContext.SetIndexBuffer(part.IndexBuffer, 0);

                    RenderContext.SetPrimitiveTopology(PrimitiveTopology.TriangleList);

                    RenderContext.Draw((int)part.VertexCount * 3, (int)part.StartVertex, 0);
                }
                ProfilerStatic.EndSubstep(true);
                ProfilerStatic.EndSubstep();
            }
        }

        void PrepareDrawableGroups(List<InstanceComponent> components)
        {
            drawableGroups.Clear();
            for(int i = 0; i < components.Count; i++)
            {
                var model = components[i].Model;
                if (components[i] is SkinnedInstanceComponent)
                {
                    foreach (var part in model.Parts)
                    {
                        StringId materialName = StringId.GetOrCompute(part.Material.Name);
                        if (!drawableGroups.ContainsKey(materialName))
                            drawableGroups.Add(materialName, new DrawableGroup(part.Material));

                        drawableGroups[materialName].DrawableParts.Add(new SkinnedDrawablePart(part, components[i].GetMatrix(), (components[i] as SkinnedInstanceComponent).SkinMatrices));
                    }
                }
                else
                {
                    foreach (var part in model.Parts)
                    {
                        StringId materialName = StringId.GetOrCompute(part.Material.Name);
                        if (!drawableGroups.ContainsKey(materialName))
                            drawableGroups.Add(materialName, new DrawableGroup(part.Material));

                        drawableGroups[materialName].DrawableParts.Add(new DrawablePart(part, components[i].GetMatrix()));
                    }
                }

                foreach(MyShaderResourceView view in model.texturesViews)
                {
                    //  Only unique ShaderResourceViews must be added. Add appropiate check.
                    drawableGroups[view.MaterialId].ResourceViews.AddArray(view.ResourceView.GetInternalArray());
                }
            }
        }

        //Copy form GeometricPrimitives.cs of Render Tutorial. Should be updated.
        /// <summary>
        /// Creates a sphere primitive.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="diameter">The diameter.</param>
        /// <param name="tessellation">The tessellation.</param>
        /// <param name="toLeftHanded">if set to <c>true</c> vertices and indices will be transformed to left handed. Default is true.</param>
        /// <returns>A sphere primitive.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">tessellation;Must be >= 3</exception>
        internal void GenerateSphere(out VertexFormatPositionTextureNormal[] vertices, out int[] indices, float radius = 0.5f, int tessellation = 16, bool clockWiseWinding = true)
        {
            if (tessellation < 3) throw new ArgumentOutOfRangeException("tessellation", "Must be >= 3");

            int verticalSegments = tessellation;
            int horizontalSegments = tessellation * 2;

            vertices = new VertexFormatPositionTextureNormal[(verticalSegments + 1) * (horizontalSegments + 1)];
            indices = new int[(verticalSegments) * (horizontalSegments + 1) * 6];

            int vertexCount = 0;
            // Create rings of vertices at progressively higher latitudes.
            for (int i = 0; i <= verticalSegments; i++)
            {
                float v = 1.0f - (float)i / verticalSegments;

                var latitude = (float)((i * Math.PI / verticalSegments) + Math.PI / 2.0);
                var dy = (float)Math.Sin(latitude);
                var dxz = (float)Math.Cos(latitude);

                // Create a single ring of vertices at this latitude.
                for (int j = 0; j <= horizontalSegments; j++)
                {
                    float u = (float)j / horizontalSegments;

                    var longitude = (float)(j * 2.0 * Math.PI / horizontalSegments);
                    var dx = (float)Math.Sin(longitude);
                    var dz = (float)Math.Cos(longitude);

                    dx *= dxz;
                    dz *= dxz;

                    var normal = new Vector3(dx, dy, dz);
                    var position = normal * radius;
                    // To generate a UV texture coordinate:
                    //var textureCoordinate = new Vector2(u, v);
                    // To generate a UVW texture cube coordinate
                    //var textureCoordinate = normal;

                    vertices[vertexCount++] = new VertexFormatPositionTextureNormal(position, normal, new Vector2(1.0f, 1.0f));
                }
            }

            // Fill the index buffer with triangles joining each pair of latitude rings.
            int stride = horizontalSegments + 1;

            int indexCount = 0;
            for (int i = 0; i < verticalSegments; i++)
            {
                for (int j = 0; j <= horizontalSegments; j++)
                {
                    int nextI = i + 1;
                    int nextJ = (j + 1) % stride;

                    indices[indexCount++] = (i * stride + j);
                    // Implement correct winding of vertices
                    if (clockWiseWinding)
                    {
                        indices[indexCount++] = (i * stride + nextJ);
                        indices[indexCount++] = (nextI * stride + j);
                    }
                    else
                    {
                        indices[indexCount++] = (nextI * stride + j);
                        indices[indexCount++] = (i * stride + nextJ);
                    }

                    indices[indexCount++] = (i * stride + nextJ);
                    // Implement correct winding of vertices
                    if (clockWiseWinding)
                    {
                        indices[indexCount++] = (nextI * stride + nextJ);
                        indices[indexCount++] = (nextI * stride + j);
                    }
                    else
                    {
                        indices[indexCount++] = (nextI * stride + j);
                        indices[indexCount++] = (nextI * stride + nextJ);
                    }
                }
            }
        }

        void CreateRasterizerState()
        {
            RasterizerStateDescription descr = new RasterizerStateDescription();
            descr.FillMode = FillMode.Solid;
            descr.CullMode = CullMode.Front;
            descr.IsFrontCounterClockwise = true;
            rasterizerState = new RasterizerState(Render11.Direct3DDevice, descr);
        }

        void CreateDepthStencilState()
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

        void CreateBlendState()
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

    struct DrawableGroup
    {
        internal List<IDrawablePart> DrawableParts;
        internal List<ShaderResourceView> ResourceViews;
        internal Material Material;

        internal DrawableGroup(Material material)
        {
            DrawableParts = new List<IDrawablePart>();
            ResourceViews = new List<ShaderResourceView>();
            Material = material;
        }
    }

    interface IDrawablePart
    {
        ShaderBundle ShaderBundle { get; set; }
        IVertexBuffer VertexBuffer { get; set; }
        IIndexBuffer IndexBuffer { get; set; }
        int VertexCount { get; set; }
        int StartVertex { get; set; }
        Matrix WorldMatrix { get; set; }
    }

    struct DrawablePart : IDrawablePart
    {
        public ShaderBundle ShaderBundle { get; set; }
        public IVertexBuffer VertexBuffer { get; set; }
        public IIndexBuffer IndexBuffer { get; set; }
        public int VertexCount { get; set; }
        public int StartVertex { get; set; }
        public Matrix WorldMatrix { get; set; }

        public DrawablePart(Part part, Matrix worldMatrix)
        {
            ShaderBundle = part.ShaderBundle;
            VertexBuffer = part.VertexBuffer;
            IndexBuffer = part.IndexBuffer;
            VertexCount = (int)part.VertexCount;
            StartVertex = (int)part.StartVertex;
            WorldMatrix = worldMatrix;
        }
    }

    struct SkinnedDrawablePart : IDrawablePart
    {
        public ShaderBundle ShaderBundle { get; set; }
        public IVertexBuffer VertexBuffer { get; set; }
        public IIndexBuffer IndexBuffer { get; set; }
        public int VertexCount { get; set; }
        public int StartVertex { get; set; }
        public Matrix WorldMatrix { get; set; }
        public Matrix[] SkinMatrices { get; set; }
        
        public SkinnedDrawablePart(Part part, Matrix worldMatrix, Matrix[] skinMatices)
        {
            ShaderBundle = part.ShaderBundle;
            VertexBuffer = part.VertexBuffer;
            IndexBuffer = part.IndexBuffer;
            VertexCount = (int)part.VertexCount;
            StartVertex = (int)part.StartVertex;
            WorldMatrix = worldMatrix;
            SkinMatrices = skinMatices;
        }
    }
}
