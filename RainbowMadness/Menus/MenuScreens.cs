using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Input.Managers.SinglePlayer;
using Engine.Utility;
using Microsoft.Xna.Framework.Graphics;

namespace RainbowMadness.Menus
{
    public abstract class MenuScreen<TTextBox> : IScreen where TTextBox : TextBox, new()
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
            var input = ScreenManager.Input;
            if (input.IsPressed("menu_up"))
                SelectedIndex--;
            if (input.IsPressed("menu_down"))
                SelectedIndex++;
            if (input.IsPressed("menu_select"))
                OnSelect(SelectedIndex);
            if (input.IsPressed("menu_toggle"))
                OnToggle(SelectedIndex);
            if (input.IsPressed("menu_back"))
                OnClose();
        }

        public bool IsPopup
        {
            get { return false; }
        }

        #endregion

        public virtual void AddOption(string option)
        {
            Options.Add(option);
            RecalculateBoxes(Options.Count - 1, Options.Count);
        }

        public virtual void RemoveOption(string option)
        {
            var index = Options.IndexOf(option);
            Options.Remove(option);
            OptionBoxes.RemoveAt(index);
            RecalculateBoxes(Options.Count + 1, Options.Count);
        }

        protected virtual void RecalculateBoxes(int oldCount, int newCount)
        {
            if (oldCount >= newCount) return;
            //newCount > oldCount

            var oldSelectedIndex = _selectedIndex;
            //OptionBoxes = new List<TTextBox>();
            var width = (int) (0.75f*ScreenManager.Dimensions.X);
            var x = (int) (0.1f*ScreenManager.Dimensions.X);

            var y0 = (0.1f*ScreenManager.Dimensions.Y);
            if (oldCount > 0)
            {
                var last_option_box = OptionBoxes[OptionBoxes.Count - 1];
                y0 = last_option_box.Y + last_option_box.Height*1.15f;
            }
            var y = y0;
            for (int i = oldCount; i < newCount; i++)
            {
                var option = Options[i];
                var optionBox = new TTextBox { X = x, Y = (int)y, MinimumWidth = width, Text = option, GradientBackground = true};
                y += optionBox.Height*1.15f;
                OptionBoxes.Add(optionBox);
            }
            if (OptionBoxes.Count == 0) return;
            SelectedIndex = oldSelectedIndex > OptionBoxes.Count ? OptionBoxes.Count : oldSelectedIndex;
        }

        protected abstract void OnSelect(int index);
        protected abstract void OnToggle(int index);

        protected virtual void OnClose()
        {
            ScreenManager.CloseScreen(this);
        }
    }

    public abstract class OptionMenuScreen : MenuScreen<OptionTextBox>
    {
        private readonly List<string> Descriptions;
        private readonly List<int> OptionIndexes;
        private readonly List<List<string>> OptionLists;

        protected OptionMenuScreen()
        {
            Descriptions = new List<string>();
            OptionLists = new List<List<string>>();
            OptionIndexes = new List<int>();
        }

        public void AddOption(string option, params string[] options)
        {
            Descriptions.Add(option);
            OptionLists.Add(options.ToList());
            OptionIndexes.Add(0);
            base.AddOption(option);
        }

        protected override void OnToggle(int index)
        {
            var optionTextBox = OptionBoxes[index];
            if (optionTextBox != null) optionTextBox.OptionIndex++;
        }

        protected override void RecalculateBoxes(int oldCount, int newCount)
        {
            base.RecalculateBoxes(oldCount, newCount);
            for (int i = 0; i < Options.Count; i++)
            {
                var box = OptionBoxes[i];
                box.Description = Descriptions[i];
                box.Options = OptionLists[i];
            }
        }
    }
}