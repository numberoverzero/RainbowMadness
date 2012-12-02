namespace RainbowMadnessClient
{
    public class MainScreen : MenuScreen<TextBox>
    {
        public MainScreen()
        {
            AddOption("Join Game");
            AddOption("Settings (Not implemented yet)");
        }

        protected override void OnSelect(int index)
        {
            switch (index)
            {
                case 0:
                    ScreenManager.OpenScreen(new JoinScreen());
                    break;
                default:
                    SelectedIndex++;
                    break;
            }
        }

        protected override void OnToggle(int index)
        {
            OnSelect(index);
        }

        protected override void OnTryClose(int selectedIndex)
        {
            ScreenManager.OpenScreen(new ConfirmDialog(false)
                                         {
                                             Message = "Really quit?",
                                             Confirm = "Yes",
                                             Cancel = "No",
                                             OnConfirm = () => base.OnTryClose(selectedIndex)
                                         });
        }
    }
}