using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Input.EventInput;
using Engine.Rendering;
using Engine.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RainbowMadness
{
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

        public string CurrentOption
        {
            get { return Options[_optionIndex]; }
        }

        public string Description { get; set; }

        public override string Text
        {
            get { return Options == null || Options.Count == 0 ? Description : Description.format(Options[OptionIndex]); }
        }
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
        public string Input { get; set; }
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
        private readonly Texture2D _solidBackground;
        private readonly Texture2D _textBoxTexture;

        /// <summary>
        ///   <para> A function that the textbox checks whenever it needs to draw its text </para>
        ///   <para> This function will always have precedence while it is non-null </para>
        /// </summary>
        public Func<string> TextFunc;

        private string _text = "";

        public TextBox()
        {
            _textBoxTexture = ScreenManager.Content.Load<Texture2D>("textBoxTexture");
            _solidBackground = ColorTextureGenerator.Create(ScreenManager.Device);
            _font = ScreenManager.Font;
            Height = (int) _font.MeasureString("I").Y + 4;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int MinimumWidth { get; set; }

        public int ActualWidth
        {
            get { return (int) Math.Max(_font.MeasureString(Text).X, MinimumWidth); }
        }

        public Vector2 Dimensions
        {
            get { return new Vector2(ActualWidth, Height); }
        }

        public int Height { get; private set; }
        public bool Highlighted { get; set; }
        public bool HasBorder { get; set; }

        public virtual String Text
        {
            get
            {
                var text = TextFunc == null ? _text : TextFunc();
                var validChars = FilterValidCharacters(text);
                return validChars;
            }
            set { _text = String.IsNullOrEmpty(value) ? "" : value; }
        }

        public bool GradientBackground { get; set; }

        private string FilterValidCharacters(string text)
        {
            // Trim non-displayable characters
            return text.Where(c => _font.Characters.Contains(c)).Aggregate("", (current, c) => current + c);
        }

        /// <summary>
        ///   Sets the TextFunc to null
        /// </summary>
        public void UseFixedText()
        {
            TextFunc = null;
        }

        public void Draw(SpriteBatch batch)
        {
            var toDraw = Text;

            //Coloring
            if (Highlighted)
                batch.Draw(_textBoxTexture, new Rectangle(X, Y, ActualWidth, Height),
                           new Rectangle(0, _textBoxTexture.Height/2,
                                         _textBoxTexture.Width, _textBoxTexture.Height/2), Color.White);
            else
            {
                if (GradientBackground)
                    batch.Draw(_textBoxTexture, new Rectangle(X, Y, ActualWidth, Height),
                               new Rectangle(0, 0, _textBoxTexture.Width, _textBoxTexture.Height/2), Color.White);
                else
                    batch.Draw(_solidBackground, new Rectangle(X, Y, ActualWidth, Height), Color.White);
            }

            var y = Y + 2;
            batch.DrawString(_font, toDraw, new Vector2(X, y) + Vector2.One, Color.Black);
            var color = Highlighted ? Color.Yellow : Color.DeepSkyBlue;
            batch.DrawString(_font, toDraw, new Vector2(X, y), color);

            if(HasBorder)
            {
                var borderSize = 2;
                var center = new Vector2(X, Y) + Dimensions/2;
                //add padding for vertical
                var dimensions = Dimensions + new Vector2(borderSize*2);
                BasicShapeRenderer.DrawRectangleOutline(batch, center, Color.White, dimensions, 0, borderSize);
            }
        }

        public virtual void Update(float dt)
        {
        }
    }
}