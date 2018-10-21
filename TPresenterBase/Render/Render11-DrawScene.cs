using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TPrenseter.Render;
using TPresenter.Profiler;
using TPresenter.Render.GeometryStage.Model;
using TPresenter.Render.GeometryStage.Rendering;
using TPresenterMath;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace TPresenter.Render
{
    public partial class Render11
    {
        public static Matrix cameraOrientation; //Debug only!
        internal GeometryRender geometryRender = new GeometryRender(); //Debug only!

        //This method should be made private static if render updates will be message-based.
        public void SetupCameraMatrices(Matrix viewMatrix, Matrix projectionMatrix, float safeNear, float fov,
            float nearestPlaneDistance, float farPlaneDistance, float nearForNearDistance, float farForNearDistance, Vector3 position)
        {
            var originalProjecion = projectionMatrix;
            var cameraPosition = position;

            var viewMatrixAt0 = viewMatrix;
            viewMatrixAt0.M14 = 0;
            viewMatrixAt0.M24 = 0;
            viewMatrixAt0.M34 = 0;
            viewMatrixAt0.M41 = 0;
            viewMatrixAt0.M42 = 0;
            viewMatrixAt0.M43 = 0;
            viewMatrixAt0.M44 = 0;

            float aspectRatio = Resolution.X / Resolution.Y;

            //Matrix projMatrix = Matrix_T.CreatePerspectiveForRhInfiniteCompemetary(fov, aspectRatio, nearestPlaneDistance);
            Matrix projMatrix = Matrix_T.CreatePerspectiveForLhInfiniteComplemetary(fov, aspectRatio, nearestPlaneDistance);

            Environment.Matrices.ViewAt0 = viewMatrixAt0;
            Environment.Matrices.InvViewAt0 = Matrix.Invert(viewMatrixAt0);
            Environment.Matrices.ViewProjectionAt0 = viewMatrixAt0 * projMatrix;
            Environment.Matrices.InvViewProjectionAt0 = Matrix.Invert(Environment.Matrices.ViewProjectionAt0);
            Environment.Matrices.CameraPosition = cameraPosition;
            Environment.Matrices.View = viewMatrix;
            Environment.Matrices.InvView = Matrix.Invert(viewMatrix);
            Environment.Matrices.ViewProjection = viewMatrix * projMatrix;
            Environment.Matrices.InvViewProjection = Matrix.Invert(viewMatrix * projMatrix);
            Environment.Matrices.Projection = projMatrix;
            Environment.Matrices.InvProjection = Matrix.Invert(projMatrix);
            Environment.Matrices.NearClipping = nearestPlaneDistance;
            Environment.Matrices.FarClipping = farPlaneDistance;

            Environment.Matrices.WorldViewProjection = worldMatrix * Environment.Matrices.ViewProjection;

            int width = (int)Resolution.X;
            int height = (int)Resolution.Y;
            float fovH = fov;
            Environment.Matrices.FovH = fovH;
            Environment.Matrices.FovL = (float)(2 * Math.Atan(Math.Tan(fovH / 2.0) * (width / (double)height)));
        }

        public void Draw()
        {
            ProfilerStatic.BeginSubstep("Render");
            d3dContext.ClearDepthStencilView(DepthStencilView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0);
            d3dContext.ClearRenderTargetView(RenderTargetView, Color.White);

            ProfilerStatic.BeginSubstep("Processing messages", true);
            ProcessRenderMessages();
            ProfilerStatic.EndSubstep();

            //PrimitivesRender.DrawTriangle(new Vector3(-15f, 0f, 0f), new Vector3(0f, 0f, 15f), new Vector3(-7.5f, 15f, 7.5f),
            //    new Color(1.0f, 0.0f, 0.0f, 1.0f), new Color(1.0f, 0.0f, 0.0f, 1.0f), new Color(1.0f, 0.0f, 0.0f, 1.0f));
            //PrimitivesRender.DrawQuadClockWise(new Vector3(0, 0f, -15f), new Vector3(15f, 0f, -7.5f), new Vector3(0f, 15f, -15f), new Vector3(15f, 15f, -7.5f),
            //    new Color(0.0f, 1.0f, 0.0f, 1.0f), new Color(0.0f, 1.0f, 0.0f, 1.0f), new Color(0.0f, 1.0f, 0.0f, 1.0f), new Color(0.0f, 1.0f, 0.0f, 1.0f));
            //PrimitivesRender.DrawBoundingBox(new BoundingBox(new Vector3(-3, -3, -3), new Vector3(0, 0, 0)), new Color(0.0f, 1.0f, 0.0f, 1.0f));
            //PrimitivesRender.Draw();

            geometryRender.Draw(renderInstances);

            ProfilerStatic.BeginSubstep("Axis and grid preparation");
            DebugLinesRender.AddAxisLinesFull(7);
            DebugLinesRender.AddGrid();
            ProfilerStatic.EndSubstep();

            ProfilerStatic.BeginSubstep("Draw axis and grid");
            LinesRender.Draw();
            ProfilerStatic.EndSubstep(true);

            if (DrawDebugText)
                DebugMessageRender.Draw();
            ProfilerStatic.EndSubstep();
        }
    }
}
