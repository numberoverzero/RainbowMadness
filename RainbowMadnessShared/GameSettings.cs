namespace RainbowMadnessShared
{
    public struct GameSettings
    {
        public int MaxPlayers { get; set; }
        public bool DrawUntilPlayable { get; set; }
        public bool CanPlayAfterDraw { get; set; }
        public int CardsPerStartingHand { get; set; }
        public string DeckFilename { get; set; }

        public string LogFilename { get; set; }
        public string HostIP { get; set; }
        public int Port { get; set; }
    }
}