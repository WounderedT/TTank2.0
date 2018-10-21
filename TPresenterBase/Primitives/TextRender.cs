using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using SharpDX.Windows;
using SharpDX.DirectWrite;
using SharpDX.Direct2D1;
using Matrix = SharpDX.Matrix;
using TextAntialiasMode = SharpDX.Direct2D1.TextAntialiasMode;
using TPresenter.Render;

namespace TPrenseter.Render
{
    /// <summary>
    /// Renders text using Direct2D to the back buffer
    /// </summary>
    class TextRender: IDisposable
    {
        TextFormat textFormat;
        Brush sceneColorBrush;
        protected string font;
        protected Color4 color;
        protected int lineLength;
        internal bool isDisposed = false;

        internal DeviceContext renderContext { get { return Render11.Direct2DContext; } }
        internal Point Location { get; set; }
        internal string Text { get; set; }
        internal int Size { get; set; }

        internal TextRender(string font, Color4 color, Point location, int size = 16, int lineLength = 500)
        {
            if (!String.IsNullOrEmpty(font))
                this.font = font;
            else
                this.font = "Calibri";

            this.color = color;
            this.Location = location;
            this.Size = size;
            this.lineLength = lineLength;
            Init();
        }

        /// <summary>
        /// Create any device resources
        /// </summary>
        internal void Init()
        {
            sceneColorBrush = new SolidColorBrush(Render11.Direct2DContext, this.color);
            textFormat = new TextFormat(Render11.DirectWriteFactory, font, Size);

            Render11.Direct2DContext.TextAntialiasMode = TextAntialiasMode.Grayscale;
        }

        /// <summary>
        /// Render
        /// </summary>
        /// <param name="target">The target to render to (the same device manager must be used in both)</param>
        internal void Draw()
        {
            if (String.IsNullOrEmpty(Text))
                return;

            renderContext.BeginDraw();
            renderContext.Transform = MatrixToMatrix3x2(Matrix.Identity);
            renderContext.DrawText(Text, textFormat, new RectangleF(Location.X, Location.Y, Location.X + lineLength, Location.Y + Size), sceneColorBrush);

            renderContext.EndDraw();
        }

        public void Dispose()
        {
            if (isDisposed)
                return;

            textFormat.Dispose();
            sceneColorBrush.Dispose();

            isDisposed = true;
        }

        //Should be placed in the Math class
        private SharpDX.Mathematics.Interop.RawMatrix3x2 MatrixToMatrix3x2(Matrix matrix)
        {
            return new SharpDX.Mathematics.Interop.RawMatrix3x2
            {
                M11 = matrix.M11,
                M12 = matrix.M12,
                M21 = matrix.M21,
                M22 = matrix.M22,
                M31 = matrix.M41,
                M32 = matrix.M42
            };
        }
    }
}
