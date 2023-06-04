using System;
using Urho3DNet;

namespace $safeprojectname$
{
    public class UrhoApplication: Application
    {
        protected Scene _scene;
        private Node _cameraNode;
        private Viewport _viewport;

        public UrhoApplication(Context context) : base(context)
        {
        }

        public override void Setup()
        {
            EngineParameters[Urho3D.EpFullScreen] = false;
            EngineParameters[Urho3D.EpWindowResizable] = true;
            EngineParameters[Urho3D.EpWindowTitle] = "$safeprojectname$";
            EngineParameters[Urho3D.EpApplicationName] = "$safeprojectname$";
            EngineParameters[Urho3D.EpOrganizationName] = "$safeprojectname$";
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
            var stateManager = this.Context.GetSubsystem<StateManager>();
            stateManager.FadeInDuration = 0.1f;
            stateManager.FadeOutDuration = 0.1f;
            using (SharedPtr<SplashScreen> splash = new SplashScreen(Context))
            {
                splash.Ptr.Duration = 1.0f;
                splash.Ptr.BackgroundImage = Context.ResourceCache.GetResource<Texture2D>("Images/Splash.png");
                splash.Ptr.ForegroundImage = Context.ResourceCache.GetResource<Texture2D>("Images/Splash.png");
                stateManager.EnqueueState(splash);
            }

            stateManager.EnqueueState(new GameState(Context));

            SubscribeToEvent(E.LogMessage, OnLogMessage);
            
            base.Start();
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
    }
}
