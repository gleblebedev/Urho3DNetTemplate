using Urho3DNet;

namespace Urho3DNetTemplate
{
    public class MainMenuComponent : RmlUIComponent
    {
        public MainMenuComponent(Context context) : base(context)
        {
        }

        public UrhoApplication Application { get; set; }

        protected override void OnDataModelInitialized()
        {
            BindDataModelProperty("is_game_played", _ => _.Set(Application?.IsGameRunning == true), _ => { });
            BindDataModelProperty("game_title", _ => _.Set("Awesome game"), _ => { });
            base.OnDataModelInitialized();
        }

        public void UpdateProperties()
        {
            DirtyAllVariables();
        }
    }
}