using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPresenter.Render.Lights
{
    public class Light
    {
        private bool lightOn;
        private bool propertiesChanged;

        /// <summary>
        /// Light is used in lighting calculation if true.
        /// </summary>
        public bool LightOn
        {
            get { return lightOn; }
            set
            {
                if(lightOn != value)
                {
                    lightOn = value;
                    propertiesChanged = true;
                }
            }
        }
    }
}
