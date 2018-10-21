using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using SharpDX.Windows;
using SharpDX.DXGI;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;

namespace TPresenter.Render
{
    public partial class Render11
    {
        #region Fields And Properties

        //Direct2D Objects
        protected static SharpDX.Direct2D1.DeviceContext d2dContext;
        protected SharpDX.Direct2D1.Device d2dDevice;
        protected SharpDX.Direct2D1.Factory1 d2dFactory;

        //Direct3D Objects
        private static MyRenderContext d3dContext;
        protected static SharpDX.Direct3D11.Device1 d3dDevice;
        protected float dpi;

        //DirectWrite and Windows Imaging Component Objects
        protected static SharpDX.DirectWrite.Factory dwriteFactory;
        protected SharpDX.WIC.ImagingFactory2 wicFactory;

        /// <summary>
        /// The list of feature levels to accept.
        /// </summary>
        public FeatureLevel[] Direct3DFeatureLevels = new FeatureLevel[]
        {
            FeatureLevel.Level_11_1,
            FeatureLevel.Level_11_0,
        };

        /// <summary>
        /// Gets Direct2D context.
        /// </summary>
        public static SharpDX.Direct2D1.DeviceContext Direct2DContext { get { return d2dContext; } }

        /// <summary>
        /// Gets Direct2D device.
        /// </summary>
        public SharpDX.Direct2D1.Device Direct2DDevice { get { return d2dDevice; } }

        /// <summary>
        /// Gets Direct2D factory.
        /// </summary>
        public SharpDX.Direct2D1.Factory1 Direct2DFactory { get { return d2dFactory; } }

        /// <summary>
        /// Gets Direct3D11 context.
        /// </summary>
        internal static MyRenderContext Direct3DContext
        {
            get { return d3dContext; }
            set { d3dContext = value; }
        }

        /// <summary>
        /// Gets Direct3D11 device.
        /// </summary>
        public static SharpDX.Direct3D11.Device1 Direct3DDevice { get { return d3dDevice; } }

        /// <summary>
        /// Gets DirectWrite factory.
        /// </summary>
        public static SharpDX.DirectWrite.Factory DirectWriteFactory { get { return dwriteFactory; } }

        /// <summary>
        /// Gets Windows Imaging Component factory.
        /// </summary>
        public SharpDX.WIC.ImagingFactory2 WICFactory { get { return wicFactory; } }

        /// <summary>
        /// Gets or sets the DPI.
        /// </summary>
        /// <remark>
        /// This methid will fire the event <see cref="OnDpiChanged"/> if the dpi is modified.
        /// </remark>
        public float Dpi
        {
            get { return dpi; }
            set
            {
                if(dpi != value)
                {
                    dpi = value;
                    d2dContext.DotsPerInch = new SharpDX.Size2F(dpi, dpi);
                    OnDpiChanged?.Invoke();
                }
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// This event is fired when the <see cref="Dpi"/> is called,
        /// </summary>
        public Action OnDpiChanged;

        /// <summary>
        /// This event is fired when the device manager is initialized by the <see cref="InitDevice"/> method.
        /// </summary>
        public Action OnInitialize;

        #endregion

        /// <summary>
        /// Initialize this instance.
        /// </summary>
        /// <param name="window">Window to receive the rendering</param>
        public void InitDevice(float dpi = 96.0f) {
            CreateInstances();
            OnInitialize?.Invoke();
            Dpi = dpi;
        }

        /// <summary>
        /// Creates device manager objects
        /// </summary>
        /// <remarks>
        /// This method is called at the initialization of this instance.
        /// </remarks>
        protected void CreateInstances() {
            RemoveAndDispose(ref d3dContext);
            RemoveAndDispose(ref d3dDevice);
            RemoveAndDispose(ref d2dContext);
            RemoveAndDispose(ref d2dDevice);
            RemoveAndDispose(ref d2dFactory);
            RemoveAndDispose(ref dwriteFactory);
            RemoveAndDispose(ref wicFactory);

            #region Create Direct3D 11.1 Device and retrive device context.
            // Bgra performs better especially with Direct2D software
            // render targets
            var creationFlags = DeviceCreationFlags.BgraSupport;
#if DEBUG
            // Enable D3D device debug layer
            creationFlags |= DeviceCreationFlags.Debug;
#endif

            using (var device = new SharpDX.Direct3D11.Device(DriverType.Hardware, creationFlags, Direct3DFeatureLevels))
            {
                d3dDevice = ToDispose(device.QueryInterface<SharpDX.Direct3D11.Device1>());
            }

            //Get Direct3D 11.1 context
            Direct3DContext = new MyRenderContext();
            Direct3DContext.Initialize(d3dDevice.ImmediateContext);
            #endregion

            #region Create Direct2D device and context

#if DEBUG
            var debugLevel = SharpDX.Direct2D1.DebugLevel.Information;
#else
            var debugLevel = SharpDX.Direct2D1.DebugLevel.None;
#endif

            d2dFactory = ToDispose(new SharpDX.Direct2D1.Factory1(SharpDX.Direct2D1.FactoryType.SingleThreaded, debugLevel));
            dwriteFactory = ToDispose(new SharpDX.DirectWrite.Factory(SharpDX.DirectWrite.FactoryType.Shared));
            wicFactory = ToDispose(new SharpDX.WIC.ImagingFactory2());

            //Create Direct2D device
            using(var dxgiDevice = d3dDevice.QueryInterface<SharpDX.DXGI.Device>())
            {
                d2dDevice = ToDispose(new SharpDX.Direct2D1.Device(d2dFactory, dxgiDevice));
            }

            //Create Direct2D context
            d2dContext = ToDispose(new SharpDX.Direct2D1.DeviceContext(d2dDevice, SharpDX.Direct2D1.DeviceContextOptions.None));
            #endregion
        }
    }
}
