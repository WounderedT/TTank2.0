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
//    class AxisLinesRenderer : RenderBase
//    {
//        //The vertex buffer for axis lines.
//        public Buffer axisLinesVerteces;

//        //The binding structure of the axis lines vertex buffer
//        public VertexBufferBinding axisLinesBinding;

//        /// <summary>
//        /// Create any device dependent resources here.
//        /// This method will be called when the device is first
//        /// initialized or recreated after being removed or reset.
//        /// </summary>
//        protected override void CreateDeviceDependentResources()
//        {
//            RemoveAndDispose(ref axisLinesVerteces);

//            var device = RenderProvider.Direct3DDevice;

//            //Create xyz-axis arrows. X is Red, Y is Greenm Z is Blue
//            //The arrows point along the plus for each axis
//            axisLinesVerteces = ToDispose(Buffer.Create(device, BindFlags.VertexBuffer, new []
//            {
//                //Vertex position                       Vertex color
//                new Vector4(-1f, 0f, 0f, 1f), (Vector4)Color.Red,           // - x-axis
//                new Vector4(1f, 0f, 0f, 1f), (Vector4)Color.Red,           // + x-axis
//                new Vector4(0.9f, -0.05f, 0f, 1f), (Vector4)Color.Red,      // arrow head start
//                new Vector4(1f, 0f, 0f, 1f), (Vector4)Color.Red,
//                new Vector4(0.9f, 0.05f, 0f, 1f), (Vector4)Color.Red,
//                new Vector4(1f, 0f, 0f, 1f), (Vector4)Color.Red,            // arrow head end

//                new Vector4(0f, -1f, 0f, 1f), (Vector4)Color.Green,           // - y-axis
//                new Vector4(0f, 1f, 0f, 1f), (Vector4)Color.Green,           // + y-axis
//                new Vector4(-0.05f, 0.9f, 0f, 1f), (Vector4)Color.Green,      // arrow head start
//                new Vector4(0f, 1f, 0f, 1f), (Vector4)Color.Green,
//                new Vector4(0.05f, 0.9f, 0f, 1f), (Vector4)Color.Green,
//                new Vector4(0f, 1f, 0f, 1f), (Vector4)Color.Green,            // arrow head end

//                new Vector4(0f, 0f, -1f, 1f), (Vector4)Color.Blue,           // - z-axis
//                new Vector4(0f, 0f, 1f, 1f), (Vector4)Color.Blue,           // + z-axis
//                new Vector4(0f, -0.05f, 0.9f, 1f), (Vector4)Color.Blue,      // arrow head start
//                new Vector4(0f, 0f, 1f, 1f), (Vector4)Color.Blue,
//                new Vector4(0f, 0.05f, 0.9f, 1f), (Vector4)Color.Blue,
//                new Vector4(0f, 0f, 1f, 1f), (Vector4)Color.Blue,            // arrow head end
//            }));

//            axisLinesBinding = new VertexBufferBinding(axisLinesVerteces, Utilities.SizeOf<Vector4>() * 2, 0);
//        }

//        protected override void DoRender()
//        {
//            //Get the context reference
//            var context = RenderProvider.Direct3DContext;

//            //Render the Axis lines

//            //Tell the IA we are using lines
//            context.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.LineList;
//            //Pass in the line verteces
//            context.InputAssembler.SetVertexBuffers(0, axisLinesBinding);
//            //Draw the 18 verteces or out xyz-arrows
//            context.Draw(18, 0);
//        }
//    }
//}
