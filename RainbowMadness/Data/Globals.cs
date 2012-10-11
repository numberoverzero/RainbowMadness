using System.Collections.Generic;
using Engine.Utility;

namespace RainbowMadness.Data
{
    public static class Global
    {
        public static readonly Dictionary<string, int>
            ColorMap = new Dictionary<string, int>
                           {
                               {"None", -1},
                               {"Red", 0},
                               {"Yellow", 1},
                               {"Green", 2},
                               {"Blue", 3},
                           };

        public static readonly Dictionary<string, int>
            TypeMap = new Dictionary<string, int>
                          {
                              {"0", 0},
                              {"1", 0},
                              {"2", 0},
                              {"3", 0},
                              {"4", 0},
                              {"5", 0},
                              {"6", 0},
                              {"7", 0},
                              {"8", 0},
                              {"9", 0},
                              {"Skip", 1},
                              {"Reverse", 2},
                              {"Draw2", 3},
                              {"Wild", 4},
                              {"WildDraw4", 4},
                              {"Swap", 5},
                          };

        public static readonly Dictionary<string, int>
            ValueMap = new Dictionary<string, int>
                           {
                               {"0", 0},
                               {"1", 1},
                               {"2", 2},
                               {"3", 3},
                               {"4", 4},
                               {"5", 5},
                               {"6", 6},
                               {"7", 7},
                               {"8", 8},
                               {"9", 9},
                               {"Skip", -1},
                               {"Reverse", -1},
                               {"Draw2", 2},
                               {"Wild", 0},
                               {"WildDraw4", 4},
                               {"Swap", -1},
                           };
    }
}