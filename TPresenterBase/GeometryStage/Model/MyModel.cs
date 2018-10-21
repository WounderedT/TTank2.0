using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TPresenter.Filesystem;
using TPresenter.Render.GeometryStage.Rendering;
using TPresenter.Render.Resources;

namespace TPresenter.Render.GeometryStage.Model
{
    enum ModelType
    {
        ANIMATED,
        STATIC,
    }

    public class MyModel
    {
        internal List<IVertexBuffer> vertexBuffers = new List<IVertexBuffer>();
        internal List<IIndexBuffer> indexBuffers = new List<IIndexBuffer>();
        internal List<MyShaderResourceView> texturesViews = new List<MyShaderResourceView>();
        internal SamplerState samplerState;

        public List<Part> Parts = new List<Part>();
        public string Name { get; set; }

        internal IConstantBuffer PerMaterialBuffer { get; set; }

        public Vertex[] Vertices;
        public SkinningVertex[] Skinning;

        //  Method is too cumbersome. Should be splitted.
        internal unsafe void Load(string modelName)
        {
            Name = modelName.Split(new string[] { "//" }, StringSplitOptions.None).Last();
            ColladaModel model = new ColladaModel();
            model.LoadModel(modelName);
            for (int i = 0; i < model.Parts.Length; i++)
            {
                var geometryPart = model.Parts[i];
                Part part = new Part();
                part.Init(
                    ModelBufferManager.GetOrCreateVertexBuffer(geometryPart.Vertices, geometryPart.GeometryId, geometryPart.Skinning),
                    ModelBufferManager.GetOrCreateIndexBuffer(geometryPart.Indices, geometryPart.GeometryId),
                    model.Material,
                    geometryPart.GeometryId,
                    geometryPart.VertexCount,
                    geometryPart.StartVertex);
                part.BindShapeMatrix = geometryPart.BindShapeMatrix;

                Parts.Add(part);
                texturesViews.Add(new MyShaderResourceView(model.Material, modelName));

                Vertices = geometryPart.Vertices;
            }
        }

        //  Method is too cumbersome. Should be splitted.
        internal unsafe void LoadSkinned(string modelName, Skeleton skeleton)
        {
            Name = modelName.Split(new string[] { "//" }, StringSplitOptions.None).Last();
            ColladaModel model = new ColladaModel();
            model.LoadSkinnedModel(modelName, skeleton);
            for (int i = 0; i < model.Parts.Length; i++)
            {
                var geometryPart = model.Parts[i];
                Part part = new Part();
                part.Init(
                    ModelBufferManager.GetOrCreateVertexBuffer(geometryPart.Vertices, geometryPart.GeometryId, geometryPart.Skinning),
                    ModelBufferManager.GetOrCreateIndexBuffer(geometryPart.Indices, geometryPart.GeometryId),
                    model.Material,
                    geometryPart.GeometryId,
                    geometryPart.VertexCount,
                    geometryPart.StartVertex,
                    model.Parts[i].Bones);
                part.BindShapeMatrix = geometryPart.BindShapeMatrix;

                Parts.Add(part);
                texturesViews.Add(new MyShaderResourceView(model.Material, modelName));

                Vertices = geometryPart.Vertices;
                Skinning = geometryPart.Skinning;
            }
        }
    }

    class MyShaderResourceView
    {
        internal StringId MaterialId { get; private set; }
        internal List<ShaderResourceView> ResourceView = new List<ShaderResourceView>();

        internal MyShaderResourceView(Material material, string modelFileName)
        {
            MaterialId = StringId.GetOrCompute(material.Name);
            for (var ind = 0; ind < material.Textures.Length; ind++)
            {
                if (material.Textures[ind].Equals(string.Empty))
                    continue;

                //TODO: Only diffuse texture is supported at the moment. Workaround for models with additional textures (e.x. Bump, Specular)
                var textureFilePath = System.IO.Path.Combine(FileProvider.TexturesPath, GetDiffuseTexture(material.Textures));
                //var textureFilePath = System.IO.Path.Combine(FileProvider.TexturesPath, material.Textures[ind]);
                if (SharpDX.IO.NativeFile.Exists(textureFilePath))
                {
                    Resource texture = MyTexture.GetTextureFromFile(textureFilePath);
                    ResourceView.Add(new ShaderResourceView(Render11.Direct3DDevice, texture));
                }
                else
                {
                    Resource texture = MyTexture.GetTextureFromFile(System.IO.Path.Combine(FileProvider.TexturesPath, "error_texture.dds"));
                    ResourceView.Add(new ShaderResourceView(Render11.Direct3DDevice, texture));
                }
            }
        }

        private String GetDiffuseTexture(String[] textures)
        {
            Regex diffuseRegEx = new Regex(".*_d01.dds");
            if(textures.Length > 1)
            {
                foreach(String texture in textures)
                {
                    if (diffuseRegEx.IsMatch(texture))
                    {
                        return texture;
                    }
                }
                return textures[0];
            }
            else
            {
                return textures[0];
            }
        }
    }
}
