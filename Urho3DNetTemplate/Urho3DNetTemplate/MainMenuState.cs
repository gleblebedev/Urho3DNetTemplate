using Urho3DNet;

namespace $ext_safeprojectname$
{
    public class MainMenuState : MenuStateBase
    {


        public MainMenuState(UrhoApplication app) : base(app, "UI/MainMenu.rml")
        {
        }

        public override void OnDataModelInitialized(MenuComponent menuComponent)
        {
            menuComponent.BindDataModelProperty("is_game_played", _ => _.Set(Application?.IsGameRunning == true), _ => { });
            menuComponent.BindDataModelProperty("game_title", _ => _.Set("$ext_safeprojectname$"), _ => { });
            menuComponent.BindDataModelEvent("Continue", OnContinue);
            menuComponent.BindDataModelEvent("NewGame", OnNewGame);
            menuComponent.BindDataModelEvent("Settings", OnSettings);
            menuComponent.BindDataModelEvent("Exit", OnExit);
        }

        public void OnNewGame(VariantList variantList)
        {
            Application.ToNewGame();
        }

        public void OnSettings(VariantList variantList)
        {
            Application.ToSettings();
        }

        public void OnExit(VariantList variantList)
        {
            Application.Quit();
        }

        public void OnContinue(VariantList variantList)
        {
            Application.ContinueGame();
        }
    }
}