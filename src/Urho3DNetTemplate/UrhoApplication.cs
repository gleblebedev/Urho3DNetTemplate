using System;
using Urho3DNet;

namespace Urho3DNetTemplate
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
            EngineParameters[Urho3D.EpWindowTitle] = "Urho3DNetTemplate";
            EngineParameters[Urho3D.EpApplicationName] = "Urho3DNetTemplate";
            EngineParameters[Urho3D.EpOrganizationName] = "Urho3DNetTemplate";
            EngineParameters[Urho3D.EpConfigName] = "";

            // Run shaders via SpirV-Cross to eliminate potential driver bugs
            EngineParameters[Urho3D.EpShaderPolicyGlsl] = 2;
            EngineParameters[Urho3D.EpShaderPolicyHlsl] = 2;
            // Enable this if you need to debug translated shaders.
            //EngineParameters[Urho3D.EpShaderLogSources] = true;

            base.Setup();
        }

        public override void Start()
        {
            Context.AddFactoryReflection<MainMenuComponent>();
            Context.AddFactoryReflection<MainMenuState>();
            Context.AddFactoryReflection<GameState>();

            var cache = GetSubsystem<ResourceCache>();
            var ui = GetSubsystem<RmlUI>();
            StringList fonts = new StringList();
            cache.Scan(fonts, "Fonts/", "*.ttf", ScanFlag.ScanFiles);
            foreach (var font in fonts)
            {
                ui.LoadFont($"Fonts/{font}");
            }
            cache.Scan(fonts, "Fonts/", "*.otf", ScanFlag.ScanFiles);
            foreach (var font in fonts)
            {
                ui.LoadFont($"Fonts/{font}");
            }

            var stateManager = this.Context.GetSubsystem<StateManager>();
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

        /// <summary>
        /// Transition to main menu
        /// </summary>
        public void ToMenu()
        {
            _mainMenuState = _mainMenuState ?? new MainMenuState(this);
            Context.GetSubsystem<StateManager>().EnqueueState(_mainMenuState);
        }

        public override void Stop()
        {
            _mainMenuState?.Dispose();
            _gameState?.Dispose();
            base.Stop();
        }

        /// <summary>
        /// Transition to game
        /// </summary>
        public void ToNewGame()
        {
            _gameState?.Dispose();
            _gameState = new GameState(this);
            Context.GetSubsystem<StateManager>().EnqueueState(_gameState);
        }

        /// <summary>
        /// Transition to game
        /// </summary>
        public void ContinueGame()
        {
            if (_gameState)
            {
                Context.GetSubsystem<StateManager>().EnqueueState(_gameState);
            }
        }

        private void OnLogMessage(VariantMap args)
        {
            var logLevel = (LogLevel)args[E.LogMessage.Level].Int;
            switch (logLevel)
            {
                case LogLevel.LogError:
                    throw new ApplicationException(args[E.LogMessage.Message].String);
            }
        }

        public void Quit()
        {
            Context.Engine.Exit();
        }
    }
}
