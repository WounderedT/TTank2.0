using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPresenter;
using TPresenter.Game.Interfaces;

namespace TTank20.Game.Game.World
{
    public class Session
    {
        internal SpectatorCameraController Spectator = new SpectatorCameraController();

        public static Session Static;

        private ICameraController cameraController;

        public ICameraController CameraController
        {
            get { return cameraController;}
            set
            {
                cameraController = value;
            }
        }

        public static void Start(ICameraController controller)
        {
            Static = new Session();
            Static.SetCameraController(controller);
        }

        public void SetCameraController(ICameraController controller)
        {
            //if(Spectator.Position == Vector3.Zero)
            //{
            //    Spectator.Position = ;
            //    Spectator.Target = ;
            //}
            SpectatorCameraController.Static.SpectatorCameraMovement = SpectatorCameraMovementEnum.UserControled;
            Static.CameraController = Spectator;
        }
    }
}
