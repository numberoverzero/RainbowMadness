using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Engine.Networking;
using RainbowMadness.Data;

namespace RainbowMadness
{
    public class GameServer
    {
        private Data.Game _game;
        private BasicServer _server;
        public GameServer(IPAddress localaddr, int port, GameSettings settings)
        {
            _game = new Data.Game(@"Content\Decks\cards.txt", settings);
            _server = new BasicServer(localaddr, port, null);
            _server.Start();
        }


        public bool CanPlayCard(Player player, Card card)
        {
            return card.CanPlay(_game);
        }

        public bool PlayCard(Card card)
        {
            //if (!CanPlayCard(card)) return false;
            _game.Top = card;
            return true;
        }

        //public bool CanDrawCard()
    }
}
