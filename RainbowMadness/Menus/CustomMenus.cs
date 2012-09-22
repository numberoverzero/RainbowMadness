using System;
using Engine.Input.Managers.SinglePlayer;
using Engine.Utility;
using Microsoft.Xna.Framework.Graphics;

namespace RainbowMadness
{
    public class JoinScreen : IScreen
    {
        private readonly InputTextBox _server;
        private readonly InputTextBox _username;

        public JoinScreen()
        {
            var x = (int) (0.1f*ScreenManager.Dimensions.X);
            var y = (int) (0.1f*ScreenManager.Dimensions.Y);
            var width = (int) (0.75f*ScreenManager.Dimensions.X);
            _server = new InputTextBox
                          {
                              Label = "Server  ",
                              Delimeter = " ",
                              Highlighted = true,
                              GradientBackground = true,
                              X = x,
                              Y = y,
                              MinimumWidth = width
                          };
            _username = new InputTextBox
                            {
                                Label = "Username",
                                Delimeter = " ",
                                Highlighted = false,
                                GradientBackground = true,
                                X = x,
                                Y = (int) (y + _server.Height*1.15f),
                                MinimumWidth = width
                            };
            _server.Input = ScreenManager.Settings.JoinServer;
            _username.Input = ScreenManager.Settings.Username;
        }

        #region IScreen Members

        public bool IsPopup
        {
            get { return false; }
        }

        public void Draw(SpriteBatch batch)
        {
            _server.Draw(batch);
            _username.Draw(batch);
        }

        public void Update(float dt)
        {
            _server.Update(dt);
            _username.Update(dt);
            var input = ScreenManager.Input;
            if (input.IsPressed("menu_up") || input.IsPressed("menu_down"))
            {
                _server.Highlighted = !_server.Highlighted;
                _username.Highlighted = !_username.Highlighted;
            }
            if (input.IsPressed("menu_select"))
                OnSelect();
            if (input.IsPressed("menu_back"))
                OnClose();
        }

        #endregion

        protected void OnSelect(string server, string password)
        {
            Console.WriteLine("Server: {0}, Password: {1}".format(_server.Text, _username.Text));
        }

        private void OnClose()
        {
            ScreenManager.CloseScreen(this);
            ScreenManager.Settings.JoinServer = _server.Input;
            ScreenManager.Settings.Username = _username.Input;
        }

        private void OnSelect()
        {
        }
    }
}