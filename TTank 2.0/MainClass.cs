using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;
using Device = SharpDX.Direct3D11.Device;
using Device1 = SharpDX.Direct3D11.Device1;
using TPresenter.Render.RenderProxy;
using TTank20.Game;

namespace TTank2
{
    class MainClass
    {
        [STAThread]
        static void Main()
        {
            InitializeRender();

            using(var game = new TTankGame())
            {
                game.Run();
            }
        }

        private static void InitializeRender()
        {
            //MyRenderProxy.Initialize();
        }
    }
}
