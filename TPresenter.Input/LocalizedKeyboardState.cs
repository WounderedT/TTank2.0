using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPresenter.Library.Win32;

namespace TPresenter.Input
{
    public class LocalizedKeyboardState
    {
        internal const uint KLF_NOTELLSHELL = 0x00000080;

        static HashSet<byte> localKeys;
        private MyKeyboardState previousKeyboardState;
        private MyKeyboardState actualKeyboardState;

        public struct KeyboardLayout : IDisposable
        {
            public readonly IntPtr Handle;
            public static KeyboardLayout US_English = new KeyboardLayout("00000409");

            public bool IsDisposed
            {
                get;
                private set;
            }

            public static KeyboardLayout Active
            {
                get
                {
                    return new KeyboardLayout(WinApi.GetKeyboardLayout(IntPtr.Zero));
                }
            }

            public KeyboardLayout(IntPtr handle) : this()
            {
                Handle = handle;
            }

            public KeyboardLayout(string keyboardLayoutId) : this(WinApi.LoadKeyboardLayout(keyboardLayoutId, KLF_NOTELLSHELL)) { }

            public void Dispose()
            {
                if (IsDisposed)
                    return;
                WinApi.UnloadKeyboardLayout(Handle);
                IsDisposed = true;
            }
        }

        public LocalizedKeyboardState()
        {
            actualKeyboardState = WindowsKeyboard.GetCurrentState();

            if(localKeys == null)
            {
                localKeys = new HashSet<byte>();
                AddLocalKey(Keys.LeftControl);
                AddLocalKey(Keys.LeftAlt);
                AddLocalKey(Keys.LeftShift);
                AddLocalKey(Keys.RightAlt);
                AddLocalKey(Keys.RightControl);
                AddLocalKey(Keys.RightShift);
                AddLocalKey(Keys.Delete);
                AddLocalKey(Keys.NumPad0);
                AddLocalKey(Keys.NumPad1);
                AddLocalKey(Keys.NumPad2);
                AddLocalKey(Keys.NumPad3);
                AddLocalKey(Keys.NumPad4);
                AddLocalKey(Keys.NumPad5);
                AddLocalKey(Keys.NumPad6);
                AddLocalKey(Keys.NumPad7);
                AddLocalKey(Keys.NumPad8);
                AddLocalKey(Keys.NumPad9);
                AddLocalKey(Keys.Decimal);
                AddLocalKey(Keys.LeftWindows);
                AddLocalKey(Keys.RightWindows);
                AddLocalKey(Keys.Apps);
                AddLocalKey(Keys.Pause);
                AddLocalKey(Keys.Divide);
            }
        }

        void AddLocalKey(Keys key)
        {
            localKeys.Add((byte)key);
        }

        public void ClearStates()
        {
            previousKeyboardState = actualKeyboardState;
            actualKeyboardState = new MyKeyboardState();
        }

        public void UpdateStates()
        {
            previousKeyboardState = actualKeyboardState;
            actualKeyboardState = WindowsKeyboard.GetCurrentState();
        }

        public void UpdateStateFromSnaphot(MyKeyboardState state)
        {
            previousKeyboardState = actualKeyboardState;
            actualKeyboardState = state;
        }

        public void UpdateKeyboardStateFromSnapshot(MyKeyboardState currentState, MyKeyboardState previousState)
        {
            previousKeyboardState = previousState;
            actualKeyboardState = currentState;
        }

        public MyKeyboardState GetActualKeyboardState()
        {
            return actualKeyboardState;
        }

        public MyKeyboardState GetPreviousKeyboardState()
        {
            return previousKeyboardState;
        }

        public void SetKey(Keys key, bool value)
        {
            actualKeyboardState.SetKey(key, value);
        }

        public bool IsPreviousKeyDown(Keys key, bool isLocalKey)
        {
            if (!isLocalKey)
                key = LocalToEnglish(key);

            return previousKeyboardState.IsKeyDown(key);
        }

        public bool IsPreviousKeyDown(Keys key)
        {
            return IsPreviousKeyDown(key, IsKeyLocal(key));
        }

        public bool IsPreviousKeyUp(Keys key, bool isLocalKey)
        {
            if (!isLocalKey)
                key = LocalToEnglish(key);

            return previousKeyboardState.IsKeyUp(key);
        }

        public bool IsPreviousKeyUp(Keys key)
        {
            return IsPreviousKeyUp(key, IsKeyLocal(key));
        }

        public bool IsKeyDown(Keys key, bool isLocalKey)
        {
            if (!isLocalKey)
                key = LocalToEnglish(key);

            return actualKeyboardState.IsKeyDown(key);
        }

        public bool IsKeyDown(Keys key)
        {
            return IsKeyDown(key, IsKeyLocal(key));
        }

        public bool IsKeyUp(Keys key, bool isLocalKey)
        {
            if (!isLocalKey)
                key = LocalToEnglish(key);

            return actualKeyboardState.IsKeyUp(key);
        }

        public bool IsKeyUp(Keys key)
        {
            return IsKeyUp(key, IsKeyLocal(key));
        }

        public static Keys LocalToEnglish(Keys key)
        {
            return key;
        }

        public static Keys EnglishToLocal(Keys key)
        {
            return key;
        }

        public bool IsKeyLocal(Keys key)
        {
            return localKeys.Contains((byte)key);
        }

        public bool IsAnyKeyPressed()
        {
            return actualKeyboardState.IsAnyKeyPressed();
        }

        public void GetActualPressedKeys(List<Keys> keys)
        {
            actualKeyboardState.GetPressedKeys(keys);
            for(int ind = 0; ind < keys.Count; ind++)
            {
                if (!IsKeyLocal(keys[ind]))
                    keys[ind] = EnglishToLocal(keys[ind]);
            }
        }
    }
}
