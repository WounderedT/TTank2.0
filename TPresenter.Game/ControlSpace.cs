using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPresenter;

namespace TPresenter.Game
{
    public static class ControlSpace
    {
        public static readonly StringId FORWARD = StringId.GetOrCompute("FORWARD");
        public static readonly StringId BACKWARD = StringId.GetOrCompute("BACKWARD");
        public static readonly StringId STRAFE_LEFT = StringId.GetOrCompute("STRAFE_LEFT");
        public static readonly StringId STRAFE_RIGHT = StringId.GetOrCompute("STRAFE_RIGHT");
        public static readonly StringId ROLL_LEFT = StringId.GetOrCompute("ROLL_LEFT");
        public static readonly StringId ROLL_RIGHT = StringId.GetOrCompute("ROLL_RIGHT");
        public static readonly StringId SPRINT = StringId.GetOrCompute("SPRINT");
        public static readonly StringId PRIMARY_ACTION = StringId.GetOrCompute("PRIMARY_ACTION");
        public static readonly StringId SECONDARY_ACTION = StringId.GetOrCompute("SECONDARY_ACTION"); 
        public static readonly StringId JUMP = StringId.GetOrCompute("JUMP"); 
        public static readonly StringId CROUCH = StringId.GetOrCompute("CROUCH"); 
        public static readonly StringId SWITCH_WALK = StringId.GetOrCompute("SWITCH_WALK");
        public static readonly StringId USE = StringId.GetOrCompute("USE"); // interact
        public static readonly StringId PICK_UP = StringId.GetOrCompute("PICK_UP"); // pick into inventory
        public static readonly StringId TERMINAL = StringId.GetOrCompute("TERMINAL");
        public static readonly StringId INVENTORY = StringId.GetOrCompute("INVENTORY");
        public static readonly StringId ROTATION_LEFT = StringId.GetOrCompute("ROTATION_LEFT");
        public static readonly StringId ROTATION_RIGHT = StringId.GetOrCompute("ROTATION_RIGHT");
        public static readonly StringId HEADLIGHTS = StringId.GetOrCompute("HEADLIGHTS");
        public static readonly StringId PAUSE_GAME = StringId.GetOrCompute("PAUSE_GAME");

        //DEBUG CONTROL
        public static readonly StringId RESET_POSITION = StringId.GetOrCompute("RESET_POSITION");
        public static readonly StringId CONSOLE = StringId.GetOrCompute("CONSOLE");
    }
}
