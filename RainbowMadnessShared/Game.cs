using System;
using System.Collections.Generic;
using System.Linq;
using Engine.DataStructures;
using Engine.Utility;

namespace RainbowMadnessShared
{
    public class Game
    {
        public CountedCollection<Card> Deck; // Cards not yet drawn
        public int PlayerIndex;
        public List<string> Players;
        public DefaultObjDict<string, List<Card>> PlayersCards;
        public bool Reverse;
        public GameSettings Settings;
        public bool IsGameStarted { get; protected set; }
        public List<Card> Stack; // Cards played
        public string Winner = "";

        public Game(GameSettings settings)
        {
            IsGameStarted = false;
            Settings = settings;
            ResetDeck();
            Reverse = false;
            Players = new List<string>();
            PlayersCards = new DefaultObjDict<string, List<Card>>();
            Stack = new List<Card>();
        }

        public Game(Game game) : this(game.Settings)
        {
            
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

        public void Start()
        {
            if (IsGameStarted) return;

            // If we're starting from a won game, we need to reset everything.  We should keep the players though
            if(!String.IsNullOrEmpty(Winner))
            {
                ResetGameFromWonState();
                Start();
            }
            IsGameStarted = true;
            Top = DrawCard();
            if (Top.IsWild) Top.Color = new Random().Next(0, 4);
            CheckTriggeredEffects();
            DrawIfNeeded();
        }

        private void ResetGameFromWonState()
        {
            Winner = "";
            // Reset Deck and re-deal everyone cards
            ResetDeck();
            Stack.Clear();
            Reverse = false;
            PlayerIndex = 0;

            var playersCopy = new List<string>(Players);
            PlayersCards.Clear();
            Players.Clear();
            playersCopy.Each(AddPlayer);
        }

        public String CurrentPlayer
        {
            get { return Players[PlayerIndex]; }
        }

        public String NextPlayer
        {
            get { return Players[NextPlayerIndex()]; }
        }

        public void AddPlayer(string player)
        {
            if (IsGameStarted) return; // Can't modify player list in a started game
            Players.Add(player);
            Settings.CardsPerStartingHand.TimesDo(() => PlayerDrawCard(player));

            // Auto-start when max players reached
            if (Players.Count == Settings.MaxPlayers) Start();
        }

        public void RemovePlayer(string player)
        {
            if (!Players.Contains(player)) return;
            Players.Remove(player);
            if (!IsGameStarted) PlayersCards[player].Each(Deck.Add);
            PlayersCards.Remove(player);
        }

        private Card DrawCard()
        {
            var card = Deck.PopRandomElement();
            if (card == null)
            {
                ResetDeck();
                card = Deck.PopRandomElement();
            }
            return card;
        }

        private int PreviousPlayerIndex()
        {
            int offset = Reverse ? 1 : -1;
            return MathExtensions.WrappedIndex(PlayerIndex + offset, Players.Count);
        }

        private int NextPlayerIndex()
        {
            int offset = Reverse ? -1 : 1;
            return MathExtensions.WrappedIndex(PlayerIndex + offset, Players.Count);
        }

        private void ReversePlayDirection()
        {
            Reverse = !Reverse;
        }

        private void ResetDeck()
        {
            Deck = Parsers.ParseDeck(Settings.DeckFilename);
        }

        public void PrintDeck()
        {
            foreach (var card in Deck)
            {
                Console.WriteLine(card);
            }
        }

        public bool CanPlayCard(Card card, int cardIndex, string player)
        {
            return IsGameStarted && (player == CurrentPlayer) && card.CanPlay(Top);
        }

        public void PlayCard(Card card, int cardIndex, string player)
        {
            lock (this)
            {
                if (!CanPlayCard(card, cardIndex, player))
                    throw new NotImplementedException("Did you check with CanPlayCardFirst?");

                PlayersCards[player].RemoveAt(cardIndex);
                Top = card;

                PlayerIndex = NextPlayerIndex();
                CheckTriggeredEffects();
                DrawIfNeeded();
            }
        }

        private void CheckTriggeredEffects()
        {
            // Check win condition for current player
            if(PlayersCards[PreviousPlayer].Count == 0)
            {
                Winner = PreviousPlayer;
                IsGameStarted = false;
                return;
            }

            var card = Top;
            if (card.IsReverse)
            {
                PlayerIndex = PreviousPlayerIndex();
                ReversePlayDirection();
                PlayerIndex = NextPlayerIndex();
            }
            if (card.IsSkip)
            {
                PlayerIndex = NextPlayerIndex();
            }
            if (card.IsDraw) card.Value.TimesDo(() => PlayerDrawCard(CurrentPlayer));
        }

        protected string PreviousPlayer
        {
            get { return Players[PreviousPlayerIndex()]; }
        }

        protected void DrawIfNeeded()
        {
            var player = CurrentPlayer;
            if (PlayerHasValidPlay(player)) return;

            // Player doesn't have any valid plays, needs to draw.
            
            var card = PlayerDrawCard(player); // Have to draw at least one
            if (Settings.DrawUntilPlayable)
                while (!card.CanPlay(Top))
                    card = PlayerDrawCard(player);

            if (!Settings.CanPlayAfterDraw)
            {
                PlayerIndex = NextPlayerIndex();
                DrawIfNeeded();
                    // If they can't play after they draw, advance the turn to the next player.
            }
        }

        private bool PlayerHasValidPlay(string player)
        {
            return PlayersCards[player].Any(card => card.CanPlay(Top));
        }

        private Card PlayerDrawCard(string player)
        {
            var card = DrawCard();
            PlayersCards[player].Add(card);
            PlayersCards[player].Sort();
            return card;
        }
    }
}