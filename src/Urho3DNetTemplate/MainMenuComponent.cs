using Urho3DNet;

namespace Urho3DNetTemplate
{
    public class MainMenuComponent : RmlUIComponent
    {
        public MainMenuComponent(Context context) : base(context)
        {
        }

        public UrhoApplication Application { get; set; }
        public MainMenuState State { get; set; }

        public void UpdateProperties()
        {
            DirtyAllVariables();
        }

        protected override void OnDataModelInitialized()
        {
            BindDataModelProperty("is_game_played", _ => _.Set(Application?.IsGameRunning == true), _ => { });
            BindDataModelProperty("game_title", _ => _.Set("Awesome game"), _ => { });
            BindDataModelEvent("Continue", OnContinue);
            BindDataModelEvent("NewGame", OnNewGame);
            BindDataModelEvent("Exit", OnExit);
            base.OnDataModelInitialized();
        }

        private void OnExit(VariantList obj)
        {
            State.OnExit();
        }

        private void OnNewGame(VariantList obj)
        {
            State.OnNewGame();
        }

        private void OnContinue(VariantList obj)
        {
            State.OnContinue();
        }
    }
}