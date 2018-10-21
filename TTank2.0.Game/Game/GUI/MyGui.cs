using TTank20.Game.Engine;
using TTank20.Game.Game.World;
using SharpDX;
using SharpDX.Direct2D1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPresenter;
using TPresenter.Input;
using TPresenter.Profiler;
using TPresenter.Render;
using TPresenter.Render.RenderProxy;

namespace TTank20.Game.GUI
{
    public static class MyGui
    {
        //Direct render interactions from this class is questionable. Will be changed in the future (when Screen entity will be created).
        internal static DeviceContext renderContext { get { return Render11.Direct2DContext; } }

        public static void GuiHandleInputBefore()
        {
            if (MyInput.Static.IsAnyAltKeyPressed() && MyInput.Static.IsNewKeyPressed(Keys.F4))
            {
                //Closing the game thread
                return;
            }

            bool inputHandled = false;
            if (MyInput.Static.ENABLE_DEVELOPER_KEYS)
            {
                if (MyInput.Static.IsNewKeyPressed(Keys.F11) && !MyInput.Static.IsAnyAltKeyPressed() && !MyInput.Static.IsAnyShiftKeyPressed())
                {
                    //swithing render of statistic and debug info
                    MyRenderProxy.render.DrawDebugText = !MyRenderProxy.render.DrawDebugText;
                    inputHandled = true;
                }
                if (MyInput.Static.IsAnyShiftKeyPressed() && MyInput.Static.IsAnyAltKeyPressed() && MyInput.Static.IsAnyCtrlKeyPressed()
                    && MyInput.Static.IsNewKeyPressed(Keys.Home))
                    throw new InvalidOperationException("Impossible imput detected. Controller had crashed.");

                //colleting trash in memory
                if (MyInput.Static.IsNewKeyPressed(Keys.Pause) && MyInput.Static.IsAnyShiftKeyPressed())
                {
                    GC.Collect(GC.MaxGeneration);
                    inputHandled = true;
                }
                if (MyInput.Static.IsNewKeyPressed(Keys.F12))
                {
                    Session.Static.Spectator.Reset();
                }
            }
            if (!inputHandled)
            {

            }
        }

        //TODO: This method will handle imput only of screen in a proper state(not loading, showing cut scene etc.)
        public static void ScreenHandleInput()
        {
            //Here we should check if screen in a valid state.

            if (!ScreenHandleControlsInput())
            {
                //Handling screen input if it was not captured by some control.
            }
        }

        public static void GuiHandleInputAfter()
        {
            //we should check if camera movement allowed.
            bool cameraControllerMovementAllowed = true;

            float rollIndicator = MyInput.Static.GetRoll();
            Vector2 rotationIndicator = MyInput.Static.GetRotation();
            Vector3 moveIndicator = MyInput.Static.GetPositionDelta();

            //Spectator camera controller
            if (cameraControllerMovementAllowed)
                SpectatorCameraController.Static.MoveAndRotate(moveIndicator, rotationIndicator, rollIndicator);
        }

        public static void Draw()
        {
            //Direct2DContext render causes huge performance impact. Commented unit proper GUI render is created.
            //MyProfiler.BeginSubstep("Draw GUI");
            //DrawCrosshairs();
            //MyProfiler.EndSubstep();
        }

        //UI draw must not be done like this. Debug only!
        internal static void DrawCrosshairs(float scaleFactor = 1)
        {
            float centerX = Render11.Bounds.Width / 2.0f;
            float centerY = Render11.Bounds.Height / 2.0f;
            Brush color = new SolidColorBrush(Render11.Direct2DContext, Color.DarkRed);

            renderContext.BeginDraw();
            renderContext.DrawLine(
                new Vector2(centerX - 5 * scaleFactor - 10, centerY + 5 * scaleFactor + 10),
                new Vector2(centerX - 5 * scaleFactor, centerY + 5 * scaleFactor),
                color);
            renderContext.DrawLine(
                new Vector2(centerX + 5 * scaleFactor, centerY - 5 * scaleFactor),
                new Vector2(centerX + 5 * scaleFactor + 10, centerY - 5 * scaleFactor - 10),
                color);
            renderContext.DrawLine(
                new Vector2(centerX - 5 * scaleFactor - 10, centerY - 5 * scaleFactor - 10),
                new Vector2(centerX - 5 * scaleFactor, centerY - 5 * scaleFactor),
                color);
            renderContext.DrawLine(
                new Vector2(centerX + 5 * scaleFactor, centerY + 5 * scaleFactor),
                new Vector2(centerX + 5 * scaleFactor + 10, centerY + 5 * scaleFactor + 10),
                color);
            renderContext.FillEllipse(new Ellipse(new SharpDX.Mathematics.Interop.RawVector2(centerX, centerY), 1, 1), color);
            renderContext.EndDraw();
        }

        //TODO: This method will try to handle contols input. 
        private static bool ScreenHandleControlsInput()
        {
            //we need to check all screens to se, it the imput could be handled by either of them. 
            //No screens currently by the one with primitives so current solution work. Update as
            //soon as additional screens are created.
            return true;
        }
    }
}
