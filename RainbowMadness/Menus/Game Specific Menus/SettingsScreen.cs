using System;
using Engine.Utility;
using Microsoft.Xna.Framework;

namespace RainbowMadness.Menus
{
    public class SettingsScreen : OptionMenuScreen
    {
        public SettingsScreen()
        {
            AddOption("Colorblind Mode [{0}]", "on", "off");
            AddOption("Resolution [{0}]", "1024x768");
            OptionBoxes[0].OptionIndex = ScreenManager.Settings.ColorblindMode ? 0 : 1;
            setResolution();
            
        }

        protected override void OnSelect(int index)
        {
            OnClose();
        }

        protected override void OnClose()
        {
            base.OnClose();
            ScreenManager.Settings.ColorblindMode = OptionBoxes[0].OptionIndex == 0;
            ScreenManager.Settings.Resolution = getResolution();
            ScreenManager.RefreshSettings();
        }

        private Point getResolution()
        {
            var resolutionText = OptionBoxes[1].CurrentOption;
            var xY = resolutionText.Split('x');
            return new Point(Int32.Parse(xY[0]), Int32.Parse(xY[1]));
        }

        private void setResolution()
        {
            var res = ScreenManager.Settings.Resolution;
            var asString = "{0}x{1}".format(res.X, res.Y);
            var index = OptionBoxes[1].Options.IndexOf(asString);
            OptionBoxes[1].OptionIndex = index;
        }
    }
}