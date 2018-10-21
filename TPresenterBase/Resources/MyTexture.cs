using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.WIC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buffer = SharpDX.Direct3D11.Buffer;
using Resource = SharpDX.Direct3D11.Resource;

namespace TPresenter.Render.Resources
{
    // Temparary class. Will be replaced with proper texture classes. Render debug only.
    static class MyTexture
    {
        internal static Texture2D GetTextureFromFile(string textureName)
        {
            BitmapSource bitmapSource = LoadBitmapFromFile(new ImagingFactory2(), textureName);
            int stride = bitmapSource.Size.Width * 4;
            using (var buffer = new DataStream(bitmapSource.Size.Height * stride, true, true))
            {
                bitmapSource.CopyPixels(stride, buffer);
                return new Texture2D(
                    Render11.Direct3DDevice,
                    GetTextureDescription(bitmapSource),
                    new DataRectangle(buffer.DataPointer, stride));
            }
        }

        internal static BitmapSource LoadBitmapFromFile(ImagingFactory2 factory, string filename)
        {
            var bitmapDecoder = new BitmapDecoder(factory, filename, DecodeOptions.CacheOnDemand);
            var formatConverter = new FormatConverter(factory);
            formatConverter.Initialize(
                bitmapDecoder.GetFrame(0),
                //PixelFormat.Format32bppPBGRA,
                PixelFormat.Format32bppRGBA,
                BitmapDitherType.None,
                null,
                0.0,
                BitmapPaletteType.Custom);

            return formatConverter;
        }

        internal static Texture2DDescription GetTextureDescription(BitmapSource bitmapSource)
        {
            return new Texture2DDescription()
            {
                Width = bitmapSource.Size.Width,
                Height = bitmapSource.Size.Height,
                ArraySize = 1,
                BindFlags = BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
            };
        }
    }
}
