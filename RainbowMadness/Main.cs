using System;
using Engine.Input.Managers.AddBindings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RainbowMadness
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
        private Data.Game _game;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public RainbowMadnessGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Console.WriteLine();
            _game = new Data.Game(@"Content\Decks\cards.txt");
        }

        protected void InitializeInput()
        {
            ScreenManager.Input.AddBinding("exit", PlayerIndex.One, Keys.Escape);
            
            ScreenManager.Input.AddBinding("menu_up", PlayerIndex.One, Keys.Up);
            ScreenManager.Input.AddBinding("menu_down", PlayerIndex.One, Keys.Down);
            ScreenManager.Input.AddBinding("menu_select", PlayerIndex.One, Keys.Enter);
            ScreenManager.Input.AddBinding("menu_toggle", PlayerIndex.One, Keys.Space);
            ScreenManager.Input.AddBinding("menu_back", PlayerIndex.One, Keys.Escape);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            ScreenManager.Initialize(this, Window, GraphicsDevice, Content);
            InitializeInput();
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
    }
}