//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using TPrenseter.Render;

//using SharpDX;
//using SharpDX.Direct3D11;

//// Resolve class name conflicts by explicitly stating
//// which class they refer to:
//using Buffer = SharpDX.Direct3D11.Buffer;

//namespace TPresenter.Render
//{
//    class QuadRenderer : RenderBase
//    {
//        Buffer quadVerteces;
//        Buffer quadIndices;
//        VertexBufferBinding quadBinding;

//        protected override void CreateDeviceDependentResources()
//        {
//            base.CreateDeviceDependentResources();

//            RemoveAndDispose(ref quadVerteces);
//            RemoveAndDispose(ref quadIndices);

//            var device = RenderProvider.Direct3DDevice;

//            //Create a quad(two triangles)
//            quadVerteces = ToDispose(Buffer.Create(device, BindFlags.VertexBuffer, new []
//            {
//                //Vertex position                       Vertex color
//                new Vector4(7.5f, 15f, -15f, 1f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),   // Top-left
//                new Vector4(22.5f, 15f, -15f, 1f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f),   // Top-right
//                new Vector4(22.5f, 0.0f, -15f, 1f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),   // Base-right
//                new Vector4(7.5f, 0.0f, -15f, 1f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f),   // Base-left
//            }));

//            quadBinding = new VertexBufferBinding(quadVerteces, Utilities.SizeOf<Vector4>() * 2, 0);

//            quadIndices = ToDispose(Buffer.Create(device, BindFlags.IndexBuffer, new ushort[]
//            {
//                0, 1, 2,    //first triangle
//                2, 3, 0     //second triangle
//            }));
//        }

//        protected override void DoRender()
//        {
//            var context = RenderProvider.Direct3DContext;

//            context.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
//            context.InputAssembler.SetIndexBuffer(quadIndices, SharpDX.DXGI.Format.R16_UInt, 0);
//            context.InputAssembler.SetVertexBuffers(0, quadBinding);
//            context.DrawIndexed(6, 0, 0);
//        }
//    }
//}
