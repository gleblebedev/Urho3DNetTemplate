using Urho3DNet;

namespace Urho3DNetTemplate
{
    public class GameState : ApplicationState
    {
        private readonly UrhoApplication _app;
        protected readonly SharedPtr<Scene> _scene;
        private readonly Node _cameraNode;
        private readonly Viewport _viewport;
        private readonly Node _character;
        private readonly Node _cameraRoot;

        public GameState(UrhoApplication app) : base(app.Context)
        {
            MouseMode = MouseMode.MmAbsolute;
            IsMouseVisible = false;

            _app = app;
            _scene = Context.CreateObject<Scene>();
            _scene.Ptr.LoadXML("Scenes/Sample.xml");

            var nodeList = _scene.Ptr.GetChildrenWithComponent(nameof(KinematicCharacterController), true);
            foreach (var node in nodeList)
            {
                SetupCharacter(node);
                node.CreateComponent<NonPlayableCharacter>();
            }

            _character = _scene.Ptr.CreateChild();
            _character.Position = new Vector3(0, 0.2f, 0);
            _character.CreateComponent<PrefabReference>().SetPrefab(Context.ResourceCache.GetResource<PrefabResource>("Models/Characters/YBot/YBot.prefab"));
            var player = SetupCharacter(_character);
            _cameraRoot = _character.CreateChild(); 
            var cameraPrefab = _cameraRoot.CreateComponent<PrefabReference>();
            cameraPrefab.SetPrefab(Context.ResourceCache.GetResource<PrefabResource>("Models/Characters/Camera.prefab"));
            cameraPrefab.Inline(PrefabInlineFlag.None);
            _cameraNode = _character.GetComponent<Camera>(true).Node;
            _character.CreateComponent<MoveAndOrbitController>().InputMap = Context.ResourceCache.GetResource<InputMap>("Input/MoveAndOrbit.inputmap");
            player.CameraYaw = _character.GetChild("CameraYawPivot", true);
            player.CameraPitch = _character.GetChild("CameraPitchPivot", true);

            _viewport = Context.CreateObject<Viewport>();
            _viewport.Camera = _cameraNode?.GetComponent<Camera>();
            _viewport.Scene = _scene;
            SetViewport(0, _viewport);
            _scene.Ptr.IsUpdateEnabled = false;
        }

        private Character SetupCharacter(Node _character)
        {
            var player = _character.CreateComponent<Character>();
            player.CharacterController = _character.GetComponent<KinematicCharacterController>();
            player.AnimationController = _character.GetComponent<AnimationController>(true);
            player.ModelPivot = _character.GetChild("ModelPivot");
            player.Idle = Context.ResourceCache.GetResource<Animation>("Animations/Idle.ani");
            player.Walk = Context.ResourceCache.GetResource<Animation>("Animations/Walking.ani");
            return player;

        }

        public override void Activate(StringVariantMap bundle)
        {
            SubscribeToEvent(E.KeyUp, HandleKeyUp);

            _scene.Ptr.IsUpdateEnabled = true;

            base.Activate(bundle);
        }

        public override void Deactivate()
        {
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
                    _app.ToMenu();
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