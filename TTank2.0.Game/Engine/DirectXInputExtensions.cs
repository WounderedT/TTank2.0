using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPresenter.Game;
using TPresenter.Input;

namespace TTank20.Game.Engine
{
    public static class DirectXInputExtensions
    {
        //Rotation constants. All values are empirical
        public const float MOUSE_ROTATION_INDICATOR_MULTIPLIER = 0.075f;
        public const float ROTATION_INDICATOR_MULTIPLIER = 0.0015f;

        public static float GetRoll(this DirectXInput input)
        {
            var roll = ControllerHelper.IsControlAnalog(ControlSpace.ROLL_RIGHT) - ControllerHelper.IsControlAnalog(ControlSpace.ROLL_LEFT);
            return roll;
        }

        public static Vector2 GetRotation(this DirectXInput input)
        {
            Vector2 rotationVector = new Vector2(input.GetMouseYForGamePlay(), input.GetMouseXForGamePlay()) * MOUSE_ROTATION_INDICATOR_MULTIPLIER;

            rotationVector.Y -= ControllerHelper.IsControlAnalog(ControlSpace.ROTATION_LEFT);
            rotationVector.Y += ControllerHelper.IsControlAnalog(ControlSpace.ROTATION_RIGHT);

            rotationVector *= EngineConstants.UPDATE_STEPS_PER_SECOND * ROTATION_INDICATOR_MULTIPLIER;

            return rotationVector;
        }

        public static Vector3 GetPositionDelta(this DirectXInput input)
        {
            Vector3 moveIdicator = Vector3.Zero;

            moveIdicator.X = ControllerHelper.IsControlAnalog(ControlSpace.STRAFE_RIGHT) - ControllerHelper.IsControlAnalog(ControlSpace.STRAFE_LEFT);
            moveIdicator.Y = ControllerHelper.IsControlAnalog(ControlSpace.JUMP) - ControllerHelper.IsControlAnalog(ControlSpace.CROUCH);
            moveIdicator.Z = ControllerHelper.IsControlAnalog(ControlSpace.FORWARD) - ControllerHelper.IsControlAnalog(ControlSpace.BACKWARD);

            return moveIdicator;
        }
    }
}
