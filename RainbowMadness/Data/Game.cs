using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainbowMadness.Data
{
    public class Game
    {
        protected List<Card> stack;
        public Card Top
        {
            get
            {
                if (stack == null || stack.Count == 0)
                    return Card.NullCard;
                return stack[stack.Count - 1];
            }
        }
        public bool SetTop(Card card)
        {
            stack.Add(card);
            return true;
        }

        protected Player[] players;
        protected int nPlayers;
        protected int playerIndex;
        protected bool reverse = false;

        public Player CurrentPlayer
        {
            get
            {
                return players[playerIndex];
            }
        }
        public Player NextPlayer
        {
            get
            {
                return players[NextPlayerIndex()];
            }
        }

        public void AdvancePlayer()
        {
            playerIndex = NextPlayerIndex();
        }

        private int NextPlayerIndex()
        {
            int offset = reverse ? -1 : 1;
            return Util.WrappedIndex(nPlayers, playerIndex + offset);
        }

        public void ReversePlayDirection()
        {
            reverse = !reverse;
        }
    }
}
