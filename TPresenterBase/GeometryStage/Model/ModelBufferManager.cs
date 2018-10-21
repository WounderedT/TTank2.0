using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPresenter.Render.Resources;

namespace TPresenter.Render.GeometryStage.Model
{
    // Probably should not be static
    static class ModelBufferManager
    {
        static Dictionary<StringId, IVertexBuffer> verterBuffers = new Dictionary<StringId, IVertexBuffer>(StringId.Comparer);
        static Dictionary<StringId, IIndexBuffer> indexBuffers = new Dictionary<StringId, IIndexBuffer>(StringId.Comparer);

        public static IVertexBuffer GetOrCreateVertexBuffer(Vertex[] vertices, StringId geometryId, SkinningVertex[] skinning = null)
        {
            StringId key = geometryId;
            if (verterBuffers.ContainsKey(key))
                return verterBuffers[key];

            var buffer = CreateVertexBuffer(vertices, geometryId.String, skinning);
            verterBuffers.Add(key, buffer);
            return buffer;
        }

        public static IIndexBuffer GetOrCreateIndexBuffer(ushort[] indices, StringId geometryId)
        {
            StringId key = geometryId;
            if (indexBuffers.ContainsKey(key))
                return indexBuffers[key];

            var buffer = CreateIndexBuffer(indices, geometryId.String);
            indexBuffers.Add(key, buffer);
            return buffer;
        }

        static unsafe IVertexBuffer CreateVertexBuffer(Vertex[] vertices, string modelName, SkinningVertex[] skinning = null)
        {
            string name = "VB-" + modelName;
            if (skinning == null)
            {
                VertexFormatPositionTextureNormal[] vbVertices = new VertexFormatPositionTextureNormal[vertices.Length];
                for (var i = 0; i < vertices.Length; i++)
                {
                    vbVertices[i] = new VertexFormatPositionTextureNormal(vertices[i].Position, vertices[i].Normal, vertices[i].UV);
                }
                fixed (void* ptr = vbVertices)
                {
                    return BufferManager.CreateVertexBuffer(name, vbVertices.Length, VertexFormatPositionTextureNormal.STRIDE, new IntPtr(ptr), ResourceUsage.Immutable);
                }
            }
            else
            {
                VertexFormatPositionSkinningTextureNormal[] vbVertices = new VertexFormatPositionSkinningTextureNormal[vertices.Length];
                for (var i = 0; i < vertices.Length; i++)
                {
                    SkinningVertex skin = new SkinningVertex();
                    skin = skinning[i];
                    vbVertices[i] = new VertexFormatPositionSkinningTextureNormal(
                        vertices[i].Position, vertices[i].Normal, vertices[i].UV, skin);
                }
                fixed (void* ptr = vbVertices)
                {
                    return BufferManager.CreateVertexBuffer(name, vbVertices.Length, VertexFormatPositionSkinningTextureNormal.STRIDE, new IntPtr(ptr), ResourceUsage.Immutable);
                }
            }
        }

        static unsafe IIndexBuffer CreateIndexBuffer(ushort[] indices, string modelName)
        {
            string name = "IB-" + modelName;
            fixed(void* ptr = indices)
            {
                return BufferManager.CreateIndexBuffer(name, indices.Length, new IntPtr(ptr), usage: ResourceUsage.Immutable);
            }
        }
    }
}
