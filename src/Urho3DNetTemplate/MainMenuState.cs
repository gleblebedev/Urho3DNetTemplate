using Urho3DNet;

namespace Urho3DNetTemplate
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
            _uiComponent.SetResource("UI/MainMenu.rml");
            Deactivate();
        }

        public override void Activate(StringVariantMap bundle)
        {
            _uiComponent.IsEnabled = true;
            _scene.Ptr.IsUpdateEnabled = true;
            _uiComponent.UpdateProperties();
            SubscribeToEvent("Exit", _ui, OnExit);
            SubscribeToEvent("NewGame", _ui, OnNewGame);
            SubscribeToEvent("Continue", _ui, OnContinue);
            SubscribeToEvent(E.KeyUp, HandleKeyUp);
            base.Activate(bundle);
        }

        public override void Deactivate()
        {
            _uiComponent.IsEnabled = false;
            _scene.Ptr.IsUpdateEnabled = false;
            UnsubscribeFromEvent("Exit");
            UnsubscribeFromEvent("NewGame");
            UnsubscribeFromEvent("Continue");
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

        private void OnNewGame(VariantMap obj)
        {
            _app.ToNewGame();
        }

        private void OnExit(VariantMap obj)
        {
            _app.Quit();
        }

        private void OnContinue(VariantMap obj)
        {
            _app.ContinueGame();
        }
    }
}