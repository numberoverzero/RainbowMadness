using System;
using Engine.Input.Managers.SinglePlayer;
using Engine.Utility;
using Microsoft.Xna.Framework.Graphics;
using RainbowMadnessShared;

namespace RainbowMadnessClient
{
    public class JoinScreen : IScreen
    {
        protected readonly InputTextBox ServerTextBox;
        protected readonly InputTextBox UsernameTextBox;

        private string _server;
        private string _username;

        public JoinScreen()
        {
            var x = (int) (0.1f*ScreenManager.Dimensions.X);
            var y = (int) (0.1f*ScreenManager.Dimensions.Y);
            var width = (int) (0.75f*ScreenManager.Dimensions.X);
            ServerTextBox = new InputTextBox
                          {
                              Label = "Server  ",
                              Delimeter = " ",
                              Highlighted = true,
                              GradientBackground = true,
                              X = x,
                              Y = y,
                              MinimumWidth = width
                          };
            UsernameTextBox = new InputTextBox
                            {
                                Label = "Username",
                                Delimeter = " ",
                                Highlighted = false,
                                GradientBackground = true,
                                X = x,
                                Y = (int) (y + ServerTextBox.Height*1.15f),
                                MinimumWidth = width
                            };
        }

        #region IScreen Members

        public bool IsPopup
        {
            get { return false; }
        }

        public void Draw(SpriteBatch batch)
        {
            ServerTextBox.Draw(batch);
            UsernameTextBox.Draw(batch);
        }

        public void Update(float dt)
        {
            ServerTextBox.Update(dt);
            UsernameTextBox.Update(dt);
            var input = ScreenManager.Input;
            if (input.IsPressed("menu_up") || input.IsPressed("menu_down") || input.IsPressed("menu_tab"))
                OnToggle();
            if (input.IsPressed("menu_select"))
                OnSelect();
            if (input.IsPressed("menu_back"))
                ScreenManager.CloseScreen(this);
        }

        #endregion

        protected virtual void OnSelect()
        {
            _server = ServerTextBox.Input;
            _username = UsernameTextBox.Input;
            if(String.IsNullOrEmpty(_server) || String.IsNullOrEmpty(_username))
            {
                OnToggle();
                return;
            }
            ScreenManager.CloseScreen(this);
            ScreenManager.OpenScreen(new GameClient(_server, _username));
        }

        protected virtual void OnToggle()
        {
            ServerTextBox.Highlighted = !ServerTextBox.Highlighted;
            UsernameTextBox.Highlighted = !UsernameTextBox.Highlighted;
        }

        public virtual void OnClose(bool asCleanup) { }

    }
}