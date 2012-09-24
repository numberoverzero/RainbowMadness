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
        readonly string _deckFileName;
        protected ICollection<Card> Deck;
        protected List<Card> Stack;

        protected Player[] Players;
        protected int PlayerIndex;
        protected bool Reverse = false;

        public Card Top
        {
            get
            {
                if (Stack == null || Stack.Count == 0)
                    return Card.NullCard;
                return Stack[Stack.Count - 1];
            }
            set
            {
                Stack.Add(value);
            }
        }
        public Player CurrentPlayer
        {
            get
            {
                return Players[PlayerIndex];
            }
        }
        public Player NextPlayer
        {
            get
            {
                return Players[NextPlayerIndex()];
            }
        }

        public GameSettings Settings;

        public Game(string deckFileName, GameSettings settings)
        {
            this._deckFileName = deckFileName;
            Settings = settings;
            ResetDeck();
        }

        public Card DrawCard()
        {
            var card = Deck.PopRandomElement();
            if (card == null)
            {
                ResetDeck();
                card = Deck.PopRandomElement();
            }
            return card;
        }

        public void AdvancePlayer()
        {
            PlayerIndex = NextPlayerIndex();
        }

        private int NextPlayerIndex()
        {
            int offset = Reverse ? -1 : 1;
            return MathExtensions.WrappedIndex(PlayerIndex + offset, Settings.NPlayers);
        }

        public void ReversePlayDirection()
        {
            Reverse = !Reverse;
        }

        private void ResetDeck()
        {
            Deck = Parsers.ParseDeck(_deckFileName);
        }
    }
}
