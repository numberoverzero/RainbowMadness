using System.Net;
using Engine.DataStructures;
using Engine.Networking;
using RainbowMadness.Data;
using RainbowMadness.Packets;

namespace RainbowMadness
{
    public class GameServer : BasicServer
    {
        private BidirectionalDict<string, Client> playerTable;
        private Game game;

        public GameServer(IPAddress localaddr, int port, string logFileName = null) : base(localaddr, port, logFileName)
        {
            playerTable = new BidirectionalDict<string, Client>();
        }

        private void HandlePlayCardRequest(PlayCardRequestPacket packet, Client client)
        {
            var player = playerTable[client];
            var card = packet.Card;
            string message;

            var canPlay = Card.CanPlay(game, player, card, out message);

            if(!canPlay)
            {
                var response = new PlayCardResponsePacket {IsPlayed = false, Message = message};
                SendPacket(response, client);
                return;
            }

            game.Top = card;


        }
    }
}