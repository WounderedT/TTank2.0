using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPresenter.Input
{
    class KeyStringEntry
    {
        private StringId id;

        public string Name { get { return id.ToString(); } }
        public StringId Id { get { return id; } }

        public KeyStringEntry(string name)
        {
            id = StringId.GetOrCompute(name);
        }
    }

    class UtilKeyToString: KeyStringEntry
    {
        private Keys key;

        public Keys Key { get { return key; } }

        public UtilKeyToString(Keys key, string name) : base(name)
        {
            this.key = key;
        }
    }

    public class KeysToString: IControlNameLookup
    {
        private readonly String[] systemKeyNamesUpper = new String[256];

        private readonly UtilKeyToString[] keyToString = new UtilKeyToString[]
        {
            new UtilKeyToString(Keys.Left, "KeysLeft"),
            new UtilKeyToString(Keys.Right, "KeysRight"),
            new UtilKeyToString(Keys.Up, "KeysUp"),
            new UtilKeyToString(Keys.Down, "KeysDown"),
            new UtilKeyToString(Keys.Home, "KeysHome"),
            new UtilKeyToString(Keys.End, "KeysEnd"),
            new UtilKeyToString(Keys.Delete, "KeysDelete"),
            new UtilKeyToString(Keys.Back, "KeysBackspace"),
            new UtilKeyToString(Keys.Insert, "KeysInsert"),
            new UtilKeyToString(Keys.PageDown, "KeysPageDown"),
            new UtilKeyToString(Keys.PageUp, "KeysPageUp"),
            new UtilKeyToString(Keys.LeftAlt, "KeysLeftAlt"),
            new UtilKeyToString(Keys.LeftControl, "KeysLeftControl"),
            new UtilKeyToString(Keys.LeftShift, "KeysLeftShift"),
            new UtilKeyToString(Keys.RightAlt, "KeysRightAlt"),
            new UtilKeyToString(Keys.RightControl, "KeysRightControl"),
            new UtilKeyToString(Keys.RightShift, "KeysRightShift"),
            new UtilKeyToString(Keys.CapsLock, "KeysCapsLock"),
            new UtilKeyToString(Keys.Enter, "KeysEnter"),
            new UtilKeyToString(Keys.Tab, "KeysTab"),
            new UtilKeyToString(Keys.OemOpenBrackets, "KeysOpenBracket"),
            new UtilKeyToString(Keys.OemCloseBrackets, "KeysCloseBracket"),
            new UtilKeyToString(Keys.Multiply, "KeysMultiply"),
            new UtilKeyToString(Keys.Subtract, "KeysSubtract"),
            new UtilKeyToString(Keys.Add, "KeysAdd"),
            new UtilKeyToString(Keys.Divide, "KeysDivide"),
            new UtilKeyToString(Keys.NumPad0, "KeysNumPad0"),
            new UtilKeyToString(Keys.NumPad1, "KeysNumPad1"),
            new UtilKeyToString(Keys.NumPad2, "KeysNumPad2"),
            new UtilKeyToString(Keys.NumPad3, "KeysNumPad3"),
            new UtilKeyToString(Keys.NumPad4, "KeysNumPad4"),
            new UtilKeyToString(Keys.NumPad5, "KeysNumPad5"),
            new UtilKeyToString(Keys.NumPad6, "KeysNumPad6"),
            new UtilKeyToString(Keys.NumPad7, "KeysNumPad7"),
            new UtilKeyToString(Keys.NumPad8, "KeysNumPad8"),
            new UtilKeyToString(Keys.NumPad9, "KeysNumPad9"),
            new UtilKeyToString(Keys.Decimal, "KeysDecimal"),
            new UtilKeyToString(Keys.OemBackslash, "KeysBackslash"),
            new UtilKeyToString(Keys.OemComma, "KeysComma"),
            new UtilKeyToString(Keys.OemMinus, "KeysMinus"),
            new UtilKeyToString(Keys.OemPeriod, "KeysPeriod"),
            new UtilKeyToString(Keys.OemPipe, "KeysPipe"),
            new UtilKeyToString(Keys.OemPlus, "KeysPlus"),
            new UtilKeyToString(Keys.OemQuestion, "KeysQuestion"),
            new UtilKeyToString(Keys.OemQuotes, "KeysQuotes"),
            new UtilKeyToString(Keys.OemSemicolon, "KeysSemicolon"),
            new UtilKeyToString(Keys.OemTilde, "KeysTilde"),
            new UtilKeyToString(Keys.Space, "KeysSpace"),
            new UtilKeyToString(Keys.Pause, "KeysPause"),

            new UtilKeyToString(Keys.D0, "0"),
            new UtilKeyToString(Keys.D1, "1"),
            new UtilKeyToString(Keys.D2, "2"),
            new UtilKeyToString(Keys.D3, "3"),
            new UtilKeyToString(Keys.D4, "4"),
            new UtilKeyToString(Keys.D5, "5"),
            new UtilKeyToString(Keys.D6, "6"),
            new UtilKeyToString(Keys.D7, "7"),
            new UtilKeyToString(Keys.D8, "8"),
            new UtilKeyToString(Keys.D9, "9"),
            new UtilKeyToString(Keys.F1, "F1"),
            new UtilKeyToString(Keys.F2, "F2"),
            new UtilKeyToString(Keys.F3, "F3"),
            new UtilKeyToString(Keys.F4, "F4"),
            new UtilKeyToString(Keys.F5, "F5"),
            new UtilKeyToString(Keys.F6, "F6"),
            new UtilKeyToString(Keys.F7, "F7"),
            new UtilKeyToString(Keys.F8, "F8"),
            new UtilKeyToString(Keys.F9, "F9"),
            new UtilKeyToString(Keys.F10, "F10"),
            new UtilKeyToString(Keys.F11, "F11"),
            new UtilKeyToString(Keys.F12, "F12"),
            new UtilKeyToString(Keys.F13, "F13"),
            new UtilKeyToString(Keys.F14, "F14"),
            new UtilKeyToString(Keys.F15, "F15"),
            new UtilKeyToString(Keys.F16, "F16"),
            new UtilKeyToString(Keys.F17, "F17"),
            new UtilKeyToString(Keys.F18, "F18"),
            new UtilKeyToString(Keys.F19, "F19"),
            new UtilKeyToString(Keys.F20, "F20"),
            new UtilKeyToString(Keys.F21, "F21"),
            new UtilKeyToString(Keys.F22, "F22"),
            new UtilKeyToString(Keys.F23, "F23"),
            new UtilKeyToString(Keys.F24, "F24"),
        };

        public KeysToString()
        {
            for(int ind = 0; ind < systemKeyNamesUpper.Length; ind++)
            {
                systemKeyNamesUpper[ind] = ((char)ind).ToString().ToUpper();
            }
        }

        public string UnassignedText
        {
            get
            {
                return "UnknownControl_Unassigned";
            }
        }

        public string GetKeyName(Keys key)
        {
            if ((int)key > systemKeyNamesUpper.Length)
                return null;

            string value = systemKeyNamesUpper[(int)key];
            foreach(UtilKeyToString entry in keyToString)
            {
                if (entry.Key == key)
                {
                    value = entry.Name;
                    break;
                }
            }
            return value;
        }

        public string GetName(MouseButtonsEnum mouseButton)
        {
            switch (mouseButton)
            {
                case MouseButtonsEnum.Left:
                    return "LeftMouseButton";
                case MouseButtonsEnum.Middle:
                    return "MiddleMouseButton";
                case MouseButtonsEnum.Right:
                    return "RightMouseButton";
                case MouseButtonsEnum.XButton1:
                    return "XButton1MouseButton";
                case MouseButtonsEnum.XButton2:
                    return "XButton2MouseButton";
            }
            return string.Empty;
        }
    }
}
