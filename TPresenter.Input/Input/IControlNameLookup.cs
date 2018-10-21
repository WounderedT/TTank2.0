using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPresenter.Input
{
    public interface IControlNameLookup
    {
        string GetKeyName(Keys key);
        string GetName(MouseButtonsEnum mouseButton);
        string UnassignedText { get; }
    }
}
