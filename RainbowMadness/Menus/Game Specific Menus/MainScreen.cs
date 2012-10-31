namespace RainbowMadness.Menus
{
    public class MainScreen : MenuScreen<TextBox>
    {
        public MainScreen()
        {
            AddOption("Host Game");
            AddOption("Join Game");
            AddOption("Settings");
        }

        protected override void OnSelect(int index)
        {
            switch (index)
            {
                case 0:
                    ScreenManager.OpenScreen(new HostScreen());
                    break;
                case 1:
                    ScreenManager.OpenScreen(new JoinScreen());
                    break;
                case 2:
                    ScreenManager.OpenScreen(new SettingsScreen());
                    break;
            }
        }

        protected override void OnToggle(int index)
        {
        }

        protected override void OnClose()
        {
            ScreenManager.OpenScreen(new ConfirmDialog(false)
                                         {
                                             Message = "Really quit?",
                                             Confirm = "Yes",
                                             Cancel = "No",
                                             OnConfirm = () => ScreenManager.CloseScreen(this)
                                         });
        }
    }
}