using System.Collections.Generic;
using Engine.Input.EventInput;
using Engine.Input.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RainbowMadness.Data;
using Game = Microsoft.Xna.Framework.Game;

namespace RainbowMadness
{
    public static class ScreenManager
    {
        public static List<IScreen> Screens = new List<IScreen>();
        public static SpriteFont Font;
        public static OptimizedKeyboardManager Input = new OptimizedKeyboardManager();
        public static float VerticalSpace;
        public static Vector2 Dimensions;
        public static GraphicsDevice Device;
        public static ContentManager Content;
        public static Game Game;
        public static GameSettings Settings = new GameSettings();
        private static readonly Color BackgroundColor = Color.Black;

        public static void OpenScreen(IScreen screen)
        {
            Screens.Add(screen);
        }

        public static void CloseScreen(IScreen screen)
        {
            if (!Screens.Contains(screen)) return;
            Screens.Remove(screen);
            if (Screens.Count <= 0) Game.Exit();
        }

        public static void Draw(SpriteBatch batch)
        {
            Device.Clear(BackgroundColor);
            if (Screens.Count == 0) return;

            // Draw all screens top-down until we hit the first non-popup screen
            var index = Screens.Count - 1;
            while (Screens[index].IsPopup && index > 0)
                index--;

            batch.Begin();
            for (; index < Screens.Count; index++)
                Screens[index].Draw(batch);
            batch.End();
        }

        public static void Update(float dt)
        {
            Input.Update();
            if (Screens.Count == 0) return;
            Screens[Screens.Count - 1].Update(dt);
        }

        public static void Initialize(Game game, GameWindow window, GraphicsDevice device, ContentManager content)
        {
            Game = game;
            Device = device;
            Content = content;
            Font = Content.Load<SpriteFont>("font");
            Dimensions = new Vector2(Device.Viewport.Width, Device.Viewport.Height);
            KeyboardDispatcher.Initialize(window);
            VerticalSpace = Font.MeasureString("I").Y;
        }
    }
}