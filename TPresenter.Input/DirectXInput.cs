using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TPresenter.Game;
using TPresenter.Input;
using TPresenter.Library.Win32;
using TPresenter.Render.ExternalApp;
using TPresenter.Utils;

using Control = TPresenter.Input.Control;

namespace TPresenter.Input
{
    public partial class DirectXInput
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        //An unmanaged function that retrieves the state of each key
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        static extern short GetKeyState(int keyCode);

        Vector2 absoluteMousePosition;
        MyMouseState previousMouseState;
        MyMouseState actualMouseState;
        LocalizedKeyboardState keyboardState;

        MyMouseState actualMouseStateRaw;

        bool mouseXIsInverted;
        bool mouseYIsInverted;
        float mouseSensetivity;

        bool gameWasFocused = false;

        public bool ENABLE_DEVELOPER_KEYS { get; private set; }
        public MyMouseState ActualMouseState { get { return actualMouseState; } }
        public bool IsMouseXInvertedDefault { get { return false; } }
        public bool IsMouseYInvertedDefault { get { return false; } }
        public float MouseSensetivityDefault { get { return 1.655f; } }

        KeyHasher hasher = new KeyHasher();

        //Control lists
        Dictionary<StringId, Control> defaultGameControlList;
        Dictionary<StringId, Control> gameControlList = new Dictionary<StringId, Control>(StringId.Comparer);
        Dictionary<StringId, Control> gameControlSnapshot = new Dictionary<StringId, Control>(StringId.Comparer);
        HashSet<StringId> gameControlBlackList = new HashSet<StringId>(StringId.Comparer);

        //List of valid keys and buttons
        List<Keys> validKeyboardKeys = new List<Keys>();
        List<MouseButtonsEnum> validMouseButtons = new List<MouseButtonsEnum>();

        List<Keys> digitKeys = new List<Keys>();

        Array allKeys = Enum.GetValues(typeof(Keys));

        IBufferedInputSource bufferedInputSource;
        IControlNameLookup controlNameLookup;
        List<char> currentTextInpt = new List<char>();
        List<Keys> tmpPressedKeys = new List<Keys>();
        IntPtr windowHandle;

        public IntPtr WindowHandle { get { return windowHandle; } }
        public List<char> TextInput { get { return currentTextInpt; } }

        public DirectXInput(IBufferedInputSource bufferedInputSource, IControlNameLookup nameLookup, Dictionary<StringId, Control> gameControls, bool enableDevKeys)
        {
            this.bufferedInputSource = bufferedInputSource;
            controlNameLookup = nameLookup;
            defaultGameControlList = gameControls;
            gameControlList = new Dictionary<StringId, Control>(StringId.Comparer);
            gameControlSnapshot = new Dictionary<StringId, Control>(StringId.Comparer);
            CloneControls(defaultGameControlList, gameControlList);
            ENABLE_DEVELOPER_KEYS = enableDevKeys;

            //Temporary init. Should be done by LoadData.
            mouseXIsInverted = IsMouseXInvertedDefault;
            mouseYIsInverted = IsMouseYInvertedDefault;
            mouseSensetivity = MouseSensetivityDefault;
        }

        public void AddDefaultControl(StringId stringId, Control control)
        {
            gameControlList[stringId] = control;
            defaultGameControlList[stringId] = control;
        }

        public void LoadData(Dictionary<string, object> controlsGeneral, Dictionary<string, object> controlsButtons)
        {
            mouseXIsInverted = IsMouseXInvertedDefault;
            mouseYIsInverted = IsMouseYInvertedDefault;
            mouseSensetivity = MouseSensetivityDefault;

            #region Digit keys only

            digitKeys.Add(Keys.D0);
            digitKeys.Add(Keys.D1);
            digitKeys.Add(Keys.D2);
            digitKeys.Add(Keys.D3);
            digitKeys.Add(Keys.D4);
            digitKeys.Add(Keys.D5);
            digitKeys.Add(Keys.D6);
            digitKeys.Add(Keys.D7);
            digitKeys.Add(Keys.D8);
            digitKeys.Add(Keys.D9);
            digitKeys.Add(Keys.NumPad0);
            digitKeys.Add(Keys.NumPad1);
            digitKeys.Add(Keys.NumPad2);
            digitKeys.Add(Keys.NumPad3);
            digitKeys.Add(Keys.NumPad4);
            digitKeys.Add(Keys.NumPad5);
            digitKeys.Add(Keys.NumPad6);
            digitKeys.Add(Keys.NumPad7);
            digitKeys.Add(Keys.NumPad8);
            digitKeys.Add(Keys.NumPad9);

            #endregion

            #region Lists of assignable keys and buttons

            //  List of assignable keyboard keys
            validKeyboardKeys.Add(Keys.A);
            validKeyboardKeys.Add(Keys.Add);
            validKeyboardKeys.Add(Keys.B);
            validKeyboardKeys.Add(Keys.Back);
            validKeyboardKeys.Add(Keys.C);
            validKeyboardKeys.Add(Keys.CapsLock);
            validKeyboardKeys.Add(Keys.D);
            validKeyboardKeys.Add(Keys.D0);
            validKeyboardKeys.Add(Keys.D1);
            validKeyboardKeys.Add(Keys.D2);
            validKeyboardKeys.Add(Keys.D3);
            validKeyboardKeys.Add(Keys.D4);
            validKeyboardKeys.Add(Keys.D5);
            validKeyboardKeys.Add(Keys.D6);
            validKeyboardKeys.Add(Keys.D7);
            validKeyboardKeys.Add(Keys.D8);
            validKeyboardKeys.Add(Keys.D9);
            validKeyboardKeys.Add(Keys.Decimal);
            validKeyboardKeys.Add(Keys.Delete);
            validKeyboardKeys.Add(Keys.Divide);
            validKeyboardKeys.Add(Keys.Down);
            validKeyboardKeys.Add(Keys.E);
            validKeyboardKeys.Add(Keys.End);
            validKeyboardKeys.Add(Keys.Enter);
            validKeyboardKeys.Add(Keys.F);
            validKeyboardKeys.Add(Keys.G);
            validKeyboardKeys.Add(Keys.H);
            validKeyboardKeys.Add(Keys.Home);
            validKeyboardKeys.Add(Keys.I);
            validKeyboardKeys.Add(Keys.Insert);
            validKeyboardKeys.Add(Keys.J);
            validKeyboardKeys.Add(Keys.K);
            validKeyboardKeys.Add(Keys.L);
            validKeyboardKeys.Add(Keys.Left);
            validKeyboardKeys.Add(Keys.LeftAlt);
            validKeyboardKeys.Add(Keys.LeftControl);
            validKeyboardKeys.Add(Keys.LeftShift);
            validKeyboardKeys.Add(Keys.M);
            validKeyboardKeys.Add(Keys.Multiply);
            validKeyboardKeys.Add(Keys.N);
            validKeyboardKeys.Add(Keys.None);
            validKeyboardKeys.Add(Keys.NumPad0);
            validKeyboardKeys.Add(Keys.NumPad1);
            validKeyboardKeys.Add(Keys.NumPad2);
            validKeyboardKeys.Add(Keys.NumPad3);
            validKeyboardKeys.Add(Keys.NumPad4);
            validKeyboardKeys.Add(Keys.NumPad5);
            validKeyboardKeys.Add(Keys.NumPad6);
            validKeyboardKeys.Add(Keys.NumPad7);
            validKeyboardKeys.Add(Keys.NumPad8);
            validKeyboardKeys.Add(Keys.NumPad9);
            validKeyboardKeys.Add(Keys.O);
            validKeyboardKeys.Add(Keys.OemCloseBrackets);
            validKeyboardKeys.Add(Keys.OemComma);
            validKeyboardKeys.Add(Keys.OemMinus);
            validKeyboardKeys.Add(Keys.OemOpenBrackets);
            validKeyboardKeys.Add(Keys.OemPeriod);
            validKeyboardKeys.Add(Keys.OemPipe);
            validKeyboardKeys.Add(Keys.OemPlus);
            validKeyboardKeys.Add(Keys.OemQuestion);
            validKeyboardKeys.Add(Keys.OemQuotes);
            validKeyboardKeys.Add(Keys.OemSemicolon);
            validKeyboardKeys.Add(Keys.OemTilde);
            validKeyboardKeys.Add(Keys.OemBackslash);
            validKeyboardKeys.Add(Keys.P);
            validKeyboardKeys.Add(Keys.PageDown);
            validKeyboardKeys.Add(Keys.PageUp);
            validKeyboardKeys.Add(Keys.Pause);
            validKeyboardKeys.Add(Keys.Q);
            validKeyboardKeys.Add(Keys.R);
            validKeyboardKeys.Add(Keys.Right);
            validKeyboardKeys.Add(Keys.RightAlt);
            validKeyboardKeys.Add(Keys.RightControl);
            validKeyboardKeys.Add(Keys.RightShift);
            validKeyboardKeys.Add(Keys.S);
            validKeyboardKeys.Add(Keys.Space);
            validKeyboardKeys.Add(Keys.Subtract);
            validKeyboardKeys.Add(Keys.T);
            validKeyboardKeys.Add(Keys.Tab);
            validKeyboardKeys.Add(Keys.U);
            validKeyboardKeys.Add(Keys.Up);
            validKeyboardKeys.Add(Keys.V);
            validKeyboardKeys.Add(Keys.W);
            validKeyboardKeys.Add(Keys.X);
            validKeyboardKeys.Add(Keys.Y);
            validKeyboardKeys.Add(Keys.Z);
            validKeyboardKeys.Add(Keys.F1);
            validKeyboardKeys.Add(Keys.F2);
            validKeyboardKeys.Add(Keys.F3);
            validKeyboardKeys.Add(Keys.F4);
            validKeyboardKeys.Add(Keys.F5);
            validKeyboardKeys.Add(Keys.F6);
            validKeyboardKeys.Add(Keys.F7);
            validKeyboardKeys.Add(Keys.F8);
            validKeyboardKeys.Add(Keys.F9);
            validKeyboardKeys.Add(Keys.F10);
            validKeyboardKeys.Add(Keys.F11);
            validKeyboardKeys.Add(Keys.F12);

            //  List of assignable mouse buttons
            validMouseButtons.Add(MouseButtonsEnum.Left);
            validMouseButtons.Add(MouseButtonsEnum.Middle);
            validMouseButtons.Add(MouseButtonsEnum.Right);
            validMouseButtons.Add(MouseButtonsEnum.XButton1);
            validMouseButtons.Add(MouseButtonsEnum.XButton2);
            validMouseButtons.Add(MouseButtonsEnum.None);

            #endregion

            CheckValidControls(defaultGameControlList);
            LoadControls(controlsGeneral, controlsButtons);
            TakeSnapshot();
            ClearBlacklist();
        }

        public void InitializeInput(IntPtr windowHandle)
        {
            this.windowHandle = windowHandle;
            WindowsMouse.SetWindow(windowHandle);

            MyDirectInput.Initialize(windowHandle);

            InitDevicePluginHandlerCallBack();

            keyboardState = new LocalizedKeyboardState();
        }

        public void UnloadData()
        {
            UninitDevicePluginHandlerCallback();
            MyDirectInput.Close();
        }

        void CheckValidControls(Dictionary<StringId, Control> controls)
        {
            foreach(Control control in controls.Values)
            {
                IsKeyValid(control.GetKeyboardControl());
                IsMouseButtonValid(control.GetMouseControl());
            }
        }

        private void InitDevicePluginHandlerCallBack()
        {
            MessageLoop.AddMessageHandler(WinApi.WM.DEVICECHANGE, DeviceChangeCallback);
        }

        private void DeviceChangeCallback(ref Message message)
        {

        }

        private void UninitDevicePluginHandlerCallback()
        {
            MessageLoop.RemoveMessageHandler(WinApi.WM.DEVICECHANGE, DeviceChangeCallback);
        }

        internal void ClearStates()
        {
            keyboardState.ClearStates();
            previousMouseState = actualMouseState;
            actualMouseState = new MyMouseState();
            actualMouseStateRaw.ClearPosition();
        }

        internal void UpdateStatesFromPlayback(MyKeyboardState currentKeyboard, MyKeyboardState previousKeyboard, MyMouseState currentMouse, MyMouseState previousMouse, int x, int y)
        {
            keyboardState.UpdateKeyboardStateFromSnapshot(currentKeyboard, previousKeyboard);
            previousMouseState = previousMouse;
            actualMouseState = currentMouse;
            absoluteMousePosition = new Vector2(x, y);
            if (gameWasFocused)
                WindowsMouse.SetPosition(x, y);
        }

        internal void UpdateStates()
        {
            previousMouseState = actualMouseState;
            keyboardState.UpdateStates();
            actualMouseStateRaw = MyDirectInput.GetMouseState();
            int wheel = actualMouseState.ScrollWheelValue + actualMouseStateRaw.ScrollWheelValue;
            actualMouseState = actualMouseStateRaw;
            actualMouseState.ScrollWheelValue = wheel;
            actualMouseStateRaw.ClearPosition();

            int x, y;
            WindowsMouse.GetPosition(out x, out y);
            absoluteMousePosition = new Vector2(x, y);
            hasher.Keys.Clear();
            GetPressedKeys(hasher.Keys);
            if(hasher.Keys.Count > 0)
                System.Console.WriteLine("");
        }

        public bool Update(bool gameFocused)
        {
            if (!gameWasFocused && gameFocused)
                UpdateStates();

            gameWasFocused = gameFocused;

            if(!gameFocused)
            {
                ClearStates();
                return false;
            }

            UpdateStates();

            bufferedInputSource.SwapBufferedTextInput(ref currentTextInpt);
            return true;
        }

        public bool IsAnyKeyPressed()
        {
            return keyboardState.IsAnyKeyPressed();
        }

        public bool IsAnyNewKeyPressed()
        {
            return (keyboardState.IsAnyKeyPressed() && !keyboardState.GetPreviousKeyboardState().IsAnyKeyPressed());
        }

        public bool IsAnyMousePressed()
        {
            return actualMouseState.LeftButton || actualMouseState.MiddleButton || actualMouseState.RightButton || actualMouseState.XButton1 || actualMouseState.XButton2;
        }

        public bool IsAnyNewMousePressed()
        {
            return IsNewLeftMousePressed() || IsNewMiddleMousePressed() || IsNewRightMousePressed() || IsNewXButton1MousePressed() || IsNewXButton2MousePressed();
        }

        public bool IsNewPrimaryButtonPressed()
        {
            return IsNewLeftMousePressed();   // or joystick primary button
        }

        public bool IsNewSecondaryButtonPressed()
        {
            return IsNewRightMousePressed();  // or joystick primary button
        }

        public bool IsNewPrimaryButtonReleased()
        {
            return IsNewLeftMouseReleased();    // or joystick primary button
        }

        public bool IsNewSecondaryButtonReleased()
        {
            return IsNewRightMouseReleased(); // or joystick secodary button

        }

        public bool IsPrimaryButtonPressed()
        {
            return IsLeftMousePressed();  // or joystick primary button.
        }

        public bool IsSecondaryButtonPressed()
        {
            return IsRightMousePressed(); // or joystick secondary button.
        }

        public bool IsPrimaryButtonReleased()
        {
            return IsLeftMouseReleased();   // or joystick primary button
        }

        public bool IsSecodaryButtonReleased()
        {
            return IsRightMouseReleased();  // or joystick secodary button
        }

        public bool IsNewButtonPressed(SharedButtonsEnum button)
        {
            switch (button)
            {
                case SharedButtonsEnum.Primary:
                    return IsNewPrimaryButtonPressed();
                case SharedButtonsEnum.Secondary:
                    return IsNewSecondaryButtonPressed();
                default:
                    return false;
            }
        }
        
        public bool IsButtonPressed(SharedButtonsEnum button)
        {
            switch (button)
            {
                case SharedButtonsEnum.Primary:
                    return IsPrimaryButtonPressed();
                case SharedButtonsEnum.Secondary:
                    return IsSecondaryButtonPressed();
                default:
                    return false;
            }
        }
        
        public bool IsNewButtonReleased(SharedButtonsEnum button)
        {
            switch (button)
            {
                case SharedButtonsEnum.Primary:
                    return IsNewPrimaryButtonReleased();
                case SharedButtonsEnum.Secondary:
                    return IsNewSecondaryButtonReleased();
                default:
                    return false;
            }
        }

        public bool IsButtonReleased(SharedButtonsEnum button)
        {
            switch (button)
            {
                case SharedButtonsEnum.Primary:
                    return IsPrimaryButtonReleased();
                case SharedButtonsEnum.Secondary:
                    return IsSecodaryButtonReleased();
                default:
                    return false;
            }
        }

        public bool IsAnyWinKeyPressed()
        {
            return IsKeyPressed(Keys.LeftWindows) || IsKeyPressed(Keys.RightWindows);
        }

        public bool IsAnyAltKeyPressed()
        {
            return IsKeyPressed(Keys.Alt) || IsKeyPressed(Keys.LeftAlt) || IsKeyPressed(Keys.RightAlt);
        }

        public bool IsAnyCtrlKeyPressed()
        {
            return IsKeyPressed(Keys.LeftControl) || IsKeyPressed(Keys.RightControl);
        }

        public bool IsAnyShiftKeyPressed()
        {
            return IsKeyPressed(Keys.LeftShift) || IsKeyPressed(Keys.RightShift);
        }

        public void GetPressedKeys(List<Keys> keys)
        {
            keyboardState.GetActualPressedKeys(keys);
        }

        public bool IsKeyPressed(Keys key)
        {
            return keyboardState.IsKeyDown(key);
        }

        public bool IsNewKeyPressed(Keys key)
        {
            return (keyboardState.IsKeyDown(key) && keyboardState.IsPreviousKeyUp(key));
        }

        public bool IsNewKeyReleased(Keys key)
        {
            return (keyboardState.IsKeyUp(key) && keyboardState.IsPreviousKeyDown(key));
        }

        public float GetDeveloperRoll()
        {
            float roll = 0f;
            roll += IsGameControlPressed(ControlSpace.ROLL_LEFT) ? -1f : 0f;
            roll += IsGameControlPressed(ControlSpace.ROLL_RIGHT) ? 1f : 0f;

            return roll;
        }

        #region Mouse Button States

        public bool IsMousePressed(MouseButtonsEnum button)
        {
            switch (button)
            {
                case MouseButtonsEnum.Left:
                    return IsLeftMousePressed();
                case MouseButtonsEnum.Middle:
                    return IsMiddleMousePressed();
                case MouseButtonsEnum.Right:
                    return IsRightMousePressed();
                case MouseButtonsEnum.XButton1:
                    return IsXButton1MousePressed();
                case MouseButtonsEnum.XButton2:
                    return IsXButton2MousePressed();
                default:
                    return false;
            }
        }

        public bool IsMouseReleased(MouseButtonsEnum button)
        {
            switch (button)
            {
                case MouseButtonsEnum.Left:
                    return IsLeftMouseReleased();
                case MouseButtonsEnum.Middle:
                    return IsMiddleMouseReleased();
                case MouseButtonsEnum.Right:
                    return IsRightMouseReleased();
                case MouseButtonsEnum.XButton1:
                    return IsXButton1MouseReleased();
                case MouseButtonsEnum.XButton2:
                    return IsXButton2MouseReleased();
                default:
                    return false;
            }
        }

        public bool WasMousePressed(MouseButtonsEnum button)
        {
            switch (button)
            {
                case MouseButtonsEnum.Left:
                    return WasLeftMousePressed();
                case MouseButtonsEnum.Middle:
                    return WasMiddleMousePressed();
                case MouseButtonsEnum.Right:
                    return WasRightMousePressed();
                case MouseButtonsEnum.XButton1:
                    return WasXButton1MousePressed();
                case MouseButtonsEnum.XButton2:
                    return WasXButton2MousePressed();
                default:
                    return false;
            }
        }

        public bool WasMouseReleased(MouseButtonsEnum button)
        {
            switch (button)
            {
                case MouseButtonsEnum.Left:
                    return WasLeftMouseReleased();
                case MouseButtonsEnum.Middle:
                    return WasMiddleMouseReleased();
                case MouseButtonsEnum.Right:
                    return WasRightMouseReleased();
                case MouseButtonsEnum.XButton1:
                    return WasXButton1MouseReleased();
                case MouseButtonsEnum.XButton2:
                    return WasXButton2MouseReleased();
                default:
                    return false;
            }
        }

        public bool IsNewMousePressed(MouseButtonsEnum button)
        {
            switch (button)
            {
                case MouseButtonsEnum.Left:
                    return IsNewLeftMousePressed();
                case MouseButtonsEnum.Middle:
                    return IsNewMiddleMousePressed();
                case MouseButtonsEnum.Right:
                    return IsNewRightMousePressed();
                case MouseButtonsEnum.XButton1:
                    return IsNewXButton1MousePressed();
                case MouseButtonsEnum.XButton2:
                    return IsNewXButton2MousePressed();
                default:
                    return false;
            }
        }

        public bool IsNewMouseReleased(MouseButtonsEnum button)
        {
            switch (button)
            {
                case MouseButtonsEnum.Left:
                    return IsNewLeftMouseReleased();
                case MouseButtonsEnum.Middle:
                    return IsNewMiddleMouseReleased();
                case MouseButtonsEnum.Right:
                    return IsNewRightMouseReleased();
                case MouseButtonsEnum.XButton1:
                    return IsNewXButton1MouseReleased();
                case MouseButtonsEnum.XButton2:
                    return IsNewXButton2MouseReleased();
                default:
                    return false;
            }
        }

        #endregion

        #region Left Mouse Button States

        public bool IsLeftMousePressed()
        {
            return actualMouseState.LeftButton;
        }

        public bool IsNewLeftMousePressed()
        {
            return IsLeftMousePressed() && WasLeftMouseReleased();
        }

        public bool WasLeftMousePressed()
        {
            return previousMouseState.LeftButton;
        }

        public bool IsLeftMouseReleased()
        {
            return actualMouseState.LeftButton == false;
        }

        public bool IsNewLeftMouseReleased()
        {
            return IsLeftMouseReleased() && WasLeftMousePressed();
        }

        public bool WasLeftMouseReleased()
        {
            return previousMouseState.LeftButton == false;
        }

        #endregion

        #region Middle Mouse Button States

        public bool IsMiddleMousePressed()
        {
            return actualMouseState.MiddleButton;
        }

        public bool IsNewMiddleMousePressed()
        {
            return IsMiddleMousePressed() && WasMiddleMouseReleased();
        }

        public bool WasMiddleMousePressed()
        {
            return previousMouseState.MiddleButton;
        }

        public bool IsMiddleMouseReleased()
        {
            return actualMouseState.MiddleButton == false;
        }

        public bool IsNewMiddleMouseReleased()
        {
            return IsMiddleMouseReleased() && WasMiddleMousePressed();
        }

        public bool WasMiddleMouseReleased()
        {
            return previousMouseState.MiddleButton == false;
        }

        #endregion

        #region Right Mouse Button States

        public bool IsRightMousePressed()
        {
            return actualMouseState.RightButton;
        }

        public bool IsNewRightMousePressed()
        {
            return IsRightMousePressed() && WasRightMouseReleased();
        }

        public bool WasRightMousePressed()
        {
            return previousMouseState.RightButton;
        }

        public bool IsRightMouseReleased()
        {
            return actualMouseState.RightButton == false;
        }

        public bool IsNewRightMouseReleased()
        {
            return IsRightMouseReleased() && WasRightMousePressed();
        }

        public bool WasRightMouseReleased()
        {
            return previousMouseState.RightButton == false;
        }

        #endregion

        #region XButton1 Mouse Button States

        public bool IsXButton1MousePressed()
        {
            return actualMouseState.XButton1;
        }

        public bool IsNewXButton1MousePressed()
        {
            return IsXButton1MousePressed() && WasXButton1MouseReleased();
        }

        public bool WasXButton1MousePressed()
        {
            return previousMouseState.XButton1;
        }

        public bool IsXButton1MouseReleased()
        {
            return actualMouseState.XButton1 == false;
        }

        public bool IsNewXButton1MouseReleased()
        {
            return IsXButton1MouseReleased() && WasXButton1MousePressed();
        }

        public bool WasXButton1MouseReleased()
        {
            return previousMouseState.XButton1 == false;
        }

        #endregion

        #region XButton2 Mouse Button States

        public bool IsXButton2MousePressed()
        {
            return actualMouseState.XButton2;
        }

        public bool IsNewXButton2MousePressed()
        {
            return IsXButton2MousePressed() && WasXButton2MouseReleased();
        }

        public bool WasXButton2MousePressed()
        {
            return previousMouseState.XButton2;
        }

        public bool IsXButton2MouseReleased()
        {
            return actualMouseState.XButton2 == false;
        }

        public bool IsNewXButton2MouseReleased()
        {
            return IsXButton2MouseReleased() && WasXButton2MousePressed();
        }

        public bool WasXButton2MouseReleased()
        {
            return previousMouseState.XButton2 == false;
        }

        #endregion

        #region Mouse Wheel Values

        public int MouseScrollWheelValue()
        {
            return actualMouseState.ScrollWheelValue;
        }

        public int PreviousMouseScrollWheelValue()
        {
            return previousMouseState.ScrollWheelValue;
        }

        public int DeltaMouseScrollWheelValue()
        {
            return MouseScrollWheelValue() - PreviousMouseScrollWheelValue();
        }

        #endregion

        #region Mouse States Management

        public int GetMouseX()
        {
            return actualMouseState.X;
        }

        public int GetMouseY()
        {
            return actualMouseState.Y;
        }

        public int GetMouseXForGamePlay()
        {
            int inv = mouseXIsInverted ? -1 : 1;
            return (int)(mouseSensetivity * (inv * (actualMouseState.X)));
        }

        public int GetMouseYForGamePlay()
        {
            int inv = mouseYIsInverted ? -1 : 1;
            return (int)(mouseSensetivity * (inv * (actualMouseState.Y)));
        }

        public bool GetMouseXInversion()
        {
            return mouseXIsInverted;
        }

        public bool GetMouseYInversion()
        {
            return mouseYIsInverted;
        }

        public void SetMouseXInversion(bool inversion)
        {
            mouseXIsInverted = inversion;
        }

        public void SetMouseYInversion(bool inversion)
        {
            mouseYIsInverted = inversion;
        }

        public void SetMouseSensetivity(float sensetivity)
        {
            mouseSensetivity = sensetivity;
        }

        public Vector2 GetMousePosition()
        {
            return absoluteMousePosition;
        }

        public Vector2 GetMouseAreaSize()
        {
            return bufferedInputSource.MouseAreaSize;
        }

        public void SetMousePosition(int x, int y)
        {
            WindowsMouse.SetPosition(x, y);
        }

        #endregion

        #region Game Control States

        public bool IsGameControlPressed(StringId controlId)
        {
            if (IsControlBlocked(controlId))
                return false;
            Control control;
            if (gameControlList.TryGetValue(controlId, out control))
                return control.IsPressed();
            else
                return false;
        }

        public bool IsNewGameControlPressed(StringId controlId)
        {
            if (IsControlBlocked(controlId))
                return false;
            Control control;
            if (gameControlList.TryGetValue(controlId, out control))
                return control.IsNewPressed();
            else
                return false;
        }

        public bool IsNewGameControlReleased(StringId controlId)
        {
            if (IsControlBlocked(controlId))
                return false;
            Control control;
            if (gameControlList.TryGetValue(controlId, out control))
                return control.IsNewReleased();
            else
                return false;
        }

        public float GetGameControlAnalogState(StringId controlId)
        {
            if (IsControlBlocked(controlId))
                return 0f;
            Control control;
            if (gameControlList.TryGetValue(controlId, out control))
                return control.GetAnalogState();
            else
                return 0f;
        }

        #endregion

        public bool IsKeyValid(Keys key)
        {
            return validKeyboardKeys.Contains(key);
            //foreach (var item in validKeyboardKeys)
            //    if (item == key)
            //        return true;
            //return false;
        }

        public bool IsKeyDigit(Keys key)
        {
            return digitKeys.Contains(key);
        }

        public bool IsMouseButtonValid(MouseButtonsEnum button)
        {
            return validMouseButtons.Contains(button);
        }

        public Control GetControl(Keys key)
        {
            foreach(var item in gameControlList.Values)
            {
                if (item.GetKeyboardControl() == key || item.GetSecondKeyboardControl() == key)
                    return item;
            }
            return null;
        }

        public void GetListOfPressedKeys(List<Keys> keys)
        {
            GetPressedKeys(keys);
        }

        public void GetListOfPressedMouseButtons(List<MouseButtonsEnum> keys)
        {
            keys.Clear();
            if (IsLeftMousePressed())
                keys.Add(MouseButtonsEnum.Left);
            if (IsMiddleMousePressed())
                keys.Add(MouseButtonsEnum.Middle);
            if (IsRightMousePressed())
                keys.Add(MouseButtonsEnum.Right);
            if (IsXButton1MousePressed())
                keys.Add(MouseButtonsEnum.XButton1);
            if (IsXButton2MousePressed())
                keys.Add(MouseButtonsEnum.XButton2);
        }

        public Dictionary<StringId, Control> GetGameControlsList()
        {
            return gameControlList;
        }

        public void TakeSnapshot()
        {
            CloneControls(gameControlList, gameControlSnapshot);
        }

        public void RevertChanges()
        {
            CloneControls(gameControlSnapshot, gameControlList);
        }
        
        public string GetGameControlTextEnum(StringId controlId)
        {
            return gameControlList[controlId].GetControlButtonName(GuiInputDeviceEnum.Keyboard);
        }

        public Control GetGameControl(StringId controlId)
        {
            Control control;
            gameControlList.TryGetValue(controlId, out control);
            return control;
        }

        private void CloneControls(Dictionary<StringId, Control> original, Dictionary<StringId, Control> copy)
        {
            foreach(var entry in original)
            {
                Control control;
                if (copy.TryGetValue(entry.Key, out control))
                    control.CopyFrom(entry.Value);
                else
                    copy[entry.Key] = new Control(entry.Value);
            }
        }

        public void RevertToDefaultControls()
        {
            mouseXIsInverted = IsMouseXInvertedDefault;
            mouseYIsInverted = IsMouseYInvertedDefault;
            mouseSensetivity = MouseSensetivityDefault;

            CloneControls(defaultGameControlList, gameControlList);
        }

        public void SaveControls(Dictionary<string, object> controlsGeneral, Dictionary<string, object> controlButtons)
        {
            controlsGeneral.Clear();
            controlsGeneral.Add("mouseXIsInverted", mouseXIsInverted.ToString());
            controlsGeneral.Add("mouseYIsInverted", mouseYIsInverted.ToString());
            controlsGeneral.Add("mouseSensetivity", mouseSensetivity.ToString());

            controlButtons.Clear();
            foreach(Control control in gameControlList.Values)
            {
                var gameControlData = new Dictionary<string, string>();
                controlButtons[control.GetControlTypeEnum().ToString()] = gameControlData;
                gameControlData["Keyboard1"] = control.GetKeyboardControl().ToString();
                gameControlData["Keyboard2"] = control.GetSecondKeyboardControl().ToString();
                gameControlData["Mouse"] = EnumToString.MouseButtonsEnum[(int)control.GetMouseControl()];
            }
        }

        private bool LoadControls(Dictionary<string, object> controlsGeneral, Dictionary<string, object> controlsButtons)
        {
            if(controlsGeneral.Count == 0)
            {
                RevertToDefaultControls();
                return false;
            }

            try
            {
                mouseXIsInverted = bool.Parse((string)controlsGeneral["mouseXIsInverted"]);
                mouseYIsInverted = bool.Parse((string)controlsGeneral["mouseYIsInverted"]);
                mouseSensetivity = float.Parse((string)controlsGeneral["mouseSensetivity"]);

                LoadGameControls(controlsButtons);
                return true;
            }
            catch(Exception ex)
            {
                // log this exception
                RevertToDefaultControls();
                return false;
            }
        }

        private void LoadGameControls(Dictionary<string, object> controlButtons)
        {
            if(controlButtons.Count == 0)
                throw new Exception("controlButtons config parameter is empty!");

            //BEFORE - FIRE_PRIMARY:Mouse:Left;
            //AFTER - FIRE_PRIMARY:Keyboard:A:Mouse:Left:Joystick:None
            foreach(var gameControlButton in controlButtons)
            {
                var controlType = TryParseGameControlEnums(gameControlButton.Key);

                if (controlType.HasValue)
                {
                    gameControlList[controlType.Value].SetNoControl();
                    var gameControlData = (Dictionary<string, string>)gameControlButton.Value;

                    LoadGameControls(gameControlData["Keyboard1"], controlType.Value, ParseGuiInputDeviceEnum("Keyboard"));
                    LoadGameControls(gameControlData["Keyboard2"], controlType.Value, ParseGuiInputDeviceEnum("KeyboardSecond"));
                    LoadGameControls(gameControlData["Mouse"], controlType.Value, ParseGuiInputDeviceEnum("Mouse"));
                }
            }
        }

        private void LoadGameControls(string controlName, StringId controlType, GuiInputDeviceEnum device)
        {
            switch (device)
            {
                case GuiInputDeviceEnum.Keyboard:
                    {
                        Keys key = (Keys)Enum.Parse(typeof(Keys), controlName);
                        if (!IsKeyValid(key))
                            throw new Exception("Key \"" + key.ToString() + " is already assigned or not valid!");
                        FindNotAssignedGameControl(controlType, device).SetControl(GuiInputDeviceEnum.Keyboard, key);
                    }
                    break;
                case GuiInputDeviceEnum.KeyboardSecondary:
                    {
                        Keys key = (Keys)Enum.Parse(typeof(Keys), controlName);
                        if (!IsKeyValid(key))
                            throw new Exception("Key \"" + key.ToString() + " is already assigned or not valid!");
                        FindNotAssignedGameControl(controlType, device).SetControl(GuiInputDeviceEnum.KeyboardSecondary, key);
                    }
                    break;
                case GuiInputDeviceEnum.Mouse:
                    {
                        MouseButtonsEnum key = ParseMouseButtonsEnum(controlName);
                        if (!IsMouseButtonValid(key))
                            throw new Exception("Key \"" + key.ToString() + "\" is already assigned or not valid!");
                        FindNotAssignedGameControl(controlType, device).SetControl(key);
                    }
                    break;
                case GuiInputDeviceEnum.None:
                    break;
                default:
                    break;
            }
        }

        public GuiInputDeviceEnum ParseGuiInputDeviceEnum(string str)
        {
           for(int ind = 0; ind < EnumToString.GuiInputDeviceEnum.Length; ind++)
           {
            if (EnumToString.GuiInputDeviceEnum[ind] == str)
                return (GuiInputDeviceEnum)ind;
           }
            throw new ArgumentException("Value \"" + str + "\" is not from GuiInputDeviceEnum!");
        }

        public MouseButtonsEnum ParseMouseButtonsEnum(string str)
        {
            for (int ind = 0; ind < EnumToString.MouseButtonsEnum.Length; ind++)
            {
                if (EnumToString.MouseButtonsEnum[ind] == str)
                    return (MouseButtonsEnum)ind;
            }
            throw new ArgumentException("Value \"" + str + "\" is not from MouseButtonsEnum!");
        }

        public StringId? TryParseGameControlEnums(string str)
        {
            StringId id = StringId.GetOrCompute(str);
            if (gameControlList.ContainsKey(id))
                return id;
            else
                return null;
        }

        public Control FindNotAssignedGameControl(StringId controlId, GuiInputDeviceEnum deviceType)
        {
            Control control;
            if (!gameControlList.TryGetValue(controlId, out control))
                throw new Exception("Game control \"" + controlId.ToString() + "\" bit four in control list!");
            if (control.IsControlAssigned(deviceType))
                throw new Exception("Game control \"" + controlId.ToString() + "\" is alredy assigned!");
            return control;
        }

        public string GetKeyName(StringId controlId)
        {
            return GetGameControl(controlId).GetControlButtonName(GuiInputDeviceEnum.Keyboard);
        }

        public string GetKeyName(Keys key)
        {
            return controlNameLookup.GetKeyName(key);
        }

        public string GetName(MouseButtonsEnum mouseButton)
        {
            return controlNameLookup.GetName(mouseButton);
        }

        public string GetUnassignedName()
        {
            return controlNameLookup.UnassignedText;
        }

        public void SetControlBlock(StringId controlId, bool block = false)
        {
            if (block)
                gameControlBlackList.Add(controlId);
            else
                gameControlBlackList.Remove(controlId);
        }

        public bool IsControlBlocked(StringId controlId)
        {
            return gameControlBlackList.Contains(controlId);
        }

        public void ClearBlacklist()
        {
            gameControlBlackList.Clear();
        }
    }
}
