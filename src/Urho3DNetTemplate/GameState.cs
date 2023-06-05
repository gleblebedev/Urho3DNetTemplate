using Urho3DNet;

namespace Urho3DNetTemplate
{
    public class GameState : ApplicationState
    {
        protected readonly SharedPtr<Scene> _scene;
        private readonly Node _cameraNode;
        private readonly Viewport _viewport;

        public GameState(Context context) : base(context)
        {
            _scene = Context.CreateObject<Scene>();
            _scene.Ptr.LoadXML("Scenes/TeapotScene.xml");

            _cameraNode = _scene.Ptr.GetChild("MainCamera", true);
            _cameraNode.CreateComponent<FreeFlyController>();

            _viewport = Context.CreateObject<Viewport>();
            _viewport.Camera = _cameraNode?.GetComponent<Camera>();
            _viewport.Scene = _scene;
            SetViewport(0, _viewport);
            _scene.Ptr.IsUpdateEnabled = false;
        }

        public override void Activate(StringVariantMap bundle)
        {
            Context.Input.SetMouseMode(MouseMode.MmFree);
            Context.Input.SetMouseVisible(true);

            SubscribeToEvent(E.KeyUp, HandleKeyUp);

            _scene.Ptr.IsUpdateEnabled = true;

            base.Activate(bundle);
        }

        public override void Deactivate()
        {
            _scene.Ptr.IsUpdateEnabled = false;

            base.Deactivate();
        }

        private void HandleKeyUp(VariantMap args)
        {
            var key = (Key)args[E.KeyUp.Key].Int;
            switch (key)
            {
                case Key.KeyEscape:
                case Key.KeyBackspace:
                    Context.Engine.Exit();
                    return;
            }
        }

        protected override void Dispose(bool disposing)
        {
            _scene?.Dispose();

            base.Dispose(disposing);
        }
    }
}