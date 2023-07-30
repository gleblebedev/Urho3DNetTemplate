using System;
using System.Diagnostics;
using Urho3DNet;

namespace $safeprojectname$
{
    public class UrhoApplication: Application
    {
        private SharedPtr<GameState> _gameState;
        private SharedPtr<MainMenuState> _mainMenuState;

        public UrhoApplication(Context context) : base(context)
        {
        }

        public bool IsGameRunning => _gameState;

        public override void Setup()
        {
            EngineParameters[Urho3D.EpFullScreen] = false;
            EngineParameters[Urho3D.EpWindowResizable] = true;
            EngineParameters[Urho3D.EpWindowTitle] = "$safeprojectname$";
            EngineParameters[Urho3D.EpApplicationName] = "$safeprojectname$";
            EngineParameters[Urho3D.EpOrganizationName] = "$safeprojectname$";
            EngineParameters[Urho3D.EpFrameLimiter] = true;
            EngineParameters[Urho3D.EpConfigName] = "";

            // Run shaders via SpirV-Cross to eliminate potential driver bugs
            EngineParameters[Urho3D.EpShaderPolicyGlsl] = 0;
            EngineParameters[Urho3D.EpShaderPolicyHlsl] = 2;
            // Enable this if you need to debug translated shaders.
            //EngineParameters[Urho3D.EpShaderLogSources] = true;

            base.Setup();
        }

        public override void Start()
        {
            Context.Engine.MaxFps = 60;
            Context.AddFactoryReflection<MainMenuComponent>();
            Context.AddFactoryReflection<MainMenuState>();
            Context.AddFactoryReflection<GameState>();
            Context.AddFactoryReflection<Character>();
            Context.AddFactoryReflection<NonPlayableCharacter>();
            Context.AddFactoryReflection<Selectable>();
            Context.AddFactoryReflection<Player>();
            Context.AddFactoryReflection<DoorButton>();
            Context.AddFactoryReflection<Pickable>();
            Context.AddFactoryReflection<DoorTrigger>();

            var cache = GetSubsystem<ResourceCache>();
            var ui = GetSubsystem<RmlUI>();
            var fonts = new StringList();
            cache.Scan(fonts, "Fonts/", "*.ttf", ScanFlag.ScanFiles);
            foreach (var font in fonts) ui.LoadFont($"Fonts/{font}");
            cache.Scan(fonts, "Fonts/", "*.otf", ScanFlag.ScanFiles);
            foreach (var font in fonts) ui.LoadFont($"Fonts/{font}");

            var stateManager = Context.GetSubsystem<StateManager>();
            stateManager.FadeInDuration = 0.1f;
            stateManager.FadeOutDuration = 0.1f;
            using (SharedPtr<SplashScreen> splash = new SplashScreen(Context))
            {
                splash.Ptr.Duration = 1.0f;
                splash.Ptr.BackgroundImage = Context.ResourceCache.GetResource<Texture2D>("Images/Background.png");
                splash.Ptr.ForegroundImage = Context.ResourceCache.GetResource<Texture2D>("Images/Splash.png");
                stateManager.EnqueueState(splash);
            }

            ToMenu();

            SubscribeToEvent(E.LogMessage, OnLogMessage);

            base.Start();
        }

        public override void Stop()
        {
            _mainMenuState?.Dispose();
            _gameState?.Dispose();
            base.Stop();
        }

        /// <summary>
        ///     Transition to main menu
        /// </summary>
        public void ToMenu()
        {
            _mainMenuState = _mainMenuState ?? new MainMenuState(this);
            Context.GetSubsystem<StateManager>().EnqueueState(_mainMenuState);
        }

        /// <summary>
        ///     Transition to game
        /// </summary>
        public void ToNewGame()
        {
            _gameState?.Dispose();
            _gameState = new GameState(this);
            Context.GetSubsystem<StateManager>().EnqueueState(_gameState);
        }

        /// <summary>
        ///     Transition to game
        /// </summary>
        public void ContinueGame()
        {
            if (_gameState) Context.GetSubsystem<StateManager>().EnqueueState(_gameState);
        }

        public void Quit()
        {
            Context.Engine.Exit();
        }

        private void OnLogMessage(VariantMap args)
        {
            var logLevel = (LogLevel)args[E.LogMessage.Level].Int;
            switch (logLevel)
            {
                case LogLevel.LogError:
                    throw new ApplicationException(args[E.LogMessage.Message].String);
                default:
                    Debug.WriteLine(args[E.LogMessage.Message].String);
                    break;
            }
        }
    }
}