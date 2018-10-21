using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPresenter.Game;
using TPresenter.Game.Interfaces;
using TPresenter.Input;
using TPresenter.Render.Lights;
using TPresenter.Utils;
using TPresenterMath;
using TPresenter.Game.Utils;

namespace TPresenter
{
    public class SpectatorCameraController: Spectator, ICameraController
    {
        public const int REFLECTOR_RANGE_MULTIPLIER = 5;

        #region Fields And Properties

        new public static SpectatorCameraController Static;

        private double _yaw;
        private double _pitch;
        private double _roll;

        private Vector3 _lastRightVec = Vector3.Right;
        private Vector3 _lastUpVec = Vector3.Up;
        private Matrix _lastOrientation = Matrix.Identity;
        private float _lastOrientationWeight = 1;

        private Light _light;

        public bool IsLightOn
        {
            get { return _light != null && _light.LightOn; }
        }

        bool ICameraController.IsInFirstPersionView
        {
            get { return IsFirstPersonView; }
            set { IsFirstPersonView = value; }
        }

        bool ICameraController.ForceFirstPersion
        {
            get { return ForceFirstPersonCamera; }
            set { ForceFirstPersonCamera = value; }
        }

        #endregion

        public SpectatorCameraController()
        {
            Position = DEFAULT_POSITION; // Spectator default position must not be controlled like this. For debug purpose only. (1, 1, -2)
            Static = this;
        }

        public override void MoveAndRotate(Vector3 moveIndicator, Vector2 rotationIndicator, float rollIndicator)
        {
            if (MyInput.Static.IsAnyCtrlKeyPressed())
            {
                if (MyInput.Static.PreviousMouseScrollWheelValue() < MyInput.Static.MouseScrollWheelValue())
                    SpeedModeAngular = Math.Min(SpeedModeAngular * 1.5f, MAX_SPECTATOR_ANGULAR_SPEED);
                else if (MyInput.Static.PreviousMouseScrollWheelValue() > MyInput.Static.MouseScrollWheelValue())
                    SpeedModeAngular = Math.Max(SpeedModeAngular / 1.5f, MIN_SPECTATOR_ANGULAR_SPEED);
            }
            else
            {
                if (MyInput.Static.PreviousMouseScrollWheelValue() < MyInput.Static.MouseScrollWheelValue())
                    SpeedModeLinear = Math.Min(SpeedModeLinear * 1.5f, MAX_SPECTATOR_LINEAR_SPEED);
                else if (MyInput.Static.PreviousMouseScrollWheelValue() > MyInput.Static.MouseScrollWheelValue())
                    SpeedModeLinear = Math.Max(SpeedModeLinear / 1.5f, MIN_SPECTATOR_LINEAR_SPEED);
            }
            switch (SpectatorCameraMovement)
            {
                case SpectatorCameraMovementEnum.None:
                    break;
                case SpectatorCameraMovementEnum.FreeMouse:
                    MoveAndRotateFreeMouse(moveIndicator, rotationIndicator, rollIndicator);
                    break;
                case SpectatorCameraMovementEnum.ConstantDelta:
                    MoveAndRotateConstantDelta(moveIndicator, rotationIndicator, rollIndicator);
                    if (IsLightOn)
                        UpdateLightPosition();
                    break;
                case SpectatorCameraMovementEnum.UserControled:
                    MoveAndRotateUserControlled(moveIndicator, rotationIndicator, rollIndicator);
                    if (IsLightOn)
                        UpdateLightPosition();
                    break;
            }
        }

        #region Private Methods

        private void MoveAndRotateUserControlled(Vector3 moveIndicator, Vector2 rotationIndicator, float rollIndicator)
        {
            float amountOfMovement = EngineConstants.UPDATE_STEP_SIZE_IN_SECONDS * 100;
            float amountOfRotation = 0.025f * speedModeAngular;

            rollIndicator = MyInput.Static.GetDeveloperRoll();

            float rollAmount = 0;
            if(rollIndicator != 0)
            {
                Vector3 right, up;
                rollAmount = rollIndicator * speedModeAngular * 0.1f;
                rollAmount = MathHelper.Clamp(rollAmount, -0.02f, 0.02f);
                MyUtils.VectorPlaneRotation(orientation.Up, orientation.Right, rollAmount, out up, out right);
                orientation.Right = right;
                orientation.Up = up;
            }

            Vector3 moveVector;

            if(_lastOrientationWeight < 1)
            {
                orientation = Matrix.Orthogonalize(orientation);
                orientation.Forward = Vector3.Cross(orientation.Up, orientation.Right);
            }

            if(rotationIndicator.Y != 0)
            {
                Vector3 right, forward;
                MyUtils.VectorPlaneRotation(orientation.Right, orientation.Forward, rotationIndicator.Y * amountOfRotation, out right, out forward);
                orientation.Right = right;
                orientation.Forward = forward;
            }

            if(rotationIndicator.X != 0)
            {
                Vector3 up, forward;
                MyUtils.VectorPlaneRotation(orientation.Up, orientation.Forward, -rotationIndicator.X * amountOfRotation, out up, out forward);
                orientation.Up = up;
                orientation.Forward = forward;
            }

            _lastOrientation = orientation;
            _lastOrientationWeight = 1;
            _roll = 0;
            _pitch = 0;

            float afterburner = (MyInput.Static.IsAnyShiftKeyPressed() ? 1.0f : 0.35f) * (MyInput.Static.IsAnyCtrlKeyPressed() ? 0.3f : 1f);
            moveIndicator *= afterburner * SpeedModeLinear;
            moveVector = moveIndicator * amountOfMovement;

            if (!moveVector.IsZero)
                System.Console.Write("test");

            Position += Vector3D.Transform(moveVector, orientation);
        }

        private void MoveAndRotateConstantDelta(Vector3 moveIndicator, Vector2 rotaitonIndicator, float rollIndicator)
        {
            MoveAndRotateUserControlled(moveIndicator, rotaitonIndicator, rollIndicator);
        }

        private void MoveAndRotateFreeMouse(Vector3 moveIndicator, Vector2 rotationIndicator, float rollIndicator)
        {
            MoveAndRotateUserControlled(moveIndicator, rotationIndicator, rollIndicator);
        }

        #endregion

        protected override void OnChangingMode(SpectatorCameraMovementEnum oldMode, SpectatorCameraMovementEnum newMode)
        {
            if(newMode == SpectatorCameraMovementEnum.UserControled && oldMode == SpectatorCameraMovementEnum.ConstantDelta)
            {
                Matrix gravityOrientationMatrix = Matrix.Identity;
                //ComputeGravityAngledOrientation(out gravityOrientationMatrix);
                orientation.Up = gravityOrientationMatrix.Up;
                orientation.Forward = Vector3.Normalize(Target - Position);
                orientation.Right = Vector3.Cross(orientation.Forward, orientation.Up);
            }
        }

        #region Light

        public void UpdateLightPosition()
        {

        }

        public void TurnLightOff()
        {

        }

        #endregion

        void ICameraController.ControlCamera(Camera currentCamera)
        {
            currentCamera.SetViewMatrix(GetViewMatrix());
            Render.Render11.cameraOrientation = orientation; //Debug only!
        }

        void ICameraController.Rotate(Vector2 rotationIndicator, float rollIndicator)
        {
            Rotate(rotationIndicator, rollIndicator);
        }

        void ICameraController.RotateStopped()
        {
            RotateStopped();
        }

        void ICameraController.OnAssumeControl(ICameraController previousCameraController)
        {
         
        }

        void ICameraController.OnReleaseControl(ICameraController newCameraController)
        {
            TurnLightOff();
        }

        bool ICameraController.HandleUse()
        {
            return false;
        }

        bool ICameraController.HandlePickUp()
        {
            throw new NotImplementedException();
        }
    }
}
