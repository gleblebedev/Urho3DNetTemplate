using System;
using Urho3DNet;

namespace $safeprojectname$
{
    public class GameState : ApplicationState
    {
        private readonly UrhoApplication _app;
        protected readonly SharedPtr<Scene> _scene;
        protected readonly SharedPtr<Sprite> _cross;
        private readonly Node _cameraNode;
        private readonly Viewport _viewport;
        private readonly Node _character;
        private readonly Node _cameraRoot;

        public GameState(UrhoApplication app) : base(app.Context)
        {
            MouseMode = MouseMode.MmRelative;
            IsMouseVisible = false;

            var inputMap = Context.ResourceCache.GetResource<InputMap>("Input/MoveAndOrbit.inputmap");

            _app = app;
            _scene = Context.CreateObject<Scene>();
            _scene.Ptr.LoadXML("Scenes/Sample.xml");

            var boxes = _scene.Ptr.GetChildrenWithTag("InteractableBox", true);
            foreach (var box in boxes)
            {
                box.CreateComponent<InteractableBox>();
            }

            var nodeList = _scene.Ptr.GetChildrenWithComponent(nameof(KinematicCharacterController), true);
            foreach (var node in nodeList)
            {
                SetupCharacter(node);
                node.CreateComponent<NonPlayableCharacter>();
            }

            _character = _scene.Ptr.CreateChild();
            _character.Position = new Vector3(0, 0.2f, 0);
            _character.CreateComponent<PrefabReference>().SetPrefab(Context.ResourceCache.GetResource<PrefabResource>("Models/Characters/YBot/YBot.prefab"));
            var character = SetupCharacter(_character);
            var player = _character.CreateComponent<Player>();
            player.InputMap = inputMap;
            player.AttractionTarget = character.ModelPivot.CreateChild("AttractionTarget");
            player.AttractionTarget.Position = new Vector3(0, 1.0f, 1.0f);
            player.AttractionTarget.CreateComponent<RigidBody>();
            player.Constraint = player.AttractionTarget.CreateComponent<Constraint>();
            player.Constraint.ConstraintType = ConstraintType.ConstraintSlider;
            _cameraRoot = _character.CreateChild(); 
            var cameraPrefab = _cameraRoot.CreateComponent<PrefabReference>();
            cameraPrefab.SetPrefab(Context.ResourceCache.GetResource<PrefabResource>("Models/Characters/Camera.prefab"));
            cameraPrefab.Inline(PrefabInlineFlag.None);
            player.Camera = _character.GetComponent<Camera>(true);
            _cameraNode = player.Camera.Node;
            _character.CreateComponent<MoveAndOrbitController>().InputMap = inputMap;
            character.CameraYaw = _character.GetChild("CameraYawPivot", true);
            character.CameraPitch = _character.GetChild("CameraPitchPivot", true);
            character.CameraNode = _cameraNode;
            _viewport = Context.CreateObject<Viewport>();
            _viewport.Camera = _cameraNode?.GetComponent<Camera>();
            _viewport.Scene = _scene;
            SetViewport(0, _viewport);
            _scene.Ptr.IsUpdateEnabled = false;

            _cross = SharedPtr.MakeShared<Sprite>(Context);
            var crossTexture = ResourceCache.GetResource<Texture2D>("Images/Cross.png");
            _cross.Ptr.Texture = crossTexture;
            _cross.Ptr.Size = new IntVector2(64, 64);
            _cross.Ptr.VerticalAlignment = VerticalAlignment.VaCenter;
            _cross.Ptr.HorizontalAlignment = HorizontalAlignment.HaCenter;
            _cross.Ptr.HotSpot = new IntVector2(32, 32);
            UIRoot.AddChild(_cross);
        }

        private Character SetupCharacter(Node _character)
        {
            var player = _character.CreateComponent<Character>();
            player.CharacterController = _character.GetComponent<KinematicCharacterController>();
            player.CameraCollisionMask = UInt32.MaxValue & ~player.CharacterController.CollisionLayer;
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