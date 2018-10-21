using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfilerViewer.Structures
{
    public static class Settings
    {
        private static int[] _scale;

        public static int AxisX
        {
            get { return 1200; }
        }

        public static int AxisY
        {
            get { return 400; }
        }

        public static int[] Scale
        {
            get
            {
                if(_scale == null)
                {
                    _scale = new int[] { AxisX / 8, AxisX / 4, AxisX / 2, AxisX };
                }
                return _scale;
            }
        }
    }
}
