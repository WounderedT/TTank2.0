using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPresenter.Game.Utils;

namespace TPresenter.Game.Interfaces
{
    public interface ICameraController
    {
        bool IsInFirstPersionView { get; set; }
        bool ForceFirstPersion { get; set; }

        /// <summary>
        /// Change camera properties.
        /// </summary>
        /// <param name="currentCamera"></param>
        void ControlCamera(Camera currentCamera);

        /// <summary>
        /// Rotate the camera
        /// </summary>
        /// <param name="rotationIndicator"></param>
        /// <param name="rollIndicator"></param>
        void Rotate(Vector2 rotationIndicator, float rollIndicator);

        /// <summary>
        /// Stop the camera rotatiton
        /// </summary>
        void RotateStopped();

        void OnAssumeControl(ICameraController previousCameraController);
        void OnReleaseControl(ICameraController newCameraController);

        /// <summary>
        /// Used to send "use" commands to camera controller.
        /// </summary>
        /// <returns>
        /// Return value indicates if the camera controller handled the use button. It should fail to ControlledObject if not.
        /// </returns>
        bool HandleUse();
        bool HandlePickUp();
    }
}
