using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TPresenter.Profiler;
using TPresenter.Render.ExternalApp;

namespace TPresenter.Render.RenderProxy
{
    public enum WindowFocusMessage
    {
        Activate,
        SetFocus,
    }

    public static class MyRenderProxy
    {
        #region Fields And Properties

        public static Render11 render;

        //public static MyRenderThread RenderThread { get; private set; }
        public static Thread RenderThread { get; private set; }

        static bool isDisposed = false;

        #endregion

        #region Public Methods

        public static void Initialize(System.Windows.Forms.Form window)
        {
            render = new Render11();
            render.Initialize(window);
            render.InitializeRenders();
            //RenderThread = new Thread(render.Run);
        }

        public static void HandleFocusMessage(WindowFocusMessage msg)
        {
            render.HandleFocusMessage(msg);
        }

        public static void SetCameraViewMatrix(Matrix viewMatrix, Matrix projectionMatrix, float safeNear, float fov,
            float nearestPlaneDistance, float farPlaneDistance, float nearForNearDistance, float farForNearDistance, Vector3 position)
        {
            render.SetupCameraMatrices(viewMatrix, projectionMatrix, safeNear, fov, nearestPlaneDistance, farPlaneDistance, nearForNearDistance, farForNearDistance, position);
        }

        public static void Dispose()
        {
            if (!isDisposed)
            {
                render.ShutDown();
                isDisposed = true;
            }
        }

        public static void Present()
        {
            render.Present();
        }
        #endregion
    }
}
