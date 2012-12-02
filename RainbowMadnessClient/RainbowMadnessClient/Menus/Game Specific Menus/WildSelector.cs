using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainbowMadnessClient.Menus.Game_Specific_Menus
{
    public class WildSelector : SelectorPopup
    {
        public WildSelector(Action<string> selectorFunc, string onCancelValue = null) : base(selectorFunc, onCancelValue)
        {
            AddOption("Red");
            AddOption("Yellow");
            AddOption("Green");
            AddOption("Blue");
        }

        protected override void OnToggle(int index)
        {
            OnSelect(index);
        }
    }
}
