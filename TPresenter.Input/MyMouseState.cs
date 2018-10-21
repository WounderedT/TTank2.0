using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPresenter.Input
{
    public struct MyMouseState
    {
        public int X;
        public int Y;
        public int ScrollWheelValue;

        public bool LeftButton;
        public bool RightButton;
        public bool MiddleButton;
        public bool XButton1;
        public bool XButton2;

        public MyMouseState(int x, int y, int scrollWheel, bool leftButton, bool rightButton, bool middleButton, bool xButton1, bool xButton2)
        {
            X = x;
            Y = y;
            ScrollWheelValue = scrollWheel;

            LeftButton = leftButton;
            RightButton = rightButton;
            MiddleButton = middleButton;
            XButton1 = xButton1;
            XButton2 = xButton2;
        }

        public void ClearPosition()
        {
            X = 0;
            Y = 0;
        }
    }
}
