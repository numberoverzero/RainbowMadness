using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Networking;

namespace RainbowMadness.Data
{
    public class Player
    {
        List<Card> hand;
        public Client Client;

        public Player(Client client)
        {
            hand = new List<Card>();
            Client = client;
        }

        public Globals.CardColor GetWildColor()
        {
            throw new NotImplementedException();
        }

        public void DrawCard(Game game, int nCards)
        {
            for (int i = 0; i < nCards; i++)
                hand.Add(game.DrawCard());
        }

        //public void 
    }
}
