using Urho3DNet;

namespace $safeprojectname$
{
    public class MainMenuState : ApplicationState
    {
        protected readonly SharedPtr<Scene> _scene;
        private readonly UrhoApplication _app;
        private readonly Viewport _viewport;
        private readonly MainMenuComponent _uiComponent;
        private readonly RmlUI _ui;

        public MainMenuState(UrhoApplication app) : base(app.Context)
        {
            MouseMode = MouseMode.MmFree;
            IsMouseVisible = true;

            _ui = Context.GetSubsystem<RmlUI>();
            _app = app;
            _scene = Context.CreateObject<Scene>();
            var scene = _scene.Ptr;
            scene.CreateComponent<Octree>();
            var zone = scene.CreateComponent<Zone>();
            zone.FogColor = Color.Blue;
            zone.SetBoundingBox(new BoundingBox(-100, 100));
            var cameraNode = scene.CreateChild();
            var camera = cameraNode.CreateComponent<Camera>();
            _viewport = Context.CreateObject<Viewport>();
            _viewport.Scene = _scene;
            _viewport.Camera = camera;
            SetViewport(0, _viewport);
            _uiComponent = scene.CreateComponent<MainMenuComponent>();
            _uiComponent.Application = _app;
            _uiComponent.State = this;
            _uiComponent.SetResource("UI/MainMenu.rml");
            Deactivate();
        }

        public override void Activate(StringVariantMap bundle)
        {
            _uiComponent.IsEnabled = true;
            _scene.Ptr.IsUpdateEnabled = true;
            _uiComponent.UpdateProperties();
            SubscribeToEvent(E.KeyUp, HandleKeyUp);
            base.Activate(bundle);
        }

        public override void Deactivate()
        {
            _uiComponent.IsEnabled = false;
            _scene.Ptr.IsUpdateEnabled = false;
            UnsubscribeFromEvent(E.KeyUp);
            base.Deactivate();
        }

        private void HandleKeyUp(VariantMap args)
        {
            var key = (Key)args[E.KeyUp.Key].Int;
            switch (key)
            {
                case Key.KeyEscape:
                case Key.KeyBackspace:
                    if (_app.IsGameRunning)
                        _app.ContinueGame();
                    else
                        _app.Quit();
                    return;
            }
        }

        public void OnNewGame()
        {
            _app.ToNewGame();
        }

        public void OnExit()
        {
            _app.Quit();
        }

        public void OnContinue()
        {
            _app.ContinueGame();
        }
    }
}