using Urho3DNet;

namespace $safeprojectname$
{
    public class MainMenuComponent : RmlUIComponent
    {
        public MainMenuComponent(Context context) : base(context)
        {
        }

        public UrhoApplication Application { get; set; }

        public void UpdateProperties()
        {
            DirtyAllVariables();
        }

        protected override void OnDataModelInitialized()
        {
            BindDataModelProperty("is_game_played", _ => _.Set(Application?.IsGameRunning == true), _ => { });
            BindDataModelProperty("game_title", _ => _.Set("Awesome game"), _ => { });
            base.OnDataModelInitialized();
        }
    }
}