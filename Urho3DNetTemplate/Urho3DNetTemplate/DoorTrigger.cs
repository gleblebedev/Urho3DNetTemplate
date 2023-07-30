using Urho3DNet;

namespace $ext_safeprojectname$
{
    public class DoorTrigger : TriggerAnimator
    {
        public string InventoryKey { get; set; }

        public DoorTrigger(Context context): base(context)
        {
        }

        public override bool Filter(Node node)
        {
            if (!string.IsNullOrEmpty(InventoryKey))
            {
                var player = node.GetComponent<Player>();
                if (player != null)
                {
                    return player.HasInInventory(InventoryKey);
                }
                return false;
            }

            return true;
        }
    }
}