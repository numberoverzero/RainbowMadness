using RainbowMadnessShared;

namespace RainbowMadnessServer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            PacketGlobals.Initialize();
            if (args.Length < 1) return;
            var settingsFilename = args[0];
            var settings = Parsers.ParseSettings(settingsFilename);
            var server = new GameServer(settings);
            server.Start();
        }
    }
}