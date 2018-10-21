using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPresenter.Render.GeometryStage.Rendering;
using TPresenter.Render.Resources;

namespace TPresenter.Render.GeometryStage.Model
{
    public class Part : IDisposable
    {
        bool isDisposed;

        internal IVertexBuffer VertexBuffer { get; set; }
        internal IIndexBuffer IndexBuffer { get; set; }
        internal Material Material { get; set; }
        internal StringId Name { get; set; }
        internal uint VertexCount { get; set; }
        internal uint StartVertex { get; set; }

        internal List<Triangle> Triangles { get; set; }

        public Dictionary<StringId, ModelBone> ModelBones { get; set; }

        public Matrix BindShapeMatrix { get; set; }

        internal ShaderBundle ShaderBundle { get; set; }

        //internal void Init(IVertexBuffer vertexBuffer, IIndexBuffer indexBuffer, Material material, string name, uint vertexCount, uint startVertex, Matrix bindShapeMatrix, List<Bone> bones = null)
        internal void Init(IVertexBuffer vertexBuffer, IIndexBuffer indexBuffer, Material material,
            StringId name, uint vertexCount, uint startVertex, Dictionary<StringId, ModelBone> modelBones = null)
        {
            VertexBuffer = vertexBuffer;
            IndexBuffer = indexBuffer;
            Material = material;
            Name = name;
            VertexCount = vertexCount;
            StartVertex = startVertex;
            isDisposed = false;
            //  This need to be rewritten.
            if (modelBones != null)
            {
                ModelBones = modelBones;
                ShaderBundle = ShaderResolver.GetShaderBundle("SHADER_OBJECT_SKINNING", true, MyShaderFlags.USE_SKINNING);
            }
            else
            {
                ShaderBundle = ShaderResolver.GetShaderBundle("SHADER_OBJECT", false, MyShaderFlags.NONE);
            }
        }

        public void Dispose()
        {
            if (!isDisposed)
                return;

            VertexBuffer.Dispose();
            IndexBuffer.Dispose();
            Material = default(Material);
            Name = StringId.NullOrEmpty;

            isDisposed = true;
        }
    }
}
