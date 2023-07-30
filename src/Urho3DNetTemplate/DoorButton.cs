﻿using Urho3DNet;

namespace Urho3DNetTemplate
{
    public class DoorButton : Component
    {
        public Animation OpenAnimation { get; set; }

        public Animation CloseAnimation { get; set; }

        public bool Open { get; set; }

        public DoorButton(Context context): base(context)
        {
        }

        protected override void OnNodeSet(Node previousNode, Node currentNode)
        {
            if (currentNode != null)
            {
                SubscribeToEvent("Use", currentNode, HandleUse);
            }
            else
            {
                UnsubscribeFromEvent("Use");
            }
            base.OnNodeSet(previousNode, currentNode);
        }

        private void HandleUse(VariantMap obj)
        {
            var controller = Node.Parent.GetComponent<AnimationController>(true);
            if (controller.NumAnimations > 0)
                return;

            var animationParameters = new AnimationParameters(Open ? CloseAnimation : OpenAnimation)
            {
                RemoveOnCompletion = true
            };

            if (animationParameters.Animation == null)
                return;

            Open = !Open;
            controller.PlayNewExclusive(animationParameters, 0.0f);
        }
    }
}