using System;
using System.Collections.Generic;
using Engine.DataStructures;
using Engine.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace RainbowMadness.Data
{
    public class Game
    {
        private readonly string _deckFileName;
        protected CountedCollection<Card> Deck;
        protected int PlayerIndex;
        protected DefaultObjDict<string, TextBox> PlayerTextBoxes;
        protected List<string> Players;
        protected DefaultObjDict<string, List<Card>> PlayersCards;
        protected bool Reverse = false;
        public GameSettings Settings;
        protected List<Card> Stack;
        private static Texture2D _currentPlayerArrowTexture;
        private Vector2 _currentPlayerArrowPos;

        public static void Initialize(ContentManager content)
        {
            _currentPlayerArrowTexture = content.Load<Texture2D>(@"arrow");
        }

        public Game(string deckFileName, GameSettings settings)
        {
            _deckFileName = deckFileName;
            Settings = settings;
            ResetDeck();
            Players = new List<string>();
            PlayersCards = new DefaultObjDict<string, List<Card>>();
            PlayerTextBoxes = new DefaultObjDict<string, TextBox>();

            if(!String.IsNullOrEmpty(Settings.LocalPlayer) && Settings.IsHost) AddPlayer(Settings.LocalPlayer);
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
            Players.Add(player);
            Settings.CardsPerStartingHand.TimesDo(() => PlayersCards[player].Add(DrawCard()));
            RecalculateTextBoxes();

            if(Players.Count < 3)
            {
                UpdatePlayerHighlighting();
                UpdateCurrentPlayerPointer();
            }
                
        }

        public void RemovePlayer(string player)
        {
            Players.Remove(player);
            PlayersCards.Remove(player);
            PlayerTextBoxes.Remove(player);
            RecalculateTextBoxes();
            if (Players.Count < 1) return;
            UpdateCurrentPlayerPointer();
            UpdatePlayerHighlighting();
        }

        private void RecalculateTextBoxes()
        {
            var screenDims = ScreenManager.Dimensions;
            var xInfoBox = (int)screenDims.X;
            var yInfoBox = 0;
            PlayerTextBoxes.Clear();
            var maxWidth = 0;
            var vPad = 0.1f;

            foreach (var player in Players)
            {
                var nCards = PlayersCards[player].Count;
                var playerIndex = Players.IndexOf(player);
                var nPlayers = Players.Count;
                var textBox = new TextBox(){HasBorder = true};
                
                // C# points to the value, not the variable, so we need a local copy for the function to grab the correct value.  Fixed in C# 5.0
                var player_name = player;
                Func<string> textFunc = () => "{0} ({1} Cards)".format(player_name, PlayersCards[player_name].Count);
                textBox.TextFunc = textFunc;
                var boxHeight = textBox.Height;
                // pad the box
                yInfoBox += (int) ((1+vPad)*boxHeight);
                maxWidth = Math.Max(textBox.ActualWidth, maxWidth);
                textBox.Y = yInfoBox;
                PlayerTextBoxes[player] = textBox;
            }

            xInfoBox -= (int)(maxWidth*(1.1f));

            foreach (var box in PlayerTextBoxes.Values)
            {
                box.MinimumWidth = maxWidth;
                box.X = xInfoBox;
            }
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
            var lastPlayer = CurrentPlayer;
            PlayerTextBoxes[lastPlayer].Highlighted = false; // No longer the current player
            PlayerIndex = NextPlayerIndex();
            UpdateCurrentPlayerPointer();
            UpdatePlayerHighlighting();
        }

        private void UpdatePlayerHighlighting()
        {
            PlayerTextBoxes[CurrentPlayer].Highlighted = true;
            if(Players.Count > 1) PlayerTextBoxes[NextPlayer].Highlighted = true;
            
        }

        private void UpdateCurrentPlayerPointer()
        {
            var currentBox = PlayerTextBoxes[CurrentPlayer];
            var boxCorner = new Vector2(currentBox.X, currentBox.Y);
            var pixPad = 5;
            _currentPlayerArrowPos = boxCorner;
            _currentPlayerArrowPos.X -= pixPad;
            _currentPlayerArrowPos.X -= _currentPlayerArrowTexture.Dimensions().X;
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

        public void DrawGame(SpriteBatch batch)
        {
            DrawPlayerInfo(batch);
            DrawPlayerHand(batch, Settings.LocalPlayer);
            DrawDeckAndTop(batch);
        }

        private void DrawPlayerInfo(SpriteBatch batch)
        {
            PlayerTextBoxes.Values.Each(box => box.Draw(batch));
            batch.Draw(_currentPlayerArrowTexture, _currentPlayerArrowPos, Color.White);
        }

        private void DrawPlayerHand(SpriteBatch batch, string player)
        {
            var cards = PlayersCards[player];
            var nCards = cards.Count;
            var cardDims = Card.GraphicDimensions;
            var screenDims = ScreenManager.Dimensions;

            var availWidth = ScreenManager.Dimensions.X;
            //Leave 10% on each side
            availWidth *= 0.8f;


            var widthPerCardWithPadding = availWidth/nCards;
            var padPerCard = widthPerCardWithPadding*0.1f;
            var widthPerCard = widthPerCardWithPadding - padPerCard;

            var cardScale = Math.Min(1.0f, widthPerCard/cardDims.X);

            var scaledHeight = cardScale*cardDims.Y;

            var x0 = padPerCard/2;
            var deltaX = widthPerCard/2 + padPerCard/2;
            var y = screenDims.Y - padPerCard - scaledHeight;

            var pos = new Vector2(x0, y);
            foreach (var card in cards)
            {
                card.Draw(batch, pos, cardScale);
                pos.X += deltaX*2;
            }
        }

        private void DrawDeckAndTop(SpriteBatch batch)
        {
            var center = ScreenManager.Dimensions/2;
            center.X *= 0.5f;
            var scale = 0.6f;
            var cardDims = Card.GraphicDimensions;
            cardDims *= scale;

            var padVector = new Vector2(0.05f*cardDims.X, 0);

            // Draw "deck" to the left
            var deckPos = center - padVector + new Vector2(-cardDims.X, -cardDims.Y/2);
            Top.Draw(batch, deckPos, scale);

            // Draw "top" to the right
            var topPos = center + padVector + new Vector2(0, -cardDims.Y/2);
            ;
            Top.Draw(batch, topPos, scale);

            //Draw deck count
            var text = "Cards in Deck: {0}".format(Deck.InstanceCount);
            var textSize = ScreenManager.Font.MeasureString(text);

            var textX = center.X - textSize.X/2;
            var textY = center.Y - cardDims.Y/2*(1.1f) - textSize.Y/2;

            batch.DrawString(ScreenManager.Font, text, new Vector2(textX, textY), Color.White);
        }

        public void Update(float dt)
        {
            var input = ScreenManager.Input;
        }
    }
}