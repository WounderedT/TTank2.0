using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPresenter;

namespace TPresenter.Input
{
    public class Control
    {
        private const int DEFAULT_CAPACITY = 16;
        private static StringBuilder toStringCache = new StringBuilder(DEFAULT_CAPACITY);

        struct Data
        {
            public StringId Name;
            public StringId ControlId;
            public Keys KeyboardKey1;
            public Keys KeyboardKey2;
            public GuiControlTypeEnum ControlType;
            public MouseButtonsEnum MouseButton;
            public StringId? Description;
        }

        private Data data;

        private StringId name
        {
            get { return data.Name; }
            set { data.Name = value; }
        }

        private StringId controlId
        {
            get { return data.ControlId; }
            set { data.ControlId = value; }
        }

        private Keys keyboardKey1
        {
            get { return data.KeyboardKey1; }
            set { data.KeyboardKey1 = value; }
        }

        private Keys keyboardKey2
        {
            get { return data.KeyboardKey2; }
            set { data.KeyboardKey2 = value; }
        }

        private GuiControlTypeEnum controlType
        {
            get { return data.ControlType; }
            set { data.ControlType = value; }
        }

        private MouseButtonsEnum mouseButton
        {
            get { return data.MouseButton; }
            set { data.MouseButton = value; }
        }

        public Control(StringId controlId, StringId name, GuiControlTypeEnum controlType,
            MouseButtonsEnum? defaultControlMouse, Keys? defaultContolKey1, Keys? defaultContolKey2 = null,
            StringId? helpText = null, StringId? description = null)
        {
            this.controlId = controlId;
            this.name = name;
            this.controlType = controlType;
            this.mouseButton = defaultControlMouse ?? MouseButtonsEnum.None;
            this.keyboardKey1 = defaultContolKey1 ?? Keys.None;
            this.keyboardKey2 = defaultContolKey2 ?? Keys.None;
            data.Description = description;
        }

        public Control(Control control)
        {
            this.CopyFrom(control);
        }

        public void SetControl(GuiInputDeviceEnum deviceInput, Keys key)
        {
            if (deviceInput == GuiInputDeviceEnum.Keyboard)
                keyboardKey1 = key;
            else if (deviceInput == GuiInputDeviceEnum.KeyboardSecondary)
                keyboardKey2 = key;
            //TODO: add logger for second else case (no match).
        }

        public void SetControl(MouseButtonsEnum mouseButton)
        {
            this.mouseButton = mouseButton;
        }

        public void SetNoControl()
        {
            mouseButton = MouseButtonsEnum.None;
            keyboardKey1 = Keys.None;
            keyboardKey2 = Keys.None;
        }

        public Keys GetKeyboardControl()
        {
            return keyboardKey1;
        }

        public Keys GetSecondKeyboardControl()
        {
            return keyboardKey2;
        }

        public MouseButtonsEnum GetMouseControl()
        {
            return mouseButton;
        }

        public bool IsPressed()
        {
            bool pressed = false;
            if (keyboardKey1 != Keys.None)
                pressed = MyInput.Static.IsKeyPressed(keyboardKey1);

            if (keyboardKey2 != Keys.None)
                pressed = MyInput.Static.IsKeyPressed(keyboardKey2);

            if (mouseButton != MouseButtonsEnum.None)
                pressed = MyInput.Static.IsMousePressed(mouseButton);

            return pressed;
        }

        public bool IsNewPressed()
        {
            bool pressed = false;
            if (keyboardKey1 != Keys.None)
                pressed = MyInput.Static.IsNewKeyPressed(keyboardKey1);

            if (keyboardKey2 != Keys.None)
                pressed = MyInput.Static.IsNewKeyPressed(keyboardKey2);

            if (mouseButton != MouseButtonsEnum.None)
                pressed = MyInput.Static.IsNewMousePressed(mouseButton);

            return pressed;
        }

        public bool IsNewReleased()
        {
            bool released = false;
            if (keyboardKey1 != Keys.None)
                released = MyInput.Static.IsNewKeyReleased(keyboardKey1);

            if (keyboardKey2 != Keys.None && released == false)
                released = MyInput.Static.IsNewKeyReleased(keyboardKey2);

            if (mouseButton != MouseButtonsEnum.None && released == false)
            {
                switch (mouseButton)
                {
                    case MouseButtonsEnum.Left:
                        released = MyInput.Static.IsNewLeftMouseReleased();
                        break;
                    case MouseButtonsEnum.Middle:
                        released = MyInput.Static.IsNewMiddleMouseReleased();
                        break;
                    case MouseButtonsEnum.Right:
                        released = MyInput.Static.IsNewRightMouseReleased();
                        break;
                    case MouseButtonsEnum.XButton1:
                        released = MyInput.Static.IsNewXButton1MouseReleased();
                        break;
                    case MouseButtonsEnum.XButton2:
                        released = MyInput.Static.IsNewXButton2MouseReleased();
                        break;
                }
            }

            return released;
        }

        public float GetAnalogState()
        {
            bool pressed = false;
            if (keyboardKey1 != Keys.None)
                pressed = MyInput.Static.IsKeyPressed(keyboardKey1);

            if (keyboardKey2 != Keys.None && pressed == false)
                pressed = MyInput.Static.IsKeyPressed(keyboardKey2);

            if (mouseButton != MouseButtonsEnum.None && pressed == false)
            {
                switch (mouseButton)
                {
                    case MouseButtonsEnum.Left:
                        pressed = MyInput.Static.IsLeftMousePressed();
                        break;
                    case MouseButtonsEnum.Middle:
                        pressed = MyInput.Static.IsMiddleMousePressed();
                        break;
                    case MouseButtonsEnum.Right:
                        pressed = MyInput.Static.IsRightMousePressed();
                        break;
                    case MouseButtonsEnum.XButton1:
                        pressed = MyInput.Static.IsXButton1MousePressed();
                        break;
                    case MouseButtonsEnum.XButton2:
                        pressed = MyInput.Static.IsXButton2MousePressed();
                        break;
                }
            }

            return pressed ? 1 : 0;
        }

        public StringId GetContolName()
        {
            return name;
        }

        public StringId? GetControlDescription()
        {
            return data.Description;
        }

        public GuiControlTypeEnum GetControlTypeEnum()
        {
            return controlType;
        }

        public StringId GetGameControlEnum()
        {
            return controlId;
        }

        public bool IsControlAssigned()
        {
            return (keyboardKey1 != Keys.None || mouseButton != MouseButtonsEnum.None);
        }

        public bool IsControlAssigned(GuiInputDeviceEnum deviceInput)
        {
            bool assigned = false;
            switch (deviceInput)
            {
                case GuiInputDeviceEnum.Keyboard:
                    assigned = keyboardKey1 != Keys.None;
                    break;
                case GuiInputDeviceEnum.Mouse:
                    assigned = mouseButton != MouseButtonsEnum.None;
                    break;
            }

            return assigned;
        }

        public void CopyFrom(Control control)
        {
            data = control.data;
        }

        #region Control to string and StringBuilder conversions
        public override string ToString()
        {
            return ButtonNames;
        }

        public string ButtonNames
        {
            get
            {
                toStringCache.Clear();
                AppendBoundButtonNames(ref toStringCache, unassignedText: MyInput.Static.GetUnassignedName());
                return toStringCache.ToString();
            }
        }

        public string ButtonNamesIgnoreSecondary
        {
            get
            {
                toStringCache.Clear();
                AppendBoundButtonNames(ref toStringCache, unassignedText: null, includeSecondary: false);
                return toStringCache.ToString();
            }
        }

        public StringBuilder ToStringBuilder(string unassignedText)
        {
            toStringCache.Clear();
            AppendBoundButtonNames(ref toStringCache, unassignedText: unassignedText);
            return new StringBuilder(toStringCache.Length).AppendStringBuilder(toStringCache);
        }

        public string GetControlButtonName(GuiInputDeviceEnum deviceType)
        {
            toStringCache.Clear();
            AppendBoundButtonNames(ref toStringCache, deviceType);
            return toStringCache.ToString();
        }

        public void AppendBoundKeyJustOne(ref StringBuilder output)
        {
            EnsureExists(ref output);
            if (keyboardKey1 != Keys.None)
                AppendName(ref output, keyboardKey1);
            else
                AppendName(ref output, keyboardKey2);
        }

        public void AppendBoundButtonNames(ref StringBuilder output, GuiInputDeviceEnum deviceType, string separator = null)
        {
            EnsureExists(ref output);
            switch (deviceType)
            {
                case GuiInputDeviceEnum.Keyboard:
                    if (separator == null)
                        AppendName(ref output, keyboardKey1);
                    else
                        AppendName(ref output, keyboardKey1, keyboardKey2, separator);
                    break;
                case GuiInputDeviceEnum.KeyboardSecondary:
                    if (separator == null)
                        AppendName(ref output, keyboardKey2);
                    else
                        AppendName(ref output, keyboardKey1, keyboardKey2, separator);
                    break;
                case GuiInputDeviceEnum.Mouse:
                    AppendName(ref output, mouseButton);
                    break;
            }
        }

        public void AppendBoundButtonNames(ref StringBuilder output, string separator = ", ", string unassignedText = "null", bool includeSecondary = true)
        {
            EnsureExists(ref output);
            GuiInputDeviceEnum[] deviceTypes = { GuiInputDeviceEnum.Keyboard, GuiInputDeviceEnum.Mouse };
            int appendCount = 0;

            foreach(GuiInputDeviceEnum deviceType in deviceTypes)
            {
                if (!IsControlAssigned(deviceType))
                    continue;
                if (appendCount > 0)
                    output.Append(separator);
                AppendBoundButtonNames(ref output, deviceType, includeSecondary ? separator : null);
                appendCount++;
            }

            if (appendCount == 0 && unassignedText != null)
                output.Append(unassignedText);
        }

        public static void AppendName(ref StringBuilder output, Keys key)
        {
            EnsureExists(ref output);
            if (key != Keys.None)
                output.Append(MyInput.Static.GetKeyName(key));
        }

        public static void AppendName(ref StringBuilder output, Keys key1, Keys key2, string separator)
        {
            EnsureExists(ref output);
            string key1String = null;
            string key2String = null;

            if (key1 != Keys.None)
                key1String = MyInput.Static.GetKeyName(key1);
            if (key2 != Keys.None)
                key2String = MyInput.Static.GetKeyName(key2);

            if (key1String != null && key2String != null)
                output.Append(key1String).Append(separator).Append(key2String);
            else if (key1String != null)
                output.Append(key1String);
            else
                output.Append(key2String);
        }

        public static void AppendName(ref StringBuilder output, MouseButtonsEnum mouseButton)
        {
            EnsureExists(ref output);
            output.Append(MyInput.Static.GetName(mouseButton));
        }

        public static void AppendUnknownTextIfNeeded(ref StringBuilder output, string unassignedText)
        {
            EnsureExists(ref output);
            if (output.Length == 0)
                output.Append(unassignedText);
        }

        public static void EnsureExists(ref StringBuilder output)
        {
            if (output == null)
                output = new StringBuilder(DEFAULT_CAPACITY);
        }

        #endregion
    }
}
