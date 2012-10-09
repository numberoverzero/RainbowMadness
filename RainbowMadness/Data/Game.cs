using System;
using System.Collections.Generic;
using Engine.Utility;

namespace RainbowMadness.Data
{
    public class Game
    {
        private readonly string _deckFileName;
        protected ICollection<Card> Deck;
        protected int PlayerIndex;
        protected Player[] Players;
        protected bool Reverse = false;
        public GameSettings Settings;
        protected List<Card> Stack;

        public Game(string deckFileName, GameSettings settings)
        {
            _deckFileName = deckFileName;
            Settings = settings;
            ResetDeck();
        }

        public Card Top
        {
            get
            {
                if (Stack == null || Stack.Count == 0)
                    return Card.NullCard;
                return Stack[Stack.Count - 1];
            }
            set { Stack.Add(value); }
        }

        public Player CurrentPlayer
        {
            get { return Players[PlayerIndex]; }
        }

        public Player NextPlayer
        {
            get { return Players[NextPlayerIndex()]; }
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

        public void PrintDeck()
        {
            foreach (var card in Deck)
            {
                Console.WriteLine(card);
            }
        }
    }
}