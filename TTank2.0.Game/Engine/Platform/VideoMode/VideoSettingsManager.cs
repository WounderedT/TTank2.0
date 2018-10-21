using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPresenter.Render;

namespace TTank20.Game.Engine.Platform.VideoMode
{
    public static class VideoSettingsManager
    {
        static VideoSettingsManager()
        {
            //Prepare basing video settings.
        }

        public static RenderDeviceSettings Initialize()
        {
            return GetDefaults();
        }

        internal static RenderDeviceSettings GetDefaults()
        {
            var settings = new RenderDeviceSettings();
            settings.BackBufferWidth = 1024;
            settings.BackBufferHeight = 768;
            settings.WindowMode = WindowModeEnum.Window;
            return settings;
        }
    }
}
