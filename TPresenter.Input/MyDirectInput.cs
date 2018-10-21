using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using SharpDX.DirectInput;

namespace TPresenter.Input
{
    public static class MyDirectInput
    {
        static DirectInput directInput;
        static Mouse mouse;
        static MouseState mouseState = new MouseState();

        public static DirectInput DirectInput
        {
            get { return directInput; }
        }

        public static void Initialize(IntPtr handle)
        {
            try
            {
                directInput = new DirectInput();
                mouse = new Mouse(directInput);
                try
                {
                    mouse.SetCooperativeLevel(handle, CooperativeLevel.Foreground | CooperativeLevel.NonExclusive);
                }
                catch
                {
                    //log this issue    
                }
            }
            catch (SharpDX.SharpDXException ex)
            {
                //log this issue
                throw;
            }
        }

        public static void Close()
        {
            if(mouse != null)
            {
                mouse.Dispose();
                mouse = null;
            }

            if(directInput != null)
            {
                directInput.Dispose();
                directInput = null;
            }
        }

        public static MyMouseState GetMouseState()
        {
            if(mouse == null)
            {
                return new MyMouseState();
            }

            MyMouseState myMouseState = new MyMouseState();
            if (mouse.TryAcquire().Success)
            {
                try
                {
                    mouse.GetCurrentState(ref mouseState);
                    mouse.Poll();
                    myMouseState = new MyMouseState()
                    {
                        X = mouseState.X,
                        Y = mouseState.Y,
                        LeftButton = mouseState.Buttons[0],
                        RightButton = mouseState.Buttons[1],
                        MiddleButton = mouseState.Buttons[2],
                        XButton1 = mouseState.Buttons[3],
                        XButton2 = mouseState.Buttons[4],
                        ScrollWheelValue = mouseState.Z,
                    };
                }
                catch (SharpDXException) { }
            }

            return myMouseState;
        }
    }
}
