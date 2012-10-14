using System;
using System.Collections.Generic;
using Engine.Networking;

namespace RainbowMadness.Data
{
    /// <summary>
    /// This class shouldn't be needed.
    /// </summary>
    public class _Player
    {
        private readonly List<Card> hand;
        private readonly Client Client;
        public string Name;

        public _Player(Client client, string name)
        {
            hand = new List<Card>();
            Client = client;
            Name = name;
        }

        public int GetWildColor()
        {
            throw new NotImplementedException();
        }

        public void DrawCard(Game game, int nCards)
        {
            for (int i = 0; i < nCards; i++)
                hand.Add(game.DrawCard());
        }

        public override bool Equals(object obj)
        {
            return GetHashCode() == obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            return Client.GetHashCode();
        }
    }
}