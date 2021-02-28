using System;
using Urho3DNet;

namespace $safeprojectname$
{
    public class UrhoApplication: Application
    {
        protected Scene _scene;
        private Node _cameeraNode;
        private Viewport _viewport;

        public UrhoApplication(Context context) : base(context)
        {
        }

        public override void Setup()
        {
            EngineParameters[Urho3D.EpFullScreen] = false;
            EngineParameters[Urho3D.EpWindowResizable] = true;
            EngineParameters[Urho3D.EpWindowTitle] = "$safeprojectname$";
            
            base.Setup();
        }

        public override void Start()
        {
            SubscribeToEvent(E.LogMessage, OnLogMessage);
            Context.Input.SetMouseMode(MouseMode.MmFree);
            Context.Input.SetMouseVisible(true);

            _scene = Context.CreateObject<Scene>();
            _scene.LoadXML("Scenes/TeapotScene.xml");

            _cameeraNode = _scene.GetChild("MainCamera", true);

            _viewport = Context.CreateObject<Viewport>();
            _viewport.Camera = _cameeraNode?.GetComponent<Camera>();
            _viewport.Scene = _scene;
            Context.Renderer.SetViewport(0, _viewport);
            
            SubscribeToEvent(E.KeyUp, HandleKeyUp);
            
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

        private void HandleKeyUp(VariantMap args)
        {
            var key = (Key) args[E.KeyUp.Key].Int;
            switch (key)
            {
                case Key.KeyEscape:
                case Key.KeyBackspace:
                    Context.Engine.Exit();
                    return;
            }
        }

        public override void Stop()
        {
            base.Stop();
        }

        protected override void Dispose(bool disposing)
        {
            _scene?.Dispose();
            base.Dispose(disposing);
        }
    }
}
