using Urho3DNet;

namespace Urho3DNetTemplate
{
    public class Player : LogicComponent
    {
        public Camera Camera { get; set; }

        public Node SelectedNode
        {
            get { return _selectedNode; }
            set
            {
                if (_selectedNode != value)
                {
                    if (_selectedNode != null)
                    {
                        _selectedNode.SendEvent("Unselected", Context.EventDataMap);
                    }

                    _selectedNode = value;
                    if (_selectedNode != null)
                    {
                        _selectedNode.SendEvent("Selected", Context.EventDataMap);
                    }
                }
            }
        }

        public Node AttractionTarget { get; set; }

        public RigidBody BodyInArms { get; set; }

        public Constraint Constraint { get; set; }

        public InputMap InputMap { get; set; }

        public Player(Context context) : base(context)
        {
            UpdateEventMask = UpdateEvent.UseUpdate | UpdateEvent.UseFixedupdate;
            _raycastResult = new PhysicsRaycastResult();
        }

        public override void Update(float timeStep)
        {
            base.Update(timeStep);

            var usePressed = InputMap.Evaluate("Use") > 0.5f;
            if (usePressed != _usePressed)
            {
                _usePressed = usePressed;
                if (_usePressed)
                {
                    BodyInArms = SelectedNode?.GetComponent<RigidBody>();
                    if (BodyInArms != null && BodyInArms.Mass > 0)
                    {
                        Constraint.OtherBody = BodyInArms;
                        SelectedNode = null;
                    }
                }
                else
                {
                    Constraint.OtherBody = null;
                    BodyInArms = null;
                }
            }


            if (BodyInArms == null)
            {
                if (InputMap.Evaluate("Select") < 0.5f)
                {

                }
                var world = Scene.GetComponent<PhysicsWorld>();
                world.RaycastSingle(_raycastResult, new Ray(Camera.Node.WorldPosition, Camera.Node.WorldDirection),
                    4.0f - Camera.Node.Position.Z);
                var selectedNode = _raycastResult.Body?.Node;
                SelectedNode = selectedNode;
            }
        }

        public override void FixedUpdate(float timeStep)
        {
            //if (BodyInArms != null && BodyInArms.Mass > 0)
            //{
            //    BodyInArms.ApplyImpulse((AttractionTarget.WorldPosition - BodyInArms.Node.WorldPosition).Normalized * 10);
            //}
        }

        private PhysicsRaycastResult _raycastResult;
        private bool _usePressed;
        private Node _selectedNode;
    }
}