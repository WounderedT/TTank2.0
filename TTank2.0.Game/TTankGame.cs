using TTank20.Game;
using TTank20.Game.Engine;
using TTank20.Game.Engine.Platform;
using TTank20.Game.Engine.Platform.VideoMode;
using TTank20.Game.Game.World;
using TTank20.Game.GUI;
using SharpDX;
using SharpDX.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TPresenter;
using TPresenter.Filesystem;
using TPresenter.Game;
using TPresenter.Game.Actor;
using TPresenter.Game.Entities;
using TPresenter.Game.Interfaces;
using TPresenter.Game.Utils;
using TPresenter.Input;
using TPresenter.Profiler;
using TPresenter.Render;
using TPresenter.Render.ExternalApp;
using TPresenter.Render.GeometryStage.Model;
using TPresenter.Render.RenderProxy;
using TPresenter.Utils;
using TPresenterMath;

namespace TTank20.Game
{
    public class TTankGame : IDisposable
    {

        #region Fields And Properties

        private static long _lastFrameTimeStamp = 0;

        private IBufferedInputSource _bufferedInputSource;
        private bool _enableDevKeys = true;

        private GameWindowForm _gameWindow;  //This should not be here.
        private GameLoop _renderLoop;

        public static IntPtr WindowHandle;
        public static TTankGame Static;
        public static ViewportF ScreenViewport;

        public Camera MainCamera; //Should not be here. Move to the Session class.

        public static double SecondsSinceLastFrame { get; private set; }
        public Thread UpdateThread { get; protected set; }
        public Thread DrawThread { get; protected set; }

        #endregion

        public TTankGame()
        {
            //Basic game initialization - language, memory, threads etc.
        }

        #region Public Methods

        public void Run()
        {
            Initialize();
            RunLoop();
            EndLoop();
        }

        public void EndLoop()
        {
            MyRenderProxy.Dispose();
        }

        public void InitInput()
        {
            //Initializing default input keys for basic actions.
            Dictionary<StringId, Control> defaultGameControls = new Dictionary<StringId, Control>(StringId.Comparer);
            AddDefaultGameControl(defaultGameControls, GuiControlTypeEnum.General, ControlSpace.FORWARD, null, Keys.W);
            AddDefaultGameControl(defaultGameControls, GuiControlTypeEnum.General, ControlSpace.BACKWARD, null, Keys.S);
            AddDefaultGameControl(defaultGameControls, GuiControlTypeEnum.General, ControlSpace.STRAFE_LEFT, null, Keys.A);
            AddDefaultGameControl(defaultGameControls, GuiControlTypeEnum.General, ControlSpace.STRAFE_RIGHT, null, Keys.D);
            AddDefaultGameControl(defaultGameControls, GuiControlTypeEnum.General, ControlSpace.ROLL_LEFT, null, Keys.Q);
            AddDefaultGameControl(defaultGameControls, GuiControlTypeEnum.General, ControlSpace.ROLL_RIGHT, null, Keys.E);
            AddDefaultGameControl(defaultGameControls, GuiControlTypeEnum.General, ControlSpace.SPRINT, null, Keys.LeftShift);
            AddDefaultGameControl(defaultGameControls, GuiControlTypeEnum.General, ControlSpace.USE, null, Keys.F);
            AddDefaultGameControl(defaultGameControls, GuiControlTypeEnum.General, ControlSpace.JUMP, null, Keys.Space);
            AddDefaultGameControl(defaultGameControls, GuiControlTypeEnum.General, ControlSpace.CROUCH, null, Keys.C);
            AddDefaultGameControl(defaultGameControls, GuiControlTypeEnum.General, ControlSpace.SWITCH_WALK, null, Keys.CapsLock);
            AddDefaultGameControl(defaultGameControls, GuiControlTypeEnum.General, ControlSpace.ROTATION_LEFT, null, Keys.Left);
            AddDefaultGameControl(defaultGameControls, GuiControlTypeEnum.General, ControlSpace.ROTATION_RIGHT, null, Keys.Right);
            AddDefaultGameControl(defaultGameControls, GuiControlTypeEnum.System, ControlSpace.PAUSE_GAME, null, Keys.Pause, null);

            AddDefaultGameControl(defaultGameControls, GuiControlTypeEnum.System, ControlSpace.CONSOLE, null, Keys.OemTilde);
            AddDefaultGameControl(defaultGameControls, GuiControlTypeEnum.System, ControlSpace.RESET_POSITION, null, Keys.F12);

            MyInput.Initialize(new TPresenter.Input.DirectXInput(_bufferedInputSource, new KeysToString(), defaultGameControls, _enableDevKeys));
        }

        public void RunLoop()
        {
            _renderLoop = new GameLoop();
            //renderLoop.Run(RunFrame);
            //This will be changed. No direct render loop interaction should be done from game classes. For render debug only.
            RenderLoop.Run(_gameWindow, RunFrame);
        }

        public void RunFrame()
        {
            ProfilerStatic.Begin("Frame");
            Update();
            ProfilerStatic.BeginSubstep("Camera and render parameters update");
            Session.Static.CameraController.ControlCamera(MainCamera);
            MainCamera.Update(EngineConstants.UPDATE_STEP_SIZE_IN_SECONDS);
            MainCamera.SetViewMatrixToRender();
            ProfilerStatic.EndSubstep();

            //SpectatorCameraController.Static.GetViewMatrix().TranslationVector);

            ProfilerStatic.BeginSubstep("Creating render messages");
            foreach(IEntity entity in Scene.SceneObjects)
            {
                IDrawableEntity objectEntity = entity as IDrawableEntity;
                if (objectEntity != null)
                {
                    objectEntity.Draw();
                }
            }
            ProfilerStatic.EndSubstep();

            MyRenderProxy.render.Draw();
            MyGui.Draw();
            ProfilerStatic.BeginSubstep("Present");
            MyRenderProxy.Present();
            ProfilerStatic.EndSubstep(true);
            ProfilerStatic.EndCurrent();
            //MyProfiler.AddMemoryUsage();
        }

        #endregion

        #region Update

        protected void Update()
        {
            ProfilerStatic.BeginSubstep("Input handling", true);
            long currentTimeStamp = Stopwatch.GetTimestamp();
            long elapsedTime = currentTimeStamp - _lastFrameTimeStamp;
            _lastFrameTimeStamp = currentTimeStamp;
            SecondsSinceLastFrame = (double)elapsedTime / Stopwatch.Frequency;

            MyInput.Static.Update(_gameWindow.IsActive);

            //message processing should be done here.

            MyGui.GuiHandleInputBefore();
            MyGui.GuiHandleInputAfter();
            ProfilerStatic.EndSubstep();

            ProfilerStatic.BeginSubstep("Objects update");
            //TODO: We must not iterate over every objects in the Scene. Find correct update strategy ASAP.
            foreach (var entity in Scene.SceneObjects)
            {
                entity.Update(currentTimeStamp);
            }
            ProfilerStatic.EndSubstep();
        }

        #endregion

        /// <summary>
        /// Performs any initialization actions required in order to run.
        /// </summary>
        protected void Initialize()
        {
            FileProvider.Init();
            var renderDeviceSettings = VideoSettingsManager.Initialize();
            StartRenderComponent(renderDeviceSettings);
            InitInput();

            Scene.LoadScene(StringId.GetOrCompute("GeneratedSceneFileXml"));
            Session.Start(null);
            LoadData();
        }

        protected static void AddDefaultGameControl(Dictionary<StringId, Control> defaultGameControl, GuiControlTypeEnum controlTypeEnum, StringId controlId,
            MouseButtonsEnum? mouseButton = null, Keys? key1 = null, Keys? key2 = null)
        {
            defaultGameControl[controlId] = new Control(controlId, controlId, controlTypeEnum, mouseButton, key1, key2);
        }

        protected virtual void StartRenderComponent(RenderDeviceSettings renderDeviceSettings)
        {
            _gameWindow = InitializeRenderThread(renderDeviceSettings);
            MyRenderProxy.Initialize(_gameWindow);
            //MyRenderProxy.Initialize(InitializeRenderThread());
            //renderThread = new Thread()
            //GameRenderComponent.StartSync(InitializeRenderThread());
            //GameRenderComponent.RenderThread.SizeChanged += RenderThreadSizeChanged;
            //GameRenderComponent.RenderThread.BeforeDraw += RenderThreadBeforeDraw;
            //gameWindow.Activate();
            //gameWindow.Show();
        }

        protected virtual GameWindowForm InitializeRenderThread(RenderDeviceSettings renderDeviceSettings)
        {
            DrawThread = Thread.CurrentThread;
            var form = new GameWindowForm();
            WindowHandle = form.Handle;
            form.FormClosed += OnFormClose;

            _bufferedInputSource = form;
            //windowCreatedEvent.Set();

            form.Text = "TTank 2.0 Rendering Tutorial";
            form.Width = renderDeviceSettings.BackBufferWidth;
            form.Height = renderDeviceSettings.BackBufferHeight;
            form.ShowCursor = false; //This is mutable value. Should be set by each screen. Debug only.
            form.WindowState = (System.Windows.Forms.FormWindowState)renderDeviceSettings.WindowMode;  //This is mutable value. Should be set by game settings. Debug only.
            //form.FormClosed += (o, e) => ExitThreadSafe();
            Action showCursor = () =>
            {
                if (!form.IsDisposed)
                    form.ShowCursor = true;
            };

            Action hideCursor = () =>
            {
                if (!form.IsDisposed)
                    form.ShowCursor = false;
            };

            //setMouseVisible = (b) =>
            //{
            //    var component = GameRenderComponent;
            //    if (component != null)
            //    {
            //        var renderThread = component.RenderThread;
            //        if (renderThread != null)
            //        {
            //            var renderThread = component.RenderThread;
            //            if (renderThread != null)
            //                renderThread.Invoke(b ? showCursor : hideCursor);
            //        }
            //    }
            //};
            return form;
        }

        internal void UpdateMouseCapture()
        {

        }

        internal void OnFormClose(object sender, System.Windows.Forms.FormClosedEventArgs args)
        {
            _renderLoop.IsDone = true;
            EndLoop();
        }

        #region Data Loading

        private void LoadData()
        {
            if (MyInput.Static != null)
                MyInput.Static.InitializeInput(WindowHandle);

            //Camera initialization should be performed some place else.
            float fov = MathHelper.ToRadians(70);
            ScreenViewport = MyRenderProxy.render.Viewport;
            MainCamera = new Camera(fov, ScreenViewport);
        }

        public void Dispose()
        {
            MyRenderProxy.Dispose();
        }

        #endregion
    }
}
