using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPresenter.Utils
{
    public class GameLoop
    {
        public delegate void GameAction();
        public bool IsDone = false;

        public void Run(GameAction action)
        {
            while (!IsDone)
                action();
        }
    }
}
