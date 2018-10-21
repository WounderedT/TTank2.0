using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPresenter.Input
{
    //Class will be used to unify static/analog input the single input value.
    public static class ControllerHelper
    {
        public static float IsControlAnalog(StringId stringId)
        {
            return MyInput.Static.GetGameControlAnalogState(stringId);
        }
    }
}
