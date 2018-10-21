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
//    class TriangleRenderer : RenderBase
//    {
//        Buffer triangleVerteces;

//        VertexBufferBinding triangleBinding;

//        protected override void CreateDeviceDependentResources()
//        {
//            base.CreateDeviceDependentResources();

//            RemoveAndDispose(ref triangleVerteces);

//            var device = RenderProvider.Direct3DDevice;

//            //Create a triangle
//            triangleVerteces = ToDispose(Buffer.Create(device, BindFlags.VertexBuffer, new []
//            {
//                //Vertex position                       Vertex color
//                new Vector4(0f, 0f, 15f, 1f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f),     // Base-right
//                new Vector4(-15f, 0f, 0f, 1f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),     // Base-left
//                new Vector4(0f, 15f, 0f, 1f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),     // Apex
//            }));

//            triangleBinding = new VertexBufferBinding(triangleVerteces, Utilities.SizeOf<Vector4>() * 2, 0);
//        }

//        protected override void DoRender()
//        {
//            var context = RenderProvider.Direct3DContext;

//            context.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
//            context.InputAssembler.SetVertexBuffers(0, triangleBinding);
//            context.Draw(3, 0);
//        }
//    }
//}
