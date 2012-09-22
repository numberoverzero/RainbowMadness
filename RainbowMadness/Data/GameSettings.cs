using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainbowMadness.Data
{
    public class GameSettings
    {
        public bool ColorblindMode { get; set; }
        public int NPlayers { get; set; }
        public bool DrawUntilPlayable { get; set; }
        public bool CanPlayAfterDraw { get; set; }
        public string JoinServer { get; set; }
    }
}
