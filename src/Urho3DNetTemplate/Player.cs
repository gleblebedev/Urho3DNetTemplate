using Urho3DNet;

namespace Urho3DNetTemplate
{
    public class Player : LogicComponent
    {
        public Camera Camera { get; set; }

        public Node SelectedNode { get; set; }

        public Player(Context context) : base(context)
        {
            UpdateEventMask = UpdateEvent.UseUpdate;
            _raycastResult = new PhysicsRaycastResult();
        }

        public override void Update(float timeStep)
        {
            base.Update(timeStep);

            var world = Scene.GetComponent<PhysicsWorld>();
            world.RaycastSingle(_raycastResult, new Ray(Camera.Node.WorldPosition, Camera.Node.WorldDirection), 4.0f - Camera.Node.Position.Z);
            var selectedNode = _raycastResult.Body?.Node;
            if (SelectedNode != selectedNode)
            {
                if (SelectedNode != null)
                {
                    SelectedNode.SendEvent("Unselected", Context.EventDataMap);
                }
                SelectedNode = selectedNode;
                if (SelectedNode != null)
                {
                    SelectedNode.SendEvent("Selected", Context.EventDataMap);
                }
            }
        }

        private PhysicsRaycastResult _raycastResult;
    }
}