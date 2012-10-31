using Microsoft.Xna.Framework;

namespace RainbowMadness.Data
{
    public class GameSettings
    {
        public bool ColorblindMode { get; set; }
        public int NPlayers { get; set; }
        public bool DrawUntilPlayable { get; set; }
        public bool CanPlayAfterDraw { get; set; }
        public string ServerName { get; set; }
        public string LocalPlayer { get; set; }
        public bool IsHost { get; set; }
        public Point Resolution { get; set; }
        public int CardsPerStartingHand { get; set; }

        public GameSettings()
        {
            Resolution = new Point(1024, 768);
            CardsPerStartingHand = 7;
        }
    }
}