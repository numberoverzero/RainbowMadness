using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Input.Managers.SinglePlayer;
using Engine.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RainbowMadness
{
    public class ConfirmDialog : IScreen
    {
        private static readonly List<string> MenuDirections = new List<string>
                                                                  {"menu_up", "menu_down", "menu_left", "menu_right"};

        private static readonly Texture2D FadeBackground = ColorTextureGenerator.Create(ScreenManager.Device,
                                                                                        new Color(0,0,0, 150));

        private static readonly Texture2D PopupBackground = ColorTextureGenerator.Create(ScreenManager.Device,
                                                                                         new Color(0,0,0, 255));

        private readonly Rectangle _backgroundRect;
        private readonly TextBox _cancelBox;
        private readonly bool _cancelOnClose;
        private readonly TextBox _confirmBox;
        private readonly TextBox _msgBox;
        private bool _confirmSelected;
        private Rectangle _popupBackgroundRect;

        public ConfirmDialog(bool cancelOnClose = true)
        {
            _confirmBox = new TextBox() { GradientBackground = false };
            _cancelBox = new TextBox() { GradientBackground = false };
            _msgBox = new TextBox() { GradientBackground = false };
            _cancelOnClose = cancelOnClose;
            _backgroundRect = new Rectangle(0, 0, (int) ScreenManager.Dimensions.X, (int) ScreenManager.Dimensions.Y);
            RecalculateBoxes();
            ConfirmSelected = false;
        }

        public Action OnCancel { get; set; }
        public Action OnConfirm { get; set; }

        public bool ConfirmSelected
        {
            get { return _confirmSelected; }
            set
            {
                _confirmSelected = value;
                _confirmBox.Highlighted = _confirmSelected;
                _cancelBox.Highlighted = !_confirmSelected;
            }
        }

        public string Confirm
        {
            get { return _confirmBox.Text; }
            set
            {
                _confirmBox.Text = value;
                RecalculateBoxes();
            }
        }

        public string Cancel
        {
            get { return _cancelBox.Text; }
            set
            {
                _cancelBox.Text = value;
                RecalculateBoxes();
            }
        }

        public string Message
        {
            get { return _msgBox.Text; }
            set
            {
                _msgBox.Text = value;
                RecalculateBoxes();
            }
        }

        #region IScreen Members

        public bool IsPopup
        {
            get { return true; }
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(FadeBackground, _backgroundRect, Color.White);
            batch.Draw(PopupBackground, _popupBackgroundRect, Color.White);
            _msgBox.Draw(batch);
            _confirmBox.Draw(batch);
            _cancelBox.Draw(batch);
        }

        public void Update(float dt)
        {
            var input = ScreenManager.Input;

            if (MenuDirections.Any(input.IsPressed))
            {
                ConfirmSelected = !ConfirmSelected;
            }

            var close = false;
            if (input.IsPressed("menu_back"))
            {
                if (_cancelOnClose && OnCancel != null)
                    OnCancel();
                close = true;
            }
            if (input.IsPressed("menu_select"))
            {
                var action = _confirmSelected ? OnConfirm : OnCancel;
                if (action != null) action();
                close = true;
            }
            if (close) ScreenManager.CloseScreen(this);
        }

        #endregion

        private void RecalculateBoxes()
        {
            var screenCenter = ScreenManager.Dimensions/2;
            var msgSize = _msgBox.Dimensions;
            var confirmSize = _confirmBox.Dimensions;
            var cancelSize = _cancelBox.Dimensions;

            _msgBox.X = (int) (screenCenter.X - msgSize.X/2);
            _msgBox.Y = (int) (screenCenter.Y - msgSize.Y*1.15f);

            var optionsWidth = 1.15f*(confirmSize.X + cancelSize.X);
            var optionsRight = screenCenter.X + optionsWidth/2;

            _confirmBox.X = (int) (screenCenter.X - optionsWidth/2);
            _confirmBox.Y = (int) (screenCenter.Y + confirmSize.Y*0.15f);

            _cancelBox.X = (int) (optionsRight - cancelSize.X);
            _cancelBox.Y = (int) (screenCenter.Y + cancelSize.Y*0.15f);

            var popupBackgroundWidth = Math.Max(msgSize.X*1.15f, optionsWidth);
            var popupBackgroundHeight = 1.15f*(msgSize.Y + Math.Max(confirmSize.Y, cancelSize.Y));
            var popupBackgroundX = screenCenter.X - popupBackgroundWidth/2;
            var popupBackgroundY = screenCenter.Y - popupBackgroundHeight/2;
            _popupBackgroundRect = new Rectangle((int) popupBackgroundX, (int) popupBackgroundY,
                                                (int) popupBackgroundWidth, (int) popupBackgroundHeight);
        }
    }
}