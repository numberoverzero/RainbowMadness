using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Engine.Input;

namespace RainbowMadness
{

#if WINDOWS || XBOX
    static class Program{
        static void Main(string[] args){
            using (RainbowMadnessGame game = new RainbowMadnessGame()){
                game.Run();
            }
        }
    }
#endif

    public class RainbowMadnessGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        BasicInjectableInputManager inputManager;
        SpriteBatch spriteBatch;

        public RainbowMadnessGame()
        {
            graphics = new GraphicsDeviceManager(this);
            inputManager = new BasicInjectableInputManager();
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            BasicInputManager.Initialize(Window);
            InitializeInput();
            base.Initialize();
        }

        protected void InitializeInput()
        {
            inputManager.AddBinding("exit", PlayerIndex.One, Keys.Escape);
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            inputManager.Update();
            if (inputManager.IsPressed("exit", PlayerIndex.One))
                Exit();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);
        }
    }
}
