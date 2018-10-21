using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPresenter.Input
{
    public class MyInput
    {
        public static DirectXInput Static
        {
            get;
            private set;
        }

        public static void Initialize(DirectXInput input)
        {
            if (Static != null)
                throw new InvalidOperationException("Imput is already initialized!");
            Static = input;
        }

        public static void UnloadData()
        {
            if(Static != null)
            {
                Static.UnloadData();
                Static = null;
            }
        }
    }
}
