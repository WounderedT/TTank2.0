using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TPresenter.Render.ExternalApp
{
    public interface IGameWindowForm
    {

        /// <summary>
        /// True when Present on device should be called (e.g. window is not minimized).
        /// </summary>
        bool DrawEnabled { get; }

        /// <summary>
        /// Target window handler.
        /// </summary>
        IntPtr Handle { get; }

        void BeforeDraw();

        void SetMouseCapture(bool capture);

        /// <summary>
        /// Calculated by render when display mode has changed.
        /// </summary>
        void OnModeChanged(WindowModeEnum mode, int width, int height);
    }
}
