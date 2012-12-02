using System;
using System.Collections.Generic;
using Engine.Input.EventInput;
using Engine.Input.Managers;
using Engine.Rendering;
using Engine.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RainbowMadnessShared;
using Game = Microsoft.Xna.Framework.Game;

namespace RainbowMadnessClient
{
    public static class ScreenManager
    {
        public static List<IScreen> Screens = new List<IScreen>();
        public static SpriteFont Font;
        public static SpriteFont BigFont;
        public static OptimizedKeyboardManager Input = new OptimizedKeyboardManager();
        public static float VerticalSpace;
        
        public static GraphicsDeviceManager DeviceManager;
        public static GraphicsDevice Device;
        public static ContentManager Content;
        public static Game Game;
        public static GameSettings Settings = new GameSettings();
        private static readonly Color BackgroundColor = Color.LightSlateGray;

        public static Vector2 Dimensions
        {
            get
            {
                return new Vector2(1024, 768);
            }
        }
        public static void OpenScreen(IScreen screen)
        {
            Screens.Add(screen);
        }

        public static void CloseScreen(IScreen screen, bool asCleanup = false)
        {
            if (!Screens.Contains(screen)) return;
            Screens.Remove(screen);
            screen.OnClose(asCleanup);
            if(!asCleanup && Screens.Count==0) Game.Exit();
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
            Screens.Last().Update(dt);
        }

        public static void Initialize(Game game, GameWindow window, GraphicsDeviceManager deviceManager, ContentManager content)
        {
            Game = game;
            DeviceManager = deviceManager;
            Device = deviceManager.GraphicsDevice;
            BasicShapeRenderer.Initialize(Device);
            Content = content;
            Font = Content.Load<SpriteFont>("font");
            BigFont = Content.Load<SpriteFont>("bigfont");
            KeyboardDispatcher.Initialize(window);
            VerticalSpace = Font.MeasureString("I").Y;
        }

        public static void OnExiting(object sender, EventArgs args)
        {
            // Close all screens down
            while(Screens.Count > 0)
                CloseScreen(Screens.Last(), true);
        }
    }
}