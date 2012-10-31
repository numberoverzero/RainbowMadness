using System;
using RainbowMadness.Data;

namespace RainbowMadness.Menus
{
    public class HostScreen : OptionMenuScreen
    {
        public HostScreen()
        {
            AddOption("[{0}] players", "2", "3", "4", "5", "6", "7");
            var npStr = ScreenManager.Settings.NPlayers.ToString();
            OptionBoxes[0].OptionIndex = OptionBoxes[0].Options.Contains(npStr)
                                             ? OptionBoxes[0].Options.IndexOf(npStr)
                                             : 0;

            AddOption("When I can't play a card, I must [{0}]", "draw one", "draw until I get a playable card");
            OptionBoxes[1].OptionIndex = ScreenManager.Settings.DrawUntilPlayable ? 1 : 0;

            AddOption("When I finish drawing I [{0}] play a card", "may", "may not");
            OptionBoxes[2].OptionIndex = ScreenManager.Settings.CanPlayAfterDraw ? 0 : 1;
            AddOption("Begin!");
        }

        protected override void OnSelect(int index)
        {
            if (index < 3) SelectedIndex++;
            else
            {
                // Get username and port
                ScreenManager.OpenScreen(new HostServerScreen());

                //Close the host screen
                ScreenManager.CloseScreen(this);
            }
        }

        protected override void OnClose()
        {
            ScreenManager.Settings.NPlayers = Int32.Parse(OptionBoxes[0].CurrentOption);
            ScreenManager.Settings.DrawUntilPlayable = OptionBoxes[1].OptionIndex == 1;
            ScreenManager.Settings.CanPlayAfterDraw = OptionBoxes[2].OptionIndex == 0;
            base.OnClose();
        }
    }
}