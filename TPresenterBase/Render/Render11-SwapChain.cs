using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using SharpDX.Windows;
using SharpDX.DXGI;
using SharpDX.Direct3D11;
using TPresenter.Render.ExternalApp;

namespace TPresenter.Render
{
    public partial class Render11 : SharpDX.Toolkit.Component
    {
        #region Fields And Propeties

        static System.Drawing.Rectangle windowRectangle;

        SharpDX.DXGI.SwapChain1 swapChain;

        static SharpDX.Direct3D11.RenderTargetView renderTargetView;
        static SharpDX.Direct3D11.DepthStencilView depthStencilView;
        SharpDX.Direct3D11.Texture2D backBuffer;
        SharpDX.Direct3D11.Texture2D depthBuffer;

        SharpDX.Direct2D1.Bitmap1 bitmapTarget;


        //public GameWindowForm Window { get { return window; } }
        IntPtr windowHandle;

        /// <summary>
        /// Gets the configured bounds of the control used to render to
        /// </summary>
        public static SharpDX.Rectangle Bounds { get; protected set; }

        /// <summary>
        /// Gets the currect bounds of the control used to render to
        /// </summary>
        public static Rectangle CurrentBounds
        {
            get
            {
                return new SharpDX.Rectangle(windowRectangle.X, windowRectangle.Y,
                    windowRectangle.Width, windowRectangle.Height);
            }
        }

        /// <summary>
        /// Gets the <see cref="SharpDX.DXGI.SwapChain1"/> attached to this instance.
        /// </summary>
        public SwapChain1 SwapChain { get { return swapChain; } }

        /// <summary>
        /// Provides access to the list of available display modes.
        /// </summary>
        public ModeDescription[] DisplayModeList { get; private set; }

        /// <summary>
        /// Gets or sets whether the swap chain will wait for the 
        /// next vertical sync before presenting.
        /// </summary>
        /// <remarks>
        /// Changes the behavior of the <see cref="D3DApplicationBase.Present"/> method.
        /// </remarks>
        public bool VSync { get; set; }

        /// <summary>
        /// Width of the swap chain buffers.
        /// </summary>
        public int Width
        {
            get
            {
                return (int)(Bounds.Width * Dpi / 96.0);
            }
        }

        /// <summary>
        /// Height of the swap chain buffers.
        /// </summary>
        public int Height
        {
            get
            {
                return (int)(Bounds.Height * Dpi / 96.0);
            }
        }

        /// <summary>
        /// Gets the Direct3D RenderTargetView used by this app.
        /// </summary>
        public static RenderTargetView RenderTargetView
        {
            get { return renderTargetView; }
            set
            {
                if(renderTargetView != value)
                {
                    if(renderTargetView != null)
                        renderTargetView.Dispose();
                    renderTargetView = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the BackBuffer used by this app.
        /// </summary>
        public Texture2D BackBuffer
        {
            get { return backBuffer; }
            set
            {
                if(backBuffer != value){
                    RemoveAndDispose(ref backBuffer);
                    backBuffer = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the depthBuffer used by this app.
        /// </summary>
        public Texture2D DepthBuffer
        {
            get { return depthBuffer; }
            set
            {
                if (depthBuffer != value)
                {
                    RemoveAndDispose(ref depthBuffer);
                    depthBuffer = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the DepthStencilView used by this app.
        /// </summary>
        public static SharpDX.Direct3D11.DepthStencilView DepthStencilView
        {
            get { return depthStencilView; }
            set
            {
                if (depthStencilView != value)
                {
                    if(depthStencilView != null)
                        depthStencilView.Dispose();
                    depthStencilView = value;
                }
            }
        }

        /// <summary>
        /// Gets the Direct2D RenderTarget used by this app.
        /// </summary>
        public SharpDX.Direct2D1.Bitmap1 BitmapTarget2D
        {
            get { return bitmapTarget; }
            set
            {
                if (bitmapTarget != value)
                {
                    RemoveAndDispose(ref bitmapTarget);
                    bitmapTarget = value;
                }
            }
        }
        
        public ViewportF Viewport { get; set; }
        protected SharpDX.Rectangle RenderTargetBouds { get; set; }
        protected SharpDX.Size2 RenderTargetSize { get { return new SharpDX.Size2(RenderTargetBouds.Width, RenderTargetBouds.Height); } }

        #endregion

        #region Events

        /// <summary>
        /// Event fired when size of the underlying render control is changed
        /// </summary>
        public Action OnSizeChanged;

        #endregion

        /// <summary>
        /// Trigger the OnSizeChanged event if the width and height
        /// of the <see cref="CurrentBounds"/> differs to the
        /// last call to SizeChanged.
        /// </summary>
        protected void SizeChanged(bool force = false) {
            var newBounds = CurrentBounds;

            if((newBounds.Width == 0 && newBounds.Height != 0) || (newBounds.Width != 0 && newBounds.Height == 0))
            {
                return;
            }

            if(newBounds.Width != Bounds.Width || newBounds.Height != Bounds.Height || force)
            {
                Bounds = newBounds;
                OnSizeChanged?.Invoke();
            }
        }

        /// <summary>
        /// Create device dependent resources
        /// </summary>
        /// <param name="deviceManager"></param>
        protected void CreateDeviceDependentResources() {
            if(swapChain != null)
            {
                RemoveAndDispose(ref swapChain);

                SizeChanged(true);
            }
        }

        /// <summary>
        /// Create size dependent resources, in this case the swap chain and render targets
        /// </summary>
        protected void CreateSizeDependentResources() {
            //Before the swapchain can resize all the buffers must be released.
            RemoveAndDispose(ref backBuffer);
            RemoveAndDispose(ref depthBuffer);
            RemoveAndDispose(ref renderTargetView);
            RemoveAndDispose(ref depthStencilView);
            RemoveAndDispose(ref bitmapTarget);
            d2dContext.Target = null;

            #region Initialize Direct3D swap chain and render targets.

            if (swapChain != null)
            {
                swapChain.ResizeBuffers(swapChain.Description1.BufferCount, Width, Height,
                    swapChain.Description.ModeDescription.Format, swapChain.Description.Flags);
            }
            else
            {
                var desc = CreateSwapChainDescription();

                //Rather than create a new DXGI Factory we reuse the one that has been used internally to
                //create the device.
                using (var dxgiDevice2 = d3dDevice.QueryInterface<SharpDX.DXGI.Device2>())
                using (var dxgiAdapter = dxgiDevice2.Adapter)
                using (var dxgiFactory2 = dxgiAdapter.GetParent<SharpDX.DXGI.Factory2>())
                using(var output = dxgiAdapter.Outputs.First())
                {
                    swapChain = ToDispose(CreateSwapChain(dxgiFactory2, d3dDevice, desc));

#if !NETFX_CORE
                    // Retrieve the list of supported display modes
                    DisplayModeList = output.GetDisplayModeList(desc.Format, DisplayModeEnumerationFlags.Scaling);
#endif
                }
            }

            BackBuffer = ToDispose(Texture2D.FromSwapChain<Texture2D>(swapChain, 0));
            RenderTargetView = ToDispose(new RenderTargetView(d3dDevice, BackBuffer));

            //var backBufferDescription = BackBuffer.Description;
            RenderTargetBouds = new SharpDX.Rectangle(0, 0, BackBuffer.Description.Width, BackBuffer.Description.Height);

            d3dContext.SetScreenViewport();

            //Create a descriptior for the depth/stencil buffer.
            //Allocate a 2-D texture as the depth/stencil buffer.
            //Create a DSV to use on bind.
            var textureDesctiption = new Texture2DDescription()
            {
                Width = RenderTargetSize.Width,
                Height = RenderTargetSize.Height,
                Format = SharpDX.DXGI.Format.D32_Float_S8X24_UInt,
                ArraySize = 1,
                MipLevels = 1,
                BindFlags = BindFlags.DepthStencil,
                Usage = ResourceUsage.Default,
                CpuAccessFlags = 0,
                SampleDescription = SwapChain.Description.SampleDescription,
                OptionFlags = 0,
            };
            DepthBuffer = ToDispose(new Texture2D(d3dDevice, textureDesctiption));
            DepthBuffer.DebugName = "BaseDepthBuffer";

            DepthStencilViewDescription dsDesc = new DepthStencilViewDescription();
            dsDesc.Format = Format.D32_Float_S8X24_UInt;
            dsDesc.Dimension = (SwapChain.Description.SampleDescription.Count > 1 || SwapChain.Description.SampleDescription.Quality > 0) ? DepthStencilViewDimension.Texture2DMultisampled : DepthStencilViewDimension.Texture2D;
            dsDesc.Flags = DepthStencilViewFlags.None;

            DepthStencilView = ToDispose(new DepthStencilView(d3dDevice, DepthBuffer, dsDesc));

            d3dContext.SetRenderTargetView(DepthStencilView, RenderTargetView);

            #endregion

            #region Initialize 2D render target

            //Now we set up the Direct2D render target bitmap linked to the swapchain. 
            //Whenever we render to this bitmap, it will be directly rendered to the 
            //swapchain associated with the window.
            var bitmapProperties = new SharpDX.Direct2D1.BitmapProperties1(
                new SharpDX.Direct2D1.PixelFormat(swapChain.Description.ModeDescription.Format, SharpDX.Direct2D1.AlphaMode.Premultiplied),
                Dpi,
                Dpi,
                SharpDX.Direct2D1.BitmapOptions.Target | SharpDX.Direct2D1.BitmapOptions.CannotDraw);

            //Direct2D needs the dxgi version of the backbuffer surface pointer.
            //Get a D2D surface from the DXGI back buffer to use as the D2D render target.
            using (var dxgiBackBuffer = swapChain.GetBackBuffer<SharpDX.DXGI.Surface>(0))
                BitmapTarget2D = ToDispose(new SharpDX.Direct2D1.Bitmap1(d2dContext, dxgiBackBuffer, bitmapProperties));

            d2dContext.Target = BitmapTarget2D;
            d2dContext.TextAntialiasMode = SharpDX.Direct2D1.TextAntialiasMode.Grayscale;

            #endregion
        }

        void Window_SizeChanged(object sender, EventArgs args)
        {
            SizeChanged();
        }

        protected SharpDX.DXGI.SwapChainFullScreenDescription CreateFullScreenDescription()
        {
            return new SwapChainFullScreenDescription()
            {
                RefreshRate = new SharpDX.DXGI.Rational(60, 1),
                Scaling = DisplayModeScaling.Centered,
                Windowed = true,
            };
        }

        /// <summary>
        /// Present the back buffer of the swap chain.
        /// </summary>
        internal void Present() {
            try
            {
                swapChain.Present((VSync ? 1 : 0), PresentFlags.None, new PresentParameters());
            }
            catch(SharpDX.SharpDXException ex)
            {
                //If the driver was removed either by a disconnect or a driver upgrade, we must completely reinitialize the renderer.
                if (ex.ResultCode == SharpDX.DXGI.ResultCode.DeviceRemoved || ex.ResultCode == SharpDX.DXGI.ResultCode.DeviceReset)
                    InitDevice(Dpi);
                else
                    throw;
            }
        }

        /// <summary>
        /// Creates the swap chain description.
        /// </summary>
        /// <returns>A swap chain description</returns>
        /// <remarks>
        /// This method can be overloaded in order to modify default parameters.
        /// </remarks>
        protected SharpDX.DXGI.SwapChainDescription1 CreateSwapChainDescription() {
            return new SharpDX.DXGI.SwapChainDescription1()
            {
                Height = Height,
                Width = Width,
                Format = Format.B8G8R8A8_UNorm,
                Stereo = false,
                Flags = SwapChainFlags.AllowModeSwitch,
                BufferCount = 1,
                SampleDescription = new SampleDescription(4, 0),
                Scaling = Scaling.Stretch,
                Usage = Usage.BackBuffer | Usage.RenderTargetOutput,
                SwapEffect = SwapEffect.Discard,
            };
        }

        /// <summary>
        /// Creates the swap chain.
        /// </summary>
        /// <param name="factory">The DXGI factory</param>
        /// <param name="device">The D3D11 device</param>
        /// <param name="desc">The swap chain description</param>
        /// <returns>An instance of swap chain</returns>
        protected SwapChain1 CreateSwapChain(Factory2 dxgiFactory, SharpDX.Direct3D11.Device1 device, SwapChainDescription1 desc)
        {
            return new SwapChain1(dxgiFactory, device, windowHandle, ref desc, CreateFullScreenDescription(), null);
        }

        protected override void Dispose(bool disposeManagedResources)
        {
            if (disposeManagedResources)
            {
                if(SwapChain != null)
                {
                    //Make sure we are no longer in fullscreen mode or the disposing of Direct3D device will generate an exception.
                    SwapChain.IsFullScreen = false;
                }
            }
            base.Dispose(disposeManagedResources);
        }
    }
}
