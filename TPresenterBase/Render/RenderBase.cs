//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using SharpDX;
//using SharpDX.Windows;
//using SharpDX.DXGI;
//using SharpDX.Direct3D11;
//using TPresenter.Render;

//namespace TPrenseter.Render
//{
//    //This will be merged with Render11 class.
//    public abstract class RenderBase : SharpDX.Toolkit.Component
//    {
//        SharpDX.Direct3D11.DeviceContext _renderContext = null;

//        public Render11 RenderProvider { get; protected set; }
//        public virtual bool Show { get; set; }
//        public Matrix World;

//        public SharpDX.Direct3D11.DeviceContext RenderContext
//        {
//            get { return _renderContext ?? RenderProvider.Direct3DContext; }
//            set { _renderContext = value; }
//        }

//        public RenderBase()
//        {
//            World = Matrix.Identity;
//            Show = true;
//        }

//        /// <summary>
//        /// Initialize with the provided D3DApplicationBase
//        /// </summary>
//        /// <param name="app"></param>
//        public virtual void Initialize(Render11 app)
//        {
//            if (RenderProvider != null)
//                RenderProvider.OnInitialize -= DeviceManager_OnInitialize;

//            RenderProvider = app;
//            RenderProvider.OnInitialize += DeviceManager_OnInitialize;
//            RenderProvider.OnSizeChanged += Target_OnSizeChanged;

//            if (RenderProvider.Direct3DContext != null)
//                CreateDeviceDependentResources();
//        }

//        void DeviceManager_OnInitialize()
//        {
//            CreateDeviceDependentResources();
//        }

//        void Target_OnSizeChanged()
//        {
//            CreateSizeDependentResources();
//        }

//        /// <summary>
//        /// Create any resources that depend on the device or device context
//        /// </summary>
//        protected virtual void CreateDeviceDependentResources() { }

//        /// <summary>
//        /// Create any resources that depend upon the size of the render target
//        /// </summary>
//        protected virtual void CreateSizeDependentResources() { }

//        /// <summary>
//        /// Render a frame
//        /// </summary>
//        public void Render()
//        {
//            if (Show)
//                DoRender();
//        }

//        public void Render(SharpDX.Direct3D11.DeviceContext context)
//        {
//            if (Show)
//                DoRender(context);
//        }

//        /// <summary>
//        /// Each descendant of RendererBase performs a frame
//        /// render within the implementation of DoRender
//        /// </summary>
//        protected abstract void DoRender();

//        protected virtual void DoRender(SharpDX.Direct3D11.DeviceContext context) { }

//    }
//}
