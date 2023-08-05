using Urho3DNet;

namespace Urho3DNetTemplate
{
    public class SettingsMenuState : MenuStateBase
    {
        private bool _bloom = true;
        private bool _ssao;
        private float _music = 0.75f;
        private string _shadowsQuality = "High";

        public SettingsMenuState(UrhoApplication app) : base(app, "UI/Options.rml")
        {
        }

        public override void OnDataModelInitialized(MenuComponent menuComponent)
        {
            menuComponent.BindDataModelEvent("Apply", OnApply);
            menuComponent.BindDataModelEvent("Cancel", OnCancel);
            menuComponent.BindDataModelProperty("bloom", val=> val.Set(_bloom), (val)=>_bloom = val.Bool);
            menuComponent.BindDataModelProperty("ssao", val => val.Set(_ssao), (val) => _ssao = val.Bool);
            menuComponent.BindDataModelProperty("music", val => val.Set(_music), (val) => _music = val.Float);
            menuComponent.BindDataModelProperty("shadows", val => val.Set(_shadowsQuality), (val) => _shadowsQuality = val.String);
        }

        private void OnCancel(VariantList obj)
        {
            Application.HandleBackKey();
        }

        private void OnApply(VariantList obj)
        {
            Application.HandleBackKey();
        }
    }
}