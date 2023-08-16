using Urho3DNet;

namespace $ext_safeprojectname$
{
    public class SettingsMenuState : MenuStateBase
    {
        private SettingFile _config;

        public SettingsMenuState(UrhoApplication app) : base(app, "UI/Options.rml")
        {
            _config = app.Settings;

        }

        public SettingFile Settings => _config;

        public override void OnDataModelInitialized(MenuComponent menuComponent)
        {
            menuComponent.BindDataModelEvent("Apply", OnApply);
            menuComponent.BindDataModelEvent("Cancel", OnCancel);
            menuComponent.BindDataModelProperty("bloom", val=> val.Set(_config.Bloom), (val)=> _config.Bloom = val.Bool);
            menuComponent.BindDataModelProperty("ssao", val => val.Set(_config.SSAO), (val) => _config.SSAO = val.Bool);
            menuComponent.BindDataModelProperty("master", val => val.Set(_config.MasterVolume), (val) => _config.MasterVolume = val.Float);
            menuComponent.BindDataModelProperty("music", val => val.Set(_config.MusicVolume), (val) => _config.MusicVolume = val.Float);
            menuComponent.BindDataModelProperty("effects", val => val.Set(_config.EffectVolume), (val) => _config.EffectVolume = val.Float);
            //menuComponent.BindDataModelProperty("shadows", val => val.Set(_shadowsQuality), (val) => _shadowsQuality = val.Convert(VariantType.VarInt).Int);
        }

        public override void Activate(StringVariantMap bundle)
        {
            _config = Application.Settings;

            Audio audio = Context.GetSubsystem<Audio>();

            base.Activate(bundle);
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }

        private void OnCancel(VariantList obj)
        {
            Application.HandleBackKey();

            Application.Settings = SettingFile.Load(Context);
        }

        private void OnApply(VariantList obj)
        {
            _config.Apply(Context);
            _config.Save(Context);

            Application.HandleBackKey();
        }
    }
}