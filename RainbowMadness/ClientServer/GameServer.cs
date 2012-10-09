using System.Net;
using Engine.Networking;
using RainbowMadness.Data;

namespace RainbowMadness
{
    public class GameServer : BasicServer
    {
        public GameServer(IPAddress localaddr, int port, string logFileName = null) : base(localaddr, port, logFileName)
        {

        }
    }
}