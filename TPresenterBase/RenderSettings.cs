using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPresenter.Render
{
    public enum WindowModeEnum : byte
    {
        Window = 0,
        FullscreenWindow = 1,
        Fullscreen = 2,
    }

    public struct RenderDeviceSettings : IEquatable<RenderDeviceSettings>
    {
        public WindowModeEnum WindowMode;
        public int BackBufferWidth;
        public int BackBufferHeight;

        public bool Equals(RenderDeviceSettings other)
        {
            throw new NotImplementedException();
        }
    }
}
