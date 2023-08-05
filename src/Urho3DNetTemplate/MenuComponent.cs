using System;
using Urho3DNet;

namespace Urho3DNetTemplate
{
    public class MenuComponent : RmlUIComponent
    {
        public MenuComponent(Context context):base(context)
        {
        }

        public MenuStateBase State { get; set; }

        private bool _bloom = true;
        public void UpdateProperties()
        {
            DirtyAllVariables();
        }

        protected override void OnDataModelInitialized()
        {
            State.OnDataModelInitialized(this);
            base.OnDataModelInitialized();
        }
    }
}