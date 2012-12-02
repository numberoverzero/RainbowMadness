using System;
using System.Net;
using Engine.DataStructures;
using Engine.Networking;
using Engine.Networking.Packets;
using Engine.Networking.Server;
using Engine.Utility;
using RainbowMadnessShared;

namespace RainbowMadnessServer
{
    public class GameServer : SingleThreadServer
    {
        protected Game Game;
        protected BidirectionalDict<string, Client> PlayerTable;
        protected GameSettings Settings;

        public GameServer(GameSettings settings) : base(IPAddress.Any, settings.Port, settings.LogFilename)
        {
            OnDisconnect += Handle_OnDisconnect;
            OnConnect += Handle_OnConnect;
            PlayerTable = new BidirectionalDict<string, Client>();
            Game = new Game(settings);
            Settings = settings;
        }

        private void Handle_OnConnect(object sender, ServerEventArgs e)
        {
            SendPacket(new RequestAuthPacket(), e.Client);
        }

        private void Handle_OnDisconnect(object sender, ServerEventArgs e)
        {
            var client = e.Client;
            if (!PlayerTable.Contains(client)) return;
            var username = PlayerTable[client];
            PlayerTable.Remove(username);
            Game.RemovePlayer(username);
            UpdateGameState();
            if (Game.Players.Count == 0) Game = new Game(Settings);
        }

        public override void ReceivePacket(Packet packet, Client client)
        {
            if (packet is AuthenticateUserPacket)
                HandleAuthenticatePacket(packet as AuthenticateUserPacket, client);
            else if (packet is UserDisconnectPacket)
                HandleDisconnectRequest(packet as UserDisconnectPacket, client);
            else if (packet is StartGamePacket)
                HandleStartRequest(packet as StartGamePacket);
            else if (packet is PlayCardRequestPacket)
                HandlePlayCardRequest(packet as PlayCardRequestPacket, client);
        }

        private void HandleStartRequest(StartGamePacket packet)
        {
            if (Game.IsGameStarted) return;
            Game.Start();
            UpdateGameState();
        }

        private void HandleAuthenticatePacket(AuthenticateUserPacket packet, Client client)
        {
            if(Game.IsGameStarted || Game.Players.Count >= Game.Settings.MaxPlayers)
            {
                SendPacket(new ServerDisconnectPacket(), client);
                Disconnect(client);
                return;
            }
            var username = packet.Username;
            PlayerTable[client] = username;
            Game.AddPlayer(username);
            UpdateGameState();
        }

        private void HandleDisconnectRequest(UserDisconnectPacket packet, Client client)
        {
            Disconnect(client);
        }

        private void HandlePlayCardRequest(PlayCardRequestPacket packet, Client client)
        {
            var player = PlayerTable[client];
            var card = packet.Card;
            var cardIndex = packet.CardIndex;

            if (!Game.CanPlayCard(card, cardIndex, player)) return;
            Game.PlayCard(card, cardIndex, player);
            UpdateGameState();
        }

        public void UpdateGameState(params string[] players)
        {
            if (players.Length == 0) players = Game.Players.ToArray();
            players.Each(player =>
                             {
                                 var packet = GameStatePacket.FromGame(Game, player);
                                 var client = PlayerTable[player];
                                 SendPacket(packet, client);
                             });
        }
    }
}