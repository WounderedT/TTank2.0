using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using SharpDX.Windows;
using SharpDX.DirectWrite;
using SharpDX.Direct2D1;
using TPresenter.Render;

namespace TPrenseter.Render
{
    static class DebugMessageRender
    {
        const int MAXSAMPLES = 100;

        static TextRender debugText;
        static int tickindex = 0;
        static long ticksum = 0;
        static long[] ticklist = new long[MAXSAMPLES];
        static Stopwatch clock;
        static long frameCount;
        static bool isDisposed = false;
        static StringBuilder text;

        internal static void Init()
        {
            debugText = new TextRender("Calibri", Color.DarkOrange, new SharpDX.Point(8, 8), 12);
            clock = Stopwatch.StartNew();
            text = new StringBuilder();
        }

        internal static void Draw()
        {
            frameCount++;
            var averageTick = CalcAverageTick(clock.ElapsedTicks) / Stopwatch.Frequency;

            text.AppendLine(string.Format("{0:F2} FPS ({1:F1} ms)", 1.0 / averageTick, averageTick * 1000.0));
            text.AppendLine(string.Format("View: ({0})", Render11.Environment.Matrices.View.TranslationVector));
            text.AppendLine(string.Format("Position: ({0})", Render11.Environment.Matrices.CameraPosition));
            text.AppendLine(string.Format("Orientation Right: ({0})\n\t    Up: ({1})\n\t    Forward: ({2})",
                Render11.cameraOrientation.Right, Render11.cameraOrientation.Up, Render11.cameraOrientation.Forward));
            text.AppendLine(string.Format("Device ptr: ({0})",Render11.Direct3DDevice.NativePointer));
            text.AppendLine("Axis:\n\t    X: Red\n\t    Y: Green\n\t    Z: Blue");
            text.AppendLine("Press F12 to reset camera.");
            debugText.Text = text.ToString();

            debugText.Draw();

            clock.Restart();
            text.Clear();
        }

        internal static void Dispose()
        {
            if (isDisposed)
                return;

            debugText.Dispose();
            isDisposed = true;
        }

        /* need to zero out the ticklist array before starting */
        /* average will ramp up until the buffer is full */
        /* returns average ticks per frame over the MAXSAMPPLES last frames */
        //http://stackoverflow.com/questions/87304/calculating-frames-per-second-in-a-game/87732#87732
        private static double CalcAverageTick(long newtick)
        {
            ticksum -= ticklist[tickindex];  /* subtract value falling off */
            ticksum += newtick;              /* add new value */
            ticklist[tickindex] = newtick;   /* save new value so it can be subtracted later */
            if (++tickindex == MAXSAMPLES)    /* inc buffer index */
                tickindex = 0;

            /* return average */
            if (frameCount < MAXSAMPLES)
                return (double)ticksum / frameCount;
            else
                return (double)ticksum / MAXSAMPLES;
        }
    }
}
