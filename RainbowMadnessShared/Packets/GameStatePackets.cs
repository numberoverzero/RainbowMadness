using System.Collections.Generic;
using System.Linq;
using Engine.DataStructures;
using Engine.Networking.Packets;

namespace RainbowMadnessShared
{
    public class GameStatePacket : Packet
    {
        public bool IsGameStarted;
        public int DeckSize;
        public int PlayerIndex;
        public bool Reverse;
        public Card Top;
        public string Winner;
        public List<string> Players;
        public List<int> PlayerHandSizes;
        public List<Card> Cards;
        
        public override Packet Copy()
        {
            return new GameStatePacket();
        }

        public override void BuildAsByteArray(ByteArrayBuilder builder)
        {
            base.BuildAsByteArray(builder);
            
            builder.Add(IsGameStarted);
            builder.Add(DeckSize);
            builder.Add(PlayerIndex);
            builder.Add(Reverse);
            builder.Add(Top);
            builder.Add(Winner);
            
            builder.AddList(Players);
            builder.AddList(PlayerHandSizes);
            builder.Add(Cards);
        }

        protected override int ReadFromByteArray(ByteArrayReader reader)
        {
            base.ReadFromByteArray(reader);

            IsGameStarted = reader.ReadBool();
            DeckSize = reader.ReadInt32();
            PlayerIndex = reader.ReadInt32();
            Reverse = reader.ReadBool();
            Top = reader.Read<Card>();
            Winner = reader.ReadString();

            Players = reader.ReadStringList();
            PlayerHandSizes = reader.ReadIntList();
            Cards = reader.ReadList<Card>();
            
            return reader.Index;
        }

        public static GameStatePacket FromGame(Game game, string player)
        {
            return new GameStatePacket
            {
                Cards = game.PlayersCards[player],
                IsGameStarted =  game.IsGameStarted,
                DeckSize = game.Deck.InstanceCount,
                PlayerHandSizes = game.Players.Select(p => game.PlayersCards[p].Count).ToList(),
                PlayerIndex = game.PlayerIndex,
                Players = game.Players,
                Top = game.Top,
                Winner = game.Winner,
                Reverse = game.Reverse
            };
        }
    }
}