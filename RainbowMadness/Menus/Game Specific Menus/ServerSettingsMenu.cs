using System;
using Engine.Input.Managers.SinglePlayer;
using Engine.Utility;
using Microsoft.Xna.Framework.Graphics;
using RainbowMadness.Data;
using RainbowMadness.Menus;

namespace RainbowMadness
{
    public class ServerSettingsMenu : IScreen
    {
        protected readonly InputTextBox Server;
        protected readonly InputTextBox Username;

        public ServerSettingsMenu()
        {
            var x = (int) (0.1f*ScreenManager.Dimensions.X);
            var y = (int) (0.1f*ScreenManager.Dimensions.Y);
            var width = (int) (0.75f*ScreenManager.Dimensions.X);
            Server = new InputTextBox
                          {
                              Label = "Server  ",
                              Delimeter = " ",
                              Highlighted = true,
                              GradientBackground = true,
                              X = x,
                              Y = y,
                              MinimumWidth = width
                          };
            Username = new InputTextBox
                            {
                                Label = "LocalPlayer",
                                Delimeter = " ",
                                Highlighted = false,
                                GradientBackground = true,
                                X = x,
                                Y = (int) (y + Server.Height*1.15f),
                                MinimumWidth = width
                            };
            Server.Input = ScreenManager.Settings.ServerName;
            Username.Input = ScreenManager.Settings.LocalPlayer;
        }

        #region IScreen Members

        public bool IsPopup
        {
            get { return false; }
        }

        public void Draw(SpriteBatch batch)
        {
            Server.Draw(batch);
            Username.Draw(batch);
        }

        public void Update(float dt)
        {
            Server.Update(dt);
            Username.Update(dt);
            var input = ScreenManager.Input;
            if (input.IsPressed("menu_up") || input.IsPressed("menu_down"))
            {
                Server.Highlighted = !Server.Highlighted;
                Username.Highlighted = !Username.Highlighted;
            }
            if (input.IsPressed("menu_select"))
                OnSelect();
            if (input.IsPressed("menu_back"))
                OnClose();
        }

        #endregion

        protected virtual void OnSelect()
        {
            ScreenManager.Settings.ServerName = Server.Input;
            ScreenManager.Settings.LocalPlayer = Username.Input;
            ScreenManager.CloseScreen(this);

            var game = new Game(@"Content\Decks\cards.txt", ScreenManager.Settings);
            ScreenManager.OpenScreen(new GameScreen(game));
        }

        protected virtual void OnClose()
        {
            ScreenManager.CloseScreen(this);
        }
    }

    public class JoinScreen : ServerSettingsMenu
    {
        protected override void OnSelect()
        {
            ScreenManager.Settings.IsHost = false;
            base.OnSelect();
        }
    }

    public class HostServerScreen : ServerSettingsMenu
    {
        protected override void OnSelect()
        {
            ScreenManager.Settings.IsHost = true;
            base.OnSelect();
        }
    }


}