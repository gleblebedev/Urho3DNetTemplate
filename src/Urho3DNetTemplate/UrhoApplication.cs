using System;
using System.Collections.Generic;
using System.Diagnostics;
using Urho3DNet;

namespace Urho3DNetTemplate
{
    /// <summary>
    ///     This class represents an Urho3D application.
    /// </summary>
    [Preserve(AllMembers = true)]
    public class UrhoApplication : Application
    {
        /// <summary>
        ///     Safe pointer to debug HUD.
        /// </summary>
        private SharedPtr<DebugHud> _debugHud;

        /// <summary>
        ///     Safe pointer to game screen.
        /// </summary>
        private SharedPtr<GameState> _gameState;

        /// <summary>
        ///     Safe pointer to menu screen.
        /// </summary>
        private SharedPtr<MainMenuState> _mainMenuState;

        /// <summary>
        ///     Safe pointer to settings screen.
        /// </summary>
        private SharedPtr<SettingsMenuState> _settingsMenuState;

        /// <summary>
        ///     Application state manager.
        /// </summary>
        private StateStack _stateStack;

        public UrhoApplication(Context context) : base(context)
        {
        }

        /// <summary>
        ///     Gets a value indicating whether the game is running.
        /// </summary>
        public bool IsGameRunning => _gameState;

        /// <summary>
        ///     Gets or sets the settings file.
        /// </summary>
        public SettingFile Settings { get; set; }


        /// <summary>
        ///     Setup application.
        ///     This method is executed before most of the engine system initialized.
        /// </summary>
        public override void Setup()
        {
            // Set up engine parameters
            EngineParameters[Urho3D.EpFullScreen] = false;
            EngineParameters[Urho3D.EpWindowResizable] = true;
            EngineParameters[Urho3D.EpWindowTitle] = "Urho3DNetTemplate";
            EngineParameters[Urho3D.EpApplicationName] = "Urho3DNetTemplate";
            EngineParameters[Urho3D.EpOrganizationName] = "Urho3DNetTemplate";
            EngineParameters[Urho3D.EpFrameLimiter] = true;
            EngineParameters[Urho3D.EpConfigName] = "";

            // Run shaders via SpirV-Cross to eliminate potential driver bugs
            EngineParameters[Urho3D.EpShaderPolicyGlsl] = 0;
            EngineParameters[Urho3D.EpShaderPolicyHlsl] = 2;
            // Enable this if you need to debug translated shaders.
            //EngineParameters[Urho3D.EpShaderLogSources] = true;

            base.Setup();
        }

        /// <summary>
        ///     Start application.
        /// </summary>
        public override void Start()
        {
            // Subscribe for log messages.
            SubscribeToEvent(E.LogMessage, OnLogMessage);

            //var engine = GetSubsystem<Engine>();
            //List<string> loadedPlugins = new List<string>();
            //var pluginManager = GetSubsystem<PluginManager>();
            //pluginManager.SetPluginsLoaded(loadedPlugins);
            //pluginManager.StartApplication();

            // Load settings.
            Settings = SettingFile.Load(Context);

            // Limit frame rate tp 60 FPS as a workaround for kinematic character controller movement.
            Context.Engine.MaxFps = 60;

            // Add factory reflections
            Context.RegisterFactories(GetType().Assembly);

#if DEBUG
            // Setup Debug HUD when building in Debug configuration.
            _debugHud = Context.Engine.CreateDebugHud();
            _debugHud.Ptr.Mode = DebugHudMode.DebughudShowAll;
#endif

            _stateStack = new StateStack(Context.GetSubsystem<StateManager>());

            // Loads all fonts from the resource cache and adds them to the RmlUI.
            var cache = GetSubsystem<ResourceCache>();
            var ui = GetSubsystem<RmlUI>();
            var fonts = new StringList();
            // Scan for .ttf files and load them
            cache.Scan(fonts, "Fonts/", "*.ttf", ScanFlag.ScanFiles);
            foreach (var font in fonts) ui.LoadFont($"Fonts/{font}");
            // Scan for .otf files and load them
            cache.Scan(fonts, "Fonts/", "*.otf", ScanFlag.ScanFiles);
            foreach (var font in fonts) ui.LoadFont($"Fonts/{font}");

            // Setup state manager.
            var stateManager = Context.GetSubsystem<StateManager>();
            stateManager.FadeInDuration = 0.1f;
            stateManager.FadeOutDuration = 0.1f;

            // Setup end enqueue splash screen.
            using (SharedPtr<SplashScreen> splash = new SplashScreen(Context))
            {
                splash.Ptr.Duration = 1.0f;
                splash.Ptr.BackgroundImage = Context.ResourceCache.GetResource<Texture2D>("Images/Background.png");
                splash.Ptr.ForegroundImage = Context.ResourceCache.GetResource<Texture2D>("Images/Splash.png");
                stateManager.EnqueueState(splash);
            }


            // Crate end enqueue main menu screen.
            _mainMenuState = _mainMenuState ?? new MainMenuState(this);
            _stateStack.Push(_mainMenuState);

            base.Start();
        }

        public override void Stop()
        {
            _mainMenuState?.Dispose();
            _gameState?.Dispose();
            _debugHud?.Dispose();
            base.Stop();
        }

        /// <summary>
        ///     Transition to settings menu
        /// </summary>
        public void ToSettings()
        {
            _settingsMenuState = _settingsMenuState ?? new SettingsMenuState(this);
            _stateStack.Push(_settingsMenuState);
        }

        /// <summary>
        ///     Transition to game
        /// </summary>
        public void ToNewGame()
        {
            _gameState?.Dispose();
            _gameState = new GameState(this);
            _stateStack.Push(_gameState);
        }

        /// <summary>
        ///     Transition to game
        /// </summary>
        public void ContinueGame()
        {
            if (_gameState) _stateStack.Push(_gameState);
            ;
        }

        public void Quit()
        {
            Context.Engine.Exit();
        }

        public void HandleBackKey()
        {
            if (_stateStack.State == _mainMenuState.Ptr)
            {
                if (IsGameRunning)
                    ContinueGame();
                else
                    Quit();
            }
            else
            {
                _stateStack.Pop();
            }
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