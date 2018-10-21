using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.WIC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Buffer = SharpDX.Direct3D11.Buffer;
using Resource = SharpDX.Direct3D11.Resource;

namespace TPresenter.Render.Resources
{
    [StructLayout(LayoutKind.Explicit)]
    struct DirectionalLight
    {
        [FieldOffset(0)]
        internal Color4 color;
        [FieldOffset(16)]
        internal Vector3 direction;
        [FieldOffset(28)]
        float __padding;
    }

    [StructLayout(LayoutKind.Explicit)]
    struct PerObject
    {
        [FieldOffset(0)]
        internal Matrix World;   // world matrix to calculate lighting in world space
        [FieldOffset(64)]
        internal Matrix InvWorldTranspose;  // invires transpose of the World for normals

        /// <summary>
        /// Transpose the matrices so that they are in row major order for HLSL
        /// </summary>
        internal void Transpose()
        {
            World.Transpose();
            InvWorldTranspose.Transpose();
        }
    }

    //[StructLayout(LayoutKind.Explicit)]
    //struct PerFrame
    //{
    //    [FieldOffset(0)]
    //    internal Vector3 cameraPosition;
    //    [FieldOffset(12)]
    //    internal float __padding;    // extra property to make size evenly divisible by 16.
    //    [FieldOffset(16)]
    //    internal Matrix ViewProj;
    //    [FieldOffset(80)]
    //    internal DirectionalLight light;
    //}

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct PerFrame
    {
        internal Vector3 cameraPosition;
        internal float __padding0;    // extra property to make size evenly divisible by 16.
        internal Matrix ViewProj;
        internal DirectionalLight light;
        internal float tessellationFactor;
        internal Vector3 __padding1;    // extra property to make size evenly divisible by 16.
    }

    //[StructLayout(LayoutKind.Explicit)]
    //struct PerMaterial
    //{
    //    [FieldOffset(0)]
    //    internal Color4 ambient;
    //    [FieldOffset(16)]
    //    internal Color4 diffuse;
    //    [FieldOffset(32)]
    //    internal Color4 specular;
    //    [FieldOffset(48)]
    //    internal float specularPower;
    //    [FieldOffset(52)]
    //    internal uint hasTexture;
    //    [FieldOffset(56)]
    //    internal Vector2 __padding;
    //    [FieldOffset(64)]
    //    public Color4 emissive;
    //    [FieldOffset(80)]
    //    public Matrix uvTransform;
    //}

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct PerMaterial
    {
        internal Color4 ambient;
        internal Color4 diffuse;
        internal Color4 specular;
        internal float specularPower;
        internal uint hasTexture;    // Does the current material have a texture (0 false, 1 true)
        Vector2 _padding0;
        internal Color4 emissive;
        internal Matrix uvTransform; // Support UV coordinate transformations

        internal PerMaterial(Material material)
        {
            ambient = new Color4(material.Ambient);
            diffuse = new Color4(material.Diffuse);
            emissive = new Color4(material.Emissive);
            specular = new Color4(material.Specular);
            specularPower = material.SpecularPower;
            uvTransform = material.UVTransform;

            hasTexture = default(uint);
            _padding0 = new Vector2();
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    //[StructLayout(LayoutKind.Explicit, Size = 256 * 16 * 4)]
    internal struct PerSkeleton
    {
        //[FieldOffset(0), MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        internal Matrix[] Bones;

        public PerSkeleton(int bones)
        {
            Bones = new Matrix[256];
        }

        internal static unsafe int STRIDE = Utilities.SizeOf<Matrix>() * 256;
    }
}
