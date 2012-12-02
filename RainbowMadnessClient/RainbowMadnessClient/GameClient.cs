using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Engine.DataStructures;
using Engine.Input.Managers.SinglePlayer;
using Engine.Networking;
using Engine.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RainbowMadnessClient.Menus.Game_Specific_Menus;
using RainbowMadnessShared;

namespace RainbowMadnessClient
{
    public class GameClient : IScreen
    {
        #region Members

        public static Texture2D CurrentPlayerArrowTexture;
        private readonly string _username;
        public List<Card> Cards;
        public Client Client;
        private int _currentCardIndex;
        public int CurrentCardIndex
        { 
            get { return _currentCardIndex; } 
            set { _currentCardIndex = MathExtensions.WrappedIndex(value, Cards.Count); }
        }
        public Vector2 CurrentPlayerArrowPos;
        public int CurrentPlayerIndex;
        public int DeckSize;
        public Dictionary<string, int> PlayerHandSizes;
        public DefaultObjDict<string, TextBox> PlayerTextBoxes;
        public List<string> Players;
        public bool Reverse;
        public GameSettings Settings;
        public Card Top;
        public bool IsGameStarted;
        public string Winner;
        private float warnAlpha = 1.0f;
        private float warnAlphaDelta = 0.01f;
        private bool isUpdating = false;

        #endregion

        public GameClient(string server, string username)
        {
            Settings = ScreenManager.Settings;
            _username = username;
            Cards = new List<Card>();
            CurrentCardIndex = 0;
            Players = new List<string>();
            PlayerHandSizes = new Dictionary<string, int>();
            PlayerTextBoxes = new DefaultObjDict<string, TextBox>();
            RecalculateTextBoxes();
            CurrentPlayerIndex = 0;
            Top = new Card();
            DeckSize = -1;
            Reverse = false;
            IsGameStarted = false;
            Connect(server);

            CurrentPlayerArrowTexture = ScreenManager.Content.Load<Texture2D>(@"arrow");
        }

        private String CurrentPlayer
        {
            get { return Players[CurrentPlayerIndex]; }
        }

        private String NextPlayer
        {
            get { return Players[NextPlayerIndex()]; }
        }

        private int NextPlayerIndex()
        {
            int offset = Reverse ? -1 : 1;
            return MathExtensions.WrappedIndex(CurrentPlayerIndex + offset, Players.Count);
        }

        #region IScreen Members

        public bool IsPopup { get; private set; }

        public void Draw(SpriteBatch batch)
        {
            if (isUpdating) return;
            DrawHand(batch);
            DrawDeck(batch);
            DrawPlayers(batch);
            DrawInfo(batch);
            DrawChat(batch);
        }

        public void Update(float dt)
        {
            var input = ScreenManager.Input;
            if (input.IsPressed("menu_back"))
            {
                ScreenManager.OpenScreen(new ConfirmDialog(false)
                                             {
                                                 Message = "Are you sure you want to leave this game?",
                                                 Confirm = "Yes",
                                                 Cancel = "No",
                                                 OnConfirm = () => ScreenManager.CloseScreen(this)
                                             });
            }
            if (input.IsPressed("card_left")) CurrentCardIndex--;
            if (input.IsPressed("card_right")) CurrentCardIndex++;
            if(input.IsPressed("play_card") || input.IsPressed("play_card2"))
            {
                var card = Cards[CurrentCardIndex];
                if (card.IsWild) PlayWildCard(CurrentCardIndex);
                else PlayCard(card, CurrentCardIndex);
            }
            if (input.IsPressed("start_game"))
                StartGame();

            warnAlpha += warnAlphaDelta;
            if (warnAlpha > 1.0f || warnAlpha < 0.0f)
            {
                warnAlphaDelta *= -1;
                warnAlpha += 2 * warnAlphaDelta;
            }
        }

        private void StartGame()
        {
            if(!IsGameStarted) Client.WritePacket(new StartGamePacket());
        }

        public void OnClose(bool asCleanup)
        {
            Disconnect();
        }

        #endregion

        #region Drawing

        private void RecalculateTextBoxes()
        {
            var screenDims = ScreenManager.Dimensions;
            var xInfoBox = (int)screenDims.X;
            var yInfoBox = 0;
            PlayerTextBoxes.Clear();
            var maxWidth = 0;
            const float vPad = 0.1f;

            foreach (var player in Players)
            {
                var nCards = PlayerHandSizes[player];
                var playerIndex = Players.IndexOf(player);
                var nPlayers = Players.Count;
                var textBox = new TextBox { HasBorder = true };

                // C# points to the value, not the variable, so we need a local copy for the function to grab the correct value.  Fixed in C# 5.0
                var playerName = player;
                textBox.Text = "{0} ({1} Cards)".format(playerName, PlayerHandSizes[playerName]);
                var boxHeight = textBox.Height;
                // pad the box
                yInfoBox += (int)((1 + vPad) * boxHeight);
                maxWidth = Math.Max(textBox.ActualWidth, maxWidth);
                textBox.Y = yInfoBox;
                PlayerTextBoxes[player] = textBox;
            }

            xInfoBox -= (int)(maxWidth * (1.1f));

            foreach (var box in PlayerTextBoxes.Values)
            {
                box.MinimumWidth = maxWidth;
                box.X = xInfoBox;
            }
        }

        private void UpdatePlayerHighlighting()
        {
            if (Players.Count < 1) return;
            PlayerTextBoxes.Values.Each(box => box.Highlighted = false);
            PlayerTextBoxes[CurrentPlayer].Highlighted = true;
            PlayerTextBoxes[NextPlayer].Highlighted = true;
        }

        private void UpdateCurrentPlayerPointer()
        {
            if (Players.Count < 1)
            {
                CurrentPlayerArrowPos = new Vector2(-200);
                return;
            }
            var currentBox = PlayerTextBoxes[CurrentPlayer];
            var boxCorner = new Vector2(currentBox.X, currentBox.Y);
            const int pixPad = 5;
            CurrentPlayerArrowPos = boxCorner;
            CurrentPlayerArrowPos.X -= pixPad;
            CurrentPlayerArrowPos.X -= CurrentPlayerArrowTexture.Dimensions().X;
        }

        private void DrawChat(SpriteBatch batch)
        {
            // Draw chat window, scrolled

            //throw new NotImplementedException();
        }

        private void DrawInfo(SpriteBatch batch)
        {
            batch.Draw(CurrentPlayerArrowTexture, CurrentPlayerArrowPos, Color.White);
            if (IsGameStarted) return;

            var winnerText = "{0} won the game!";
            const string startText = "Press Ctrl+Shift+S to start the game!";

            var font = ScreenManager.BigFont;
            const float textBorderScale = 1.001f;
            var textSize = font.MeasureString(startText);
            var textBorderSize = textBorderScale*textSize;
            var center = ScreenManager.Dimensions/2;
            var textPos = center - textSize/2;
            var textBorderPos = center - textBorderSize/2;
            var textBorderColor = new Color(0.0f, 0.0f, 0.0f, warnAlpha * 0.7f);
            var textColor = new Color(warnAlpha, warnAlpha, warnAlpha, 1);


            if(!String.IsNullOrEmpty(Winner))
            {
                winnerText = winnerText.format(Winner);
                var posOffset = new Vector2(0, -textSize.Y);
                batch.DrawString(font, winnerText, textBorderPos+posOffset, textBorderColor, 0, Vector2.Zero, textBorderScale, SpriteEffects.None, 0);
                batch.DrawString(font, winnerText, textPos + posOffset, textColor, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            }

            textPos.Y += textSize.Y;
            textBorderPos.Y += textBorderSize.Y;

            

            batch.DrawString(font, startText, textBorderPos, textBorderColor, 0, Vector2.Zero, textBorderScale, SpriteEffects.None, 0);
            batch.DrawString(font, startText, textPos, textColor, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

            // Draw info msg box
        }

        private void DrawPlayers(SpriteBatch batch)
        {
            PlayerTextBoxes.Values.Each(box => box.Draw(batch));
        }

        private void DrawDeck(SpriteBatch batch)
        {
            var center = ScreenManager.Dimensions/2;
            center.X *= 0.59f;
            center.Y *= 0.56f;
            const float scale = 0.85f;
            var cardDims = Card.GraphicDimensions;
            cardDims *= scale;

            var padVector = new Vector2(0.05f*cardDims.X, 0);

            // Draw "deck" to the left
            var deckPos = center - padVector + new Vector2(-cardDims.X, -cardDims.Y/2);
            Card.BackCard.Draw(batch, ScreenManager.Font, deckPos, scale, true);

            // Draw "top" to the right
            var topPos = center + padVector + new Vector2(0, -cardDims.Y/2);

            Top.Draw(batch, ScreenManager.Font, topPos, scale, true);
        }

        private void DrawHand(SpriteBatch batch)
        {
            var nCards = Cards.Count;
            var cardDims = Card.GraphicDimensions;
            var screenDims = ScreenManager.Dimensions;

            var availWidth = ScreenManager.Dimensions.X;
            //Leave 10% on each side
            //availWidth *= 0.8f;

            var selectedScale = 1.5f;
            if (nCards >= 10) selectedScale += 0.15f*(nCards - 9);

            var widthPerCardWithPadding = availWidth/(nCards + selectedScale - 1);
            var padPerCard = widthPerCardWithPadding*0.1f;
            var widthPerCard = widthPerCardWithPadding - padPerCard;

            var cardScale = Math.Min(0.406836352f, widthPerCard/cardDims.X);
            var currentCardScale = cardScale * selectedScale;

            var scaledHeight = cardScale*cardDims.Y;
            var currentCardScaledHeight = currentCardScale*cardDims.Y;

            var x0 = padPerCard/2;
            var deltaX = widthPerCard/2 + padPerCard/2;
            var y = screenDims.Y - padPerCard - scaledHeight;
            var selectedY = screenDims.Y - padPerCard - currentCardScaledHeight;

            var pos = new Vector2(x0, y);
            for (int i = 0; i < Cards.Count; i++)
            {
                var card = Cards[i];
                var scale = i == CurrentCardIndex ? currentCardScale : cardScale;
                var cardY = i == CurrentCardIndex ? selectedY : y;
                pos.Y = cardY;
                card.Draw(batch, ScreenManager.Font, pos, scale, false);
                pos.X += deltaX*2*scale/cardScale;
            }
        }

        #endregion

        #region Networking

        public void Connect(string server)
        {
            // See if it's one of the server favorites
            var favoritesFilename = ScreenManager.Content.RootDirectory + @"\Servers.ini";
            var favorites = Parsers.ParseServerFavorites(favoritesFilename);
            if (favorites.ContainsKey(server.ToLower()))
                server = favorites[server.ToLower()];

            var hostAndPort = server.Split(':');
            var host = hostAndPort[0];
            var port = Int32.Parse(hostAndPort[1]);

            TcpClient baseClient;
            try
            {
                baseClient = new TcpClient(host, port);
            }
            catch (SocketException)
            {
                ScreenManager.CloseScreen(this);
                return;
            }

            Client = new Client(baseClient);
            Client.OnReadPacket += OnClientRead;
            Client.OnConnectionLost += (o, args) => ScreenManager.CloseScreen(this);
        }

        private void OnClientRead(object sender, PacketArgs args)
        {
            var client = args.Client;
            if (client == null) return;
            var packet = args.Packet;
            if (packet == null) return;

            if(packet is RequestAuthPacket)
                Client.WritePacket(new AuthenticateUserPacket{Username = _username});
            else if (packet is GameStatePacket)
                UpdateGameState(packet as GameStatePacket);
            else if (packet is ServerDisconnectPacket)
                ScreenManager.CloseScreen(this);
        }

        private void Disconnect()
        {
            if (Client == null) return;
            Client.WritePacket(new UserDisconnectPacket());
            Client.Close();
        }

        private void UpdateGameState(GameStatePacket packet)
        {
            isUpdating = true;
            Cards = packet.Cards;
            CurrentCardIndex = MathExtensions.WrappedIndex(CurrentCardIndex, Cards.Count);

            IsGameStarted = packet.IsGameStarted;
            DeckSize = packet.DeckSize;
            Players = packet.Players;
            Top = packet.Top;
            Reverse = packet.Reverse;
            CurrentPlayerIndex = packet.PlayerIndex;
            Winner = packet.Winner;
            PlayerHandSizes.Clear();
            for (var i = 0; i < Players.Count; i++)
                PlayerHandSizes[Players[i]] = packet.PlayerHandSizes[i];

            RecalculateTextBoxes();
            UpdateCurrentPlayerPointer();
            UpdatePlayerHighlighting();
            isUpdating = false;
        }

        #endregion

        #region Updating

        private void PlayCard(Card card, int index)
        {
            Client.WritePacket(new PlayCardRequestPacket {Card = card, CardIndex = CurrentCardIndex});
        }

        private void PlayWildCard(int index)
        {
            Action<string> onSelectWild = s =>
            {
                if (s == null || !Global.ColorMap.ContainsKey(s)) return;
                var color = Global.ColorMap[s];
                var card = Cards[index];
                card.Color = color;
                PlayCard(card, index);
            };
            ScreenManager.OpenScreen(new WildSelector(onSelectWild));
        }
        private bool SetWildColor(Card card)
        {
            var isColorSet = false;
            Action<string> onSelectWild = s => { 
                if(s == null || !Global.ColorMap.ContainsKey(s)) return;
                var color = Global.ColorMap[s];
                card.Color = color;
                isColorSet = true;
            };
            ScreenManager.OpenScreen(new WildSelector(onSelectWild));
            return isColorSet;
        }

        #endregion
    }
}