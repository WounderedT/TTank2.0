using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPresenter.Input
{
    public static class EnumToString
    {
        public static string[] GuiInputDeviceEnum = new string[] { "None", "Keyboard", "Mouse", "KeyboardSecondary" };
        public static string[] MouseButtonsEnum = new string[] { "None", "Left", "Middle", "Right", "XButton1", "XButton2" };

        public static string[] ControlTypeEnum = new string[] { "General", "Systems", "Spectator" };
    }
}
