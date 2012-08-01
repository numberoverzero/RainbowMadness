using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainbowMadness.Data
{
    public class Player
    {
        List<Card> hand;

        public Player()
        {
            hand = new List<Card>();
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
    }
}
