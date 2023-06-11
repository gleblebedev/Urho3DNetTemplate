using System.ComponentModel;
using Urho3DNet;

namespace $safeprojectname$
{
    public class MainMenuState : ApplicationState
    {
        private readonly UrhoApplication _app;
        protected readonly SharedPtr<Scene> _scene;
        private readonly Viewport _viewport;
        private readonly RmlUIComponent _uiComponent;
        private readonly RmlUI _ui;

        public MainMenuState(UrhoApplication app) : base(app.Context)
        {
            _ui = Context.GetSubsystem<RmlUI>();
            _app = app;
            _scene = Context.CreateObject<Scene>();
            var scene = _scene.Ptr;
            scene.CreateComponent<Octree>();
            var zone = scene.CreateComponent<Zone>();
            zone.FogColor = Color.Blue;
            zone.SetBoundingBox(new BoundingBox(-100,100));
            var cameraNode =  scene.CreateChild();
            var camera = cameraNode.CreateComponent<Camera>();
            _viewport = Context.CreateObject<Viewport>();
            _viewport.Scene = _scene;
            _viewport.Camera = camera;
            SetViewport(0, _viewport);
            _uiComponent = scene.CreateComponent<RmlUIComponent>();
            _uiComponent.SetResource("UI/MainMenu.rml");
            scene.IsUpdateEnabled = false;
            _uiComponent.IsEnabled = false;
        }

        public override void Activate(StringVariantMap bundle)
        {
            _uiComponent.IsEnabled = true;
            _scene.Ptr.IsUpdateEnabled = true;
            SubscribeToEvent("Exit", _ui, OnExit);
            SubscribeToEvent("Game", _ui, OnGame);
            base.Activate(bundle);
        }

        private void OnExit(VariantMap obj)
        {
            Context.Engine.Exit();
        }

        private void OnGame(VariantMap obj)
        {
            _app.ToGame();
        }

        public override void Deactivate()
        {
            _uiComponent.IsEnabled = false;
            _scene.Ptr.IsUpdateEnabled = false;
            base.Deactivate();
        }
    }
}