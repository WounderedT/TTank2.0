using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPresenter;
using TPresenter.Game.Character;
using TPresenter.Game.Interfaces;

namespace TPresenter.Game
{
    public static class MyObjectManager
    {
        public static List<IMyEntity> Entities = new List<IMyEntity>();

        //  Loading the world entities. This should be loaded from save file / default configuration.
        public static void Start()
        {
            MyCharacter character = new MyCharacter();
            character.Init();
            Entities.Add(character);
        }
    }
}
