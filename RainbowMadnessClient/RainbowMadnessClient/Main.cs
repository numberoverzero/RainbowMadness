using System;
using Engine.Input;
using Engine.Input.Managers.AddBindings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RainbowMadnessShared;
using Game = Microsoft.Xna.Framework.Game;

namespace RainbowMadnessClient
{
#if WINDOWS || XBOX
    internal static class Program
    {
        private static void Main(string[] args)
        {
            using (var game = new RainbowMadnessGame())
            {
                game.Run();
            }
        }
    }
#endif

    public class RainbowMadnessGame : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public RainbowMadnessGame()
        {
            _graphics = new GraphicsDeviceManager(this);
        }

        protected void InitializeInput()
        {
            Action<string, Keys> addBinding = (s, k) => ScreenManager.Input.AddBinding(s, PlayerIndex.One, k);
            
            addBinding("exit", Keys.Escape);
            addBinding("menu_up", Keys.Up);
            addBinding("menu_down", Keys.Down);
            addBinding("menu_down", Keys.Down);
            addBinding("menu_left", Keys.Left);
            addBinding("menu_right", Keys.Right);
            addBinding("menu_select", Keys.Enter);
            addBinding("menu_toggle", Keys.Space);
            addBinding("menu_tab", Keys.Tab);
            addBinding("menu_back", Keys.Escape);

            addBinding("card_left", Keys.Left);
            addBinding("card_right", Keys.Right);
            addBinding("play_card", Keys.Space);
            addBinding("play_card2", Keys.Enter);

            ScreenManager.Input.AddBinding("start_game", PlayerIndex.One, Keys.S, ModifierKey.Ctrl, ModifierKey.Shift);
        }

        protected override void LoadContent()
        {
            Content.RootDirectory = "Content";
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            PacketGlobals.Initialize();
            ScreenManager.Initialize(this, Window, _graphics, Content);
            InitializeInput();
            IsMouseVisible = true;
            _graphics.PreferredBackBufferHeight = 768;
            _graphics.PreferredBackBufferWidth =1024;
            _graphics.ApplyChanges();
            Card.LoadContent(Content);
            ScreenManager.OpenScreen(new MainScreen());
        }

        protected override void Update(GameTime gameTime)
        {
            ScreenManager.Update((float) gameTime.ElapsedGameTime.TotalMilliseconds/1000);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            ScreenManager.Draw(_spriteBatch);
            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            base.OnExiting(sender, args);

            ScreenManager.OnExiting(sender, args);
        }
    }
}