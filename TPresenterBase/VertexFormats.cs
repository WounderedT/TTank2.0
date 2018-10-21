using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TPresenter.Render.Resources;
using TPresenterMath;

namespace TPresenter.Render
{
    struct VertexFormatPositionTexture
    {
        internal Vector3 Position;
        internal Vector2 Texcoord;

        internal VertexFormatPositionTexture(Vector3 position, Vector2 texcoord)
        {
            Position = position;
            Texcoord = texcoord;
        }

        internal static unsafe int STRIDE = sizeof(VertexFormatPositionTexture);
    }

    //  Normal and Texcoord should be moved to separate structure.
    struct VertexFormatPositionTextureNormal
    {
        internal Vector3 Position;
        internal Vector3 Normal;
        internal Vector2 Texcoord;

        internal VertexFormatPositionTextureNormal(Vector3 position, Vector3 normal, Vector2 texcoord)
        {
            Position = position;
            Normal = normal;
            Texcoord = texcoord;
        }

        internal VertexFormatPositionTextureNormal(Vector3 position, Vector2 texcoord)
        {
            Position = position;
            Normal = Vector3.Normalize(position);
            Texcoord = texcoord;
        }

        internal static unsafe int STRIDE = sizeof(VertexFormatPositionTextureNormal);
    }

    //  Normal and Texcoord should be moved to separate structure.
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct VertexFormatPositionSkinningTextureNormal
    {
        internal Vector3 Position;
        internal Vector3 Normal;
        internal Vector2 Texcoord;
        internal BoneIndices BoneIndices0;
        
        internal BoneWeights BoneWeights0;
        internal BoneWeights BoneWeights1;

        internal VertexFormatPositionSkinningTextureNormal(Vector3 position, Vector3 normal, Vector2 texcoord, SkinningVertex skin)
        {
            Position = position;
            var indices = new uint[24];
            var weights = new float[20];
            for (int i = 0; i < skin.BoneIndices.Length; i++)
            {
                indices[i] = skin.BoneIndices[i];
                weights[i] = skin.BoneWeights[i];
            }
            BoneIndices0 = new BoneIndices()
            {
                BoneIndex0 = new Byte4(indices[0], indices[1], indices[2], indices[3]),
                BoneIndex1 = new Byte4(indices[4], indices[5], indices[6], indices[7]),
            };
            byte ind = 0;
            BoneWeights0 = new BoneWeights()
            {
                BoneWeight0 = weights[ind++],
                BoneWeight1 = weights[ind++],
                BoneWeight2 = weights[ind++],
                BoneWeight3 = weights[ind++],
            };
            BoneWeights1 = new BoneWeights()
            {
                BoneWeight0 = weights[ind++],
                BoneWeight1 = weights[ind++],
                BoneWeight2 = weights[ind++],
                BoneWeight3 = weights[ind++],
            };
            
            Normal = normal;
            Texcoord = texcoord;
        }

        internal static unsafe int STRIDE = sizeof(VertexFormatPositionSkinningTextureNormal);
    }

    struct VertexFormatPositionColor
    {
        internal Vector3 Position;
        internal Color Color;

        internal VertexFormatPositionColor(Vector3 position, Color color)
        {
            Position = position;
            Color = color;
        }

        internal static unsafe int STRIDE = sizeof(VertexFormatPositionColor);
    }
}
