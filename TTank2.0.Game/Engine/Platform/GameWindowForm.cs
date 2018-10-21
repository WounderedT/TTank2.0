using TTank20Game;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TPresenter.Library.Win32;
using TPresenter.Render;
using TPresenter.Render.ExternalApp;
using TPresenter.Render.RenderProxy;
using TTank20.Game;

namespace TPresenter.Render
{
    public class GameWindowForm : Form, IMessageFilter, IBufferedInputSource, TPresenter.Render.ExternalApp.IGameWindowForm
    {
        #region Fields and Properties

        private bool allowUserResizing;
        private bool captureMouse = true;
        private bool isCursorVisible = true;
        private bool showCursor = true;
        private MouseEventArgs emptyMouseEventArgs = new MouseEventArgs(0, 0, 0, 0, 0);
        private List<char> bufferedChars = new List<char>();
        private Vector2 mousePosition;

        public HashSet<int> BypassedMessages { get; private set; }

        public bool IsActive { get; private set; }

        public bool IsCursorVisible
        {
            get { return isCursorVisible; }
            set
            {
                if(!isCursorVisible && value)
                {
                    Cursor.Show();
                    isCursorVisible = value;
                }
                else if(isCursorVisible && !value)
                {
                    Cursor.Hide();
                    isCursorVisible = value;
                }
            }
        }

        public bool ShowCursor
        {
            get { return showCursor; }
            set
            {
                if(showCursor != value)
                {
                    showCursor = value;
                    IsCursorVisible = value;
                }
            }
        }

        public bool DrawEnabled
        {
            get { return WindowState != FormWindowState.Minimized; }
        }

        Vector2 IBufferedInputSource.MousePosition
        {
            get { return mousePosition; }
        }

        Vector2 IBufferedInputSource.MouseAreaSize
        {
            get { return new Vector2(ClientSize.Width, ClientSize.Height); }
        }

        #endregion

        #region Public Methods

        public GameWindowForm() : base()
        {
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;

            BypassedMessages = new HashSet<int>();
            BypassedMessages.Add((int)WinApi.WM.KEYDOWN);
            BypassedMessages.Add((int)WinApi.WM.KEYUP);
            BypassedMessages.Add((int)WinApi.WM.CHAR);
            BypassedMessages.Add((int)WinApi.WM.DEADCHAR);
            BypassedMessages.Add((int)WinApi.WM.SYSKEYDOWN);
            BypassedMessages.Add((int)WinApi.WM.SYSKEYUP);
            BypassedMessages.Add((int)WinApi.WM.SYSCHAR);
            BypassedMessages.Add((int)WinApi.WM.SYSDEADCHAR);

            BypassedMessages.Add((int)WinApi.WM.MOUSEWHEEL);
            BypassedMessages.Add((int)WinApi.WM.MOUSEMOVE);
            BypassedMessages.Add((int)WinApi.WM.LBUTTONDOWN);
            BypassedMessages.Add((int)WinApi.WM.LBUTTONUP);
            BypassedMessages.Add((int)WinApi.WM.LBUTTONDBLCLK);
            BypassedMessages.Add((int)WinApi.WM.RBUTTONDOWN);
            BypassedMessages.Add((int)WinApi.WM.RBUTTONUP);
            BypassedMessages.Add((int)WinApi.WM.RBUTTONDBLCLK);
            BypassedMessages.Add((int)WinApi.WM.MBUTTONDOWN);
            BypassedMessages.Add((int)WinApi.WM.MBUTTONUP);
            BypassedMessages.Add((int)WinApi.WM.MBUTTONDBLCLK);
            BypassedMessages.Add((int)WinApi.WM.XBUTTONDBLCLK);
            BypassedMessages.Add((int)WinApi.WM.XBUTTONDOWN);
            BypassedMessages.Add((int)WinApi.WM.XBUTTONUP);

            BypassedMessages.Add((int)WinApi.WM.ERASEBKGND);
            BypassedMessages.Add((int)WinApi.WM.SHOWWINDOW);
            BypassedMessages.Add((int)WinApi.WM.ACTIVATE);
            BypassedMessages.Add((int)WinApi.WM.SETFOCUS);
            BypassedMessages.Add((int)WinApi.WM.KILLFOCUS);

            BypassedMessages.Add((int)WinApi.WM.IME_NOTIFY);
        }

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == (int)WinApi.WM.MOUSEMOVE)
                return false;

            if (m.Msg == (int)WinApi.WM.CHAR)
                return false;

            if (m.Msg == (int)WinApi.WM.SYSKEYDOWN)
                return true;

            if (m.Msg == (int)WinApi.WM.SYSKEYUP)
                return true;

            if (m.Msg == (int)WinApi.WM.SYSCHAR)
                return true;

            if (m.Msg == (int)WinApi.WM.SYSDEADCHAR)
                return true;

            if (m.Msg == (int)WinApi.WM.KEYUP)
                return true;

            if (m.Msg == (int)WinApi.WM.KEYDOWN)
                return true;

            if (m.Msg == (int)WinApi.WM.SYSCOMMAND)
            {
                //http://msdn.microsoft.com/en-us/library/windows/desktop/ms646360(v=vs.85).aspx
                WinApi.SystemCommands correctWParam = (WinApi.SystemCommands)((int)m.WParam & 0xFFF0);
                if (correctWParam == WinApi.SystemCommands.SC_MOUSEMENU)
                    return true;
            }

            if (m.Msg == (int)WinApi.WM.NCRBUTTONDOWN)
                return true;

            if (m.Msg == (int)WinApi.WM.ACTIVATE)
                MyRenderProxy.HandleFocusMessage(WindowFocusMessage.Activate);

            if (m.Msg == (int)WinApi.WM.SETFOCUS)
                MyRenderProxy.HandleFocusMessage(WindowFocusMessage.SetFocus);

            if (BypassedMessages.Contains(m.Msg))
            {
                if(m.Msg == (int)WinApi.WM.ACTIVATE)
                {
                    if (m.WParam == IntPtr.Zero)
                        OnDeactivate(EventArgs.Empty);
                    else
                        OnActivated(EventArgs.Empty);
                }
                if (m.Msg == (int)WinApi.WM.MOUSEMOVE)
                    OnMouseMove(emptyMouseEventArgs);

                m.Result = WinApi.DefWindowProc(m.HWnd, m.Msg, m.WParam, m.LParam);
                return true;
            }

            return false;
        }

        public void UpdateClip()
        {
            TTankGame.WindowHandle = Handle;
            Control control = FromHandle(WinApi.GetForegroundWindow());

            bool isActive = false;

            if (control != null)
                isActive = !control.TopLevelControl.InvokeRequired && Handle == control.TopLevelControl.Handle;

            isActive = isActive && (captureMouse || isCursorVisible);

            if (isActive)
                SetClip();
            else
                ClearClip();
        }

        public void SetMouseCapture(bool capture)
        {
            captureMouse = capture;
            UpdateClip();
        }

        public void OnModeChanged(WindowModeEnum windowMode, int width, int height)
        {
            if(windowMode == WindowModeEnum.Window)
            {
                FormBorderStyle = FormBorderStyle.FixedSingle;
                TopMost = false;
            }
            else if(windowMode == WindowModeEnum.FullscreenWindow)
            {
                FormBorderStyle = FormBorderStyle.None;
                TopMost = false;
                SizeGripStyle = SizeGripStyle.Hide;
            }
            else if(windowMode == WindowModeEnum.Fullscreen)
            {
                FormBorderStyle = FormBorderStyle.None;
            }

            ClientSize = new System.Drawing.Size(width, height);
            WinApi.DEVMODE mode = new WinApi.DEVMODE();
            WinApi.EnumDisplaySettings(null, WinApi.ENUM_CURRENT_SETTINGS, ref mode);

            Location = new System.Drawing.Point(mode.dmPelsWidth / 2 - width / 2, mode.dmPelsHeight / 2 - height / 2);

            Show();
            Activate();
            TTankGame.Static.UpdateMouseCapture();
        }

        public void BeforeDraw()
        {
            UpdateClip();
        }

        #endregion

        void IBufferedInputSource.SwapBufferedTextInput(ref List<char> swappedBuffer)
        {
            swappedBuffer.Clear();
            //TODO: Add a lock to prevent multithreding issues.
            var tmp = swappedBuffer;
            swappedBuffer = bufferedChars;
            bufferedChars = tmp;
        }

        #region Events

        protected override void OnActivated(EventArgs e)
        {
            if (!IsActive)
            {
                IsActive = true;
                if (!ShowCursor)
                    IsCursorVisible = false;
            }
            base.OnActivated(e);
        }

        protected override void OnDeactivate(EventArgs e)
        {
            if (IsActive)
            {
                IsActive = false;
                if (!IsCursorVisible)
                    IsCursorVisible = true;
            }
            base.OnDeactivate(e);
        }

        protected override void OnResizeBegin(EventArgs e)
        {
            ClearClip();
            base.OnResizeBegin(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            MessageFilterHook.AddMessageFilter(Handle, this);
            base.OnLoad(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            MessageFilterHook.RemoveMessageFilter(Handle, this);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            var size = Size;
            var screen = Screen.GetWorkingArea(this);
            var location = Location;

            if (size.Height > screen.Height)
                Location = new System.Drawing.Point(Location.X, screen.Height - size.Height);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == (int)WinApi.WM.SYSKEYDOWN)
                return;

            if(m.Msg == (int)WinApi.WM.CHAR)
            {
                char input = (char)m.WParam;
                //TODO: Add a lock to prevent multithreading issues.
                bufferedChars.Add(input);
                return;
            }

            if(m.Msg == (int)WinApi.WM.MOUSEMOVE)
            {
                mousePosition.X = unchecked((short)(long)m.LParam);
                mousePosition.Y = unchecked((short)(long)m.LParam >> 16);
            }

            base.WndProc(ref m);
            //MessageLoop.AddMessage(ref m);
        }

        #endregion

        #region Private Methods

        private void ClearClip()
        {
            Cursor.Clip = System.Drawing.Rectangle.Empty;
        }

        private void SetClip()
        {
            Cursor.Clip = RectangleToScreen(ClientRectangle);
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            ClientSize = new System.Drawing.Size(284, 262);
            Name = "GameWindowForm";
            ResumeLayout(false);
        }

        #endregion
    }
}
