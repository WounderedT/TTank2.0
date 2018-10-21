using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPresenter.Render.ExternalApp
{
    public interface IBufferedInputSource
    {
        void SwapBufferedTextInput(ref List<char> swappedBuffer);

        Vector2 MousePosition { get; }
        Vector2 MouseAreaSize { get; }
    }
}
