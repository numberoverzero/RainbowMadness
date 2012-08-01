using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainbowMadness.Data
{
    public static class Globals{
    public enum CardColor { Blue, Green, Red, Yellow, None};
    public enum CardValue { Zero, One, Two, Three, Four, Five, Six, Seven, Eight, Nine, None };
    public enum SpecialCards { Wild, WildDraw4, Draw2, Skip, Reverse, None };

    public static readonly Dictionary<string, CardValue> CardValueMap = new Dictionary<string, CardValue>
    {
        {"0", CardValue.Zero},
        {"1", CardValue.One},
        {"2", CardValue.Two},
        {"3", CardValue.Three},
        {"4", CardValue.Four},
        {"5", CardValue.Five},
        {"6", CardValue.Six},
        {"7", CardValue.Seven},
        {"8", CardValue.Eight},
        {"9", CardValue.Nine},
    };
    }
}
