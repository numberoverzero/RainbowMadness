using Engine.Input.Managers.SinglePlayer;
using Microsoft.Xna.Framework.Graphics;
using RainbowMadness.Data;

namespace RainbowMadness.Menus
{
    public class GameScreen : IScreen
    {
        private readonly Game _game;

        public bool IsPopup { get { return false; }}

        public GameScreen(Game game)
        {
            _game = game;
        }

        public void Draw(SpriteBatch batch)
        {
            _game.DrawGame(batch);
        }

        public void Update(float dt)
        {
            var input = ScreenManager.Input;
            if (input.IsPressed("menu_back"))
                OnClose();
            _game.Update(dt);
        }

        protected void OnClose()
        {
            ScreenManager.OpenScreen(new ConfirmDialog(false)
            {
                Message = "Are you sure you want to leave this game?",
                Confirm = "Yes",
                Cancel = "No",
                OnConfirm = () => {
                                    _game.RemovePlayer(_game.Settings.LocalPlayer); 
                                    ScreenManager.CloseScreen(this); 
                                  }
            });
        }
    }
}
