using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.DataStructures;
using Engine.Utility;

namespace RainbowMadness.Data
{
    public class Game
    {
        string deckFileName;
        protected ICollection<Card> deck;
        protected List<Card> stack;

        protected Player[] players;
        protected int nPlayers;
        protected int playerIndex;
        protected bool reverse = false;

        public Card Top
        {
            get
            {
                if (stack == null || stack.Count == 0)
                    return Card.NullCard;
                return stack[stack.Count - 1];
            }
            set
            {
                stack.Add(value);
            }
        }
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

        public Game(string deckFileName)
        {
            this.deckFileName = deckFileName;
            ResetDeck();
            Console.WriteLine("\n".Join(deck));
        }

        public Card DrawCard()
        {
            var card = deck.PopRandomElement();
            if (card == null)
            {
                ResetDeck();
                card = deck.PopRandomElement();
            }
            return card;
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

        private void ResetDeck()
        {
            deck = Parsers.ParseDeck(deckFileName);
        }
    }
}
