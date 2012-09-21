﻿using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Input.EventInput;
using Engine.Input.Managers;
using Engine.Input.Managers.SinglePlayer;
using Engine.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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

        private static readonly Color BackgroundColor = Color.Black;

        public static void OpenScreen(IScreen screen)
        {
            Screens.Add(screen);
        }

        public static void CloseScreen(IScreen screen)
        {
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

    public interface IScreen
    {
        bool IsPopup { get; }
        void Draw(SpriteBatch batch);
        void Update(float dt);
    }

    public abstract class MenuScreen<TTextBox> : IScreen where TTextBox: TextBox, new()
    {
        protected readonly List<string> Options;
        protected List<TTextBox> OptionBoxes;
        private int _selectedIndex;

        protected MenuScreen()
        {
            Options = new List<string>();
            OptionBoxes = new List<TTextBox>();
        }

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                if (Options.Count == 0) return;
                if (_selectedIndex < OptionBoxes.Count) OptionBoxes[_selectedIndex].Highlighted = false;
                _selectedIndex = value.Mod(Options.Count);
                OptionBoxes[_selectedIndex].Highlighted = true;
            }
        }

        #region IScreen Members

        public void Draw(SpriteBatch batch)
        {
            if (OptionBoxes == null) return;
            foreach (var optionBox in OptionBoxes)
                optionBox.Draw(batch);
        }

        public void Update(float dt)
        {
            foreach (var optionBox in OptionBoxes) optionBox.Update(dt);
            if (ScreenManager.Input.IsPressed("menu_up"))
                SelectedIndex--;
            if (ScreenManager.Input.IsPressed("menu_down"))
                SelectedIndex++;
            if (ScreenManager.Input.IsPressed("menu_select"))
                OnSelect(SelectedIndex);
            if (ScreenManager.Input.IsPressed("menu_toggle"))
                OnToggle(SelectedIndex);
            if (ScreenManager.Input.IsPressed("menu_back"))
                ScreenManager.CloseScreen(this);
        }

        public bool IsPopup
        {
            get { return false; }
        }

        #endregion

        public virtual void AddOption(string option)
        {
            Options.Add(option);
            RecalculateBoxes();
        }

        public virtual void RemoveOption(string option)
        {
            var index = Options.IndexOf(option);
            Options.Remove(option);
            OptionBoxes.RemoveAt(index);
            RecalculateBoxes();
        }

        protected virtual void RecalculateBoxes()
        {
            var oldSelectedIndex = _selectedIndex;
            OptionBoxes = new List<TTextBox>();
            var width = (int) (0.75f*ScreenManager.Dimensions.X);
            var x = (int) (0.1f*ScreenManager.Dimensions.X);
            var y0 = (0.1f*ScreenManager.Dimensions.Y);
            var y = y0;
            foreach (var option in Options)
            {
                var optionBox = new TTextBox {X = x, Y = (int) y, Width = width, Text = option};
                y += optionBox.Height*1.15f;
                OptionBoxes.Add(optionBox);
            }
            if (OptionBoxes.Count == 0) return;
            SelectedIndex = oldSelectedIndex > OptionBoxes.Count ? OptionBoxes.Count : oldSelectedIndex;
        }

        protected abstract void OnSelect(int index);
        protected abstract void OnToggle(int index);
    }

    public abstract class OptionMenuScreen : MenuScreen<OptionTextBox>
    {
        private List<string> Descriptions;
        private List<List<string>> OptionLists;
 
        public void AddOption(string option, params string[] options)
        {
            Descriptions.Add(option);
            OptionLists.Add(options.ToList());
            base.AddOption(option);
            
        }

        public OptionMenuScreen()
        {
            Descriptions = new List<string>();
            OptionLists = new List<List<string>>();
        }

        protected override void OnToggle(int index)
        {
            var optionTextBox = OptionBoxes[index] as OptionTextBox;
            if (optionTextBox != null) optionTextBox.OptionIndex++;
        }

        protected override void RecalculateBoxes()
        {
            base.RecalculateBoxes();
            for(int i=0;i<Options.Count;i++)
            {
                var box = OptionBoxes[i];
                box.Description = Descriptions[i];
                box.Options = OptionLists[i];
            }
        }
    }

    public class MainScreen : MenuScreen<TextBox>
    {
        public MainScreen()
        {
            AddOption("Host Game");
            AddOption("Join Game");
        }

        protected override void OnSelect(int index)
        {
            switch (index)
            {
                case 0:
                    ScreenManager.OpenScreen(new HostScreen());
                    break;
                case 1:
                    ScreenManager.OpenScreen(new JoinScreen());
                    break;
            }
        }

        protected override void OnToggle(int index)
        {
        }
    }

    public class HostScreen : OptionMenuScreen
    {
        public HostScreen() : base()
        {
            AddOption("[{0}] players", "2", "3", "4", "5", "6", "7");
            AddOption("When I can't play a card, I must [{0}]", "draw one", "draw until I get a playable card");
            AddOption("When I finish drawing I [{0}] play a card", "may", "may not");
            AddOption("Password is [{0}]", "jellybean", "bob", "cucumber1743", "9rQhEfD3k$#i");
        }

        protected override void OnSelect(int index)
        {
            // Start hosting the game
            throw new NotImplementedException();
        }
    }

    public class JoinScreen : IScreen
    {
        private readonly InputTextBox _password;
        private readonly InputTextBox _server;

        public JoinScreen()
        {
            var x = (int) (0.1f*ScreenManager.Dimensions.X);
            var y = (int) (0.1f*ScreenManager.Dimensions.Y);
            var width = (int) (0.75f*ScreenManager.Dimensions.X);
            _server = new InputTextBox
                          {Label = "Server  ", Delimeter = " ", Highlighted = true, X = x, Y = y, Width = width};
            _password = new InputTextBox
                            {
                                Label = "Password",
                                Delimeter = " ",
                                PasswordBox = true,
                                Highlighted = false,
                                X = x,
                                Y = (int) (y + _server.Height*1.15f),
                                Width = width
                            };
        }

        #region IScreen Members

        public bool IsPopup
        {
            get { return false; }
        }

        public void Draw(SpriteBatch batch)
        {
            _server.Draw(batch);
            _password.Draw(batch);
        }

        public void Update(float dt)
        {
            _server.Update(dt);
            _password.Update(dt);
            if (ScreenManager.Input.IsPressed("menu_up") || ScreenManager.Input.IsPressed("menu_down"))
            {
                _server.Highlighted = !_server.Highlighted;
                _password.Highlighted = !_password.Highlighted;
            }
            if (ScreenManager.Input.IsPressed("menu_select"))
                OnSelect();
            if (ScreenManager.Input.IsPressed("menu_back"))
                ScreenManager.CloseScreen(this);
        }

        #endregion

        protected void OnSelect(string server, string password)
        {
            Console.WriteLine("Server: {0}, Password: {1}".format(_server.Text, _password.Text));
        }

        private void OnSelect()
        {
        }
    }

    public class OptionTextBox : TextBox
    {
        private int _optionIndex;
        public int OptionIndex
        {
            get { return _optionIndex; }
            set
            {
                if (Options.Count == 0) return;
                _optionIndex = value.Mod(Options.Count);
                
            }
        }
        public List<string> Options { get; set; }
        public string Description { get; set; }
        public override string Text { get { return Options == null || Options.Count == 0 ?  "" :  Description.format(Options[OptionIndex]); } }

    }

    public class InputTextBox : TextBox, IKeyboardSubscriber
    {
        private float _elapsedTime;

        public InputTextBox()
        {
            Input = "";
        }

        public string Label { get; set; }
        public string Delimeter { get; set; }
        public string Input { get; private set; }
        public bool PasswordBox { get; set; }

        public new bool Highlighted
        {
            get { return base.Highlighted; }
            set
            {
                base.Highlighted = value;
                if (value)
                    KeyboardDispatcher.RegisterListener(this);
                else
                    KeyboardDispatcher.UnregisterListener(this);
            }
        }

        public override string Text
        {
            get
            {
                var input = PasswordBox ? new string('*', Input.Length) : Input;
                var caret = Highlighted && !((_elapsedTime%500) < 250) ? "|" : "";
                return "{0}{1}{2}{3}".format(Label, Delimeter, input, caret);
            }
        }

        #region IKeyboardSubscriber Members

        public void ReceiveTextInput(char inputChar)
        {
            Input = Input + inputChar;
        }

        public void ReceiveTextInput(string text)
        {
            Input = Input + text;
        }

        public void ReceiveCommandInput(char command)
        {
            switch (command)
            {
                case '\b': //backspace
                    if (Input.Length > 0)
                        Input = Input.Substring(0, Input.Length - 1);
                    break;
            }
        }

        public void ReceiveSpecialInput(Keys key)
        {
        }

        public bool Selected { get; set; }

        #endregion

        public override void Update(float dt)
        {
            _elapsedTime += 1000*dt;
        }
    }

    public class TextBox
    {
        private readonly SpriteFont _font;
        private readonly Texture2D _textBoxTexture;

        private string _text = "";

        public TextBox()
        {
            _textBoxTexture = ScreenManager.Content.Load<Texture2D>("textBoxTexture");
            _font = ScreenManager.Font;
            Height = (int) _font.MeasureString("I").Y + 2;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; private set; }
        public bool Highlighted { get; set; }

        public virtual String Text
        {
            get { return _text; }
            set
            {
                _text = String.IsNullOrEmpty(value) ? "" : value;

                //if you attempt to display a character that is not in your font
                //you will get an exception, so we filter the characters
                _text = _text.Where(c => _font.Characters.Contains(c)).Aggregate("", (current, c) => current + c);

                if (_font.MeasureString(_text).X > Width)
                {
                    //recursion to ensure that text cannot be larger than the box
                    Text = _text.Substring(0, _text.Length - 1);
                }
            }
        }

        public void Draw(SpriteBatch batch)
        {
            var toDraw = Text;
            batch.Draw(_textBoxTexture, new Rectangle(X, Y, Width, Height),
                       new Rectangle(0, Highlighted ? (_textBoxTexture.Height/2) : 0,
                                     _textBoxTexture.Width, _textBoxTexture.Height/2), Color.White);

            batch.DrawString(_font, toDraw, new Vector2(X, Y) + Vector2.One, Color.Black);
            var color = Highlighted ? Color.Yellow : Color.DeepSkyBlue;
            batch.DrawString(_font, toDraw, new Vector2(X, Y), color);
        }

        public virtual void Update(float dt)
        {
        }
    }
}