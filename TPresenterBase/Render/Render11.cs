using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using SharpDX.Windows;
using SharpDX.DXGI;
using SharpDX.Direct3D11;
using SharpDX.D3DCompiler;

using Buffer = SharpDX.Direct3D11.Buffer;
using TPresenter.Render.RenderProxy;
using TPresenter.Render.ExternalApp;
using System.Windows.Forms;
using TPrenseter.Render;
using TPresenter.Render.GeometryStage.Model;
using TPresenter.Render.Messages;
using TPresenter.Profiler;

namespace TPresenter.Render
{
    public partial class Render11
    {
        MyShaders shaders;

        public Matrix viewMatrix;
        public Matrix projectionMatrix;
        public Matrix worldMatrix = Matrix.Identity;

        public MyMessageQueue MessageQueue = new MyMessageQueue();
        
        internal List<InstanceComponent> renderInstances = new List<InstanceComponent>();
        internal static bool UseComplementaryDepthBuffer = true;
        internal static MyEnvironment Environment = new MyEnvironment();

#if DEBUG
        bool drawDebugText = true;
#else
        bool drawDebugText = false;
#endif
        System.Windows.Forms.Form Window;

        internal Vector2 Resolution { get { return new Vector2(windowRectangle.Width, windowRectangle.Height); } }
        public bool DrawDebugText
        {
            get { return drawDebugText; }
            set { drawDebugText = value; }
        }

        public void Initialize(System.Windows.Forms.Form window, float dpi = 96.0f)
        {
            //If the device needs to be reinitialized, make sure we are able to recreate our device dependant resources.
            OnInitialize += CreateDeviceDependentResources;
           
            //If the size changes, make sure we are reinitialize any size dependent resources.
            OnSizeChanged += CreateSizeDependentResources;

            Window = window;
            this.windowHandle = window.Handle;
            windowRectangle = window.ClientRectangle;
            shaders = new MyShaders();
            ProfilerStatic.Profiler = new MyProfiler();

            //Initializing device manager.
            InitDevice(dpi);
            SizeChanged();

            MyCommon.Init();
            //Window.SizeChanged += Window_SizeChanged;

            //We trigger an initial size change event to ensure all render buffers and size dependent resources have the 
            //correct dimentions.
            //SizeChanged(true);

            //shaders.Initialize(d3dDevice, d3dContext);
        }

        public void ShutDown()
        {
            DebugMessageRender.Dispose();

            renderTargetView.Dispose();
            backBuffer.Dispose();
            d3dDevice.Dispose();
            d2dDevice.Dispose();
            d3dContext.Dispose();
            swapChain.Dispose();

            PrimitivesRender.Dispose();
            DebugMessageRender.Dispose();
            LinesRender.Dispose();
            geometryRender.Dispose();

            ProfilerStatic.Profiler.Dispose();
        }

        public void InitializeRenders()
        {
            PrimitivesRender.Init();
            DebugMessageRender.Init();
            LinesRender.Init();

            geometryRender.Init();
        }

        public void HandleFocusMessage(WindowFocusMessage msg)
        {
            //if (msg == WindowFocusMessage.Activate && MyRenderProxy.RenderThread.CurrentSettings.WindowMode = WindowModeEnum.Fullscreen)
            //    MyRenderProxy.RenderThread.UpdateSize(WindowModeEnum.FullscreenWindow);

            //if(msg == WindowFocusMessage.SetFocus && MyRenderProxy.RenderThread.CurrentSettings.WindowMode = WindowModeEnum.Fullscreen)
            //{
            //    MyRenderProxy.RenderThread.UpdateSize(WindowModeEnum.Fullscreen);
            //    RestoreFullscreenMode();
            //}
        }

        internal static void RestoreFullscreenMode()
        {
            
        }

        internal void ProcessRenderMessages()
        {
            renderInstances.Clear();
            //  Could messages be added into the queue while we iterating over it?
            RenderMessageBase messageRaw;
            while (MessageQueue.TryDequeue(out messageRaw))
            {
                switch (messageRaw.MessageType)
                {
                    case RenderMessageTypeEnum.SetRenderInstance:
                        var messageRenderInst = (RenderMessageSetRenderInstance)messageRaw;
                        renderInstances.Add(new InstanceComponent(messageRenderInst.Model, messageRenderInst.WorldMatrix));
                        break;
                    case RenderMessageTypeEnum.SetRenderInstanceSkinned:
                        var messageRenderInstSkinned = (RenderMessageSetRenderInstanceSkinned)messageRaw;
                        renderInstances.Add(new SkinnedInstanceComponent(messageRenderInstSkinned.Model, messageRenderInstSkinned.WorldMatrix, messageRenderInstSkinned.SkinMatrices));
                        break;
                }
            }
        }
    }
}
