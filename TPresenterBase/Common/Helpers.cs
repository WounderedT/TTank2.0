using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPresenter.Render
{
    class HashHelpers
    {
        public static int Combine(int h0, int h1)
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + h0;
                hash = hash * 31 + h1;
                return hash;
            }
        }

        public static void Combine(ref int h0, int h1)
        {
            h0 = Combine(h0, h1);
        }
    }
}
