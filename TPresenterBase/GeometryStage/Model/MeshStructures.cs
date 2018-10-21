using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using TPresenterMath;

#if NETFX_CORE
using Windows.Storage;
#endif

namespace TPresenter.Render.Resources
{
    public struct Triangle
    {
        public Vector3[] Points;
        public Vector3 FaceNormal;
        public int[] Indeces;

        public Triangle(Vertex[] vertices, int i0, int i1, int i2)
        {
            Indeces = new[] { i0, i1, i2 };
            this.Points = (from vertex in vertices select vertex.Position).ToArray();

            //Calculate face normal
            Vector3 edge1 = this.Points[1] - this.Points[0];
            Vector3 edge2 = this.Points[2] - this.Points[0];
            Vector3 faceNormal = Vector3.Cross(edge1, edge2);

            //Calculate face normal direction using vertex normals rather that relying on vertex winding.
            Vector3 avgVertexNormal = Vector3.Normalize((vertices[0].Normal + vertices[1].Normal + vertices[2].Normal) / 3.0f);

            this.FaceNormal = (Vector3.Dot(faceNormal, avgVertexNormal) < 0.0f) ? -faceNormal : faceNormal;
        }

        public Vector3 DominantAxis()
        {
            Vector3 n = this.FaceNormal;
            float max = Math.Max(Math.Abs(n.X), Math.Max(Math.Abs(n.Y), Math.Abs(n.Z)));
            float x, y, z;
            x = Math.Abs(n.X) < max ? 0.0f : 1.0f;
            y = Math.Abs(n.Y) < max ? 0.0f : 1.0f;
            z = Math.Abs(n.Z) < max ? 0.0f : 1.0f;

            if (x > 0)
                return Vector3.UnitX;
            else if (y > 0)
                return Vector3.UnitY;
            else
                return Vector3.UnitZ;
        }
    }

    public struct Ray
    {
        public Vector3 Direction;
        public Vector3 Origin;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SubMesh
    {
        public uint MaterialIndex;
        public uint IndexBufferIndex;
        public uint VertexBufferIndex;
        public uint StartIndex;
        public uint PrimCount;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Material
    {
        public string Name;

        public Vector4 Ambient;
        public Vector4 Diffuse;
        public Vector4 Specular;
        public float SpecularPower;
        public Vector4 Emissive;
        public Matrix UVTransform;

        public string[] Textures;
        public string VertexShaderName;
        public string PixelShaderName;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Vertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector4 Tangent;
        public Color Color;
        public Vector2 UV;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SkinningVertex
    {
        public byte Size;
        public uint[] BoneIndices;
        public float[] BoneWeights;

        public SkinningVertex(int size)
        {
            Size =(byte)size;
            BoneIndices = new uint[Size];
            BoneWeights = new float[Size];
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SkinningVertex1
    {
        public uint BoneIndex0;
        public uint BoneIndex1;
        public uint BoneIndex2;
        public uint BoneIndex3;
        public float BoneWeight0;
        public float BoneWeight1;
        public float BoneWeight2;
        public float BoneWeight3;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct BoneIndices
    {
        public Byte4 BoneIndex0;
        public Byte4 BoneIndex1;
        long _padding0;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct BoneWeights
    {
        public float BoneWeight0;
        public float BoneWeight1;
        public float BoneWeight2;
        public float BoneWeight3;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MeshExtent
    {
        public Vector3 Center;
        public float Radius;

        public Vector3 Min;
        public Vector3 Max;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BoneFromCmo
    {
        public int ParentIndex;
        public Matrix InvBonePose;
        public Matrix BindPose;
        public Matrix BoneLocalTransform;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Bone
    {
        public StringId BoneId;
        public int ParentIndex;
        public Matrix InvBindPose;
        public Matrix BindPose;
        public Matrix BoneLocalTransform;

        public Bone(StringId name, BoneFromCmo fromCmo)
        {
            BoneId = name;
            ParentIndex = fromCmo.ParentIndex;
            InvBindPose = fromCmo.InvBonePose;
            BindPose = fromCmo.BindPose;
            BoneLocalTransform = fromCmo.BoneLocalTransform;
        }
    }

    public struct ModelBone
    {
        public StringId BoneSid;
        public int BoneIndex; //  index of this joint in model skeleton.
        public Matrix InvBindPose;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Keyframe
    {
        public uint BoneIndex;
        public float Time;
        public Matrix Transform;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Animation
    {
        public float StartTime;
        public float EndTime;
        public List<Keyframe> Keyframse;
    }
}
