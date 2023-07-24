﻿using Urho3DNet;

namespace $safeprojectname$
{
    public class InteractableBox : Component
    {
        private ComponentList _drawables;

        public InteractableBox(Context context) : base(context)
        {
        }

        protected override void OnNodeSet(Node previousNode, Node currentNode)
        {
            base.OnNodeSet(previousNode, currentNode);
            if (currentNode != null)
            {
                SubscribeToEvent("Selected", currentNode, Select);
                SubscribeToEvent("Unselected", currentNode, Unselect);
            }
            else
            {
                UnsubscribeFromAllEvents();
            }
        }

        private void Unselect(VariantMap obj)
        {
            var outline = Scene.GetComponent<OutlineGroup>();
            if (outline != null && _drawables != null)
                foreach (var component in _drawables)
                {
                    var drawable = component as Drawable;
                    outline.RemoveDrawable(drawable);
                }
        }

        private void Select(VariantMap obj)
        {
            var outline = Scene.GetComponent<OutlineGroup>();
            if (outline != null)
            {
                _drawables = Node.GetComponents<StaticModel>();
                foreach (var component in _drawables)
                {
                    var drawable = component as Drawable;
                    outline.AddDrawable(drawable);
                }
            }
        }
    }
}