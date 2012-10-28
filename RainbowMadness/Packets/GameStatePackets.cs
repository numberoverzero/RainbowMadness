using System.Collections.Generic;
using Engine.DataStructures;
using Engine.Networking.Packets;
using RainbowMadness.Data;

namespace RainbowMadness.Packets
{
    public class GameUpdateRequestPacket : Packet
    {
        public override Packet Copy()
        {
            return new GameUpdateRequestPacket();
        }
    }

    public class GameUpdatePacketTopCard : Packet
    {
        public Card TopCard;

        public override Packet Copy()
        {
            return new GameUpdatePacketTopCard();
        }

        public override void BuildAsByteArray(ByteArrayBuilder builder)
        {
            base.BuildAsByteArray(builder);
            builder.Add(TopCard);
        }

        protected override int ReadFromByteArray(ByteArrayReader reader)
        {
            base.ReadFromByteArray(reader);
            TopCard = reader.Read<Card>();
            return reader.Index;
        }
    }

    public class GameUpdatePacketPlayerHandSize : Packet
    {
        public int Count;
        public string PlayerName;

        public override Packet Copy()
        {
            return new GameUpdatePacketPlayerHandSize();
        }

        public override void BuildAsByteArray(ByteArrayBuilder builder)
        {
            base.BuildAsByteArray(builder);
            builder.Add(Count);
            builder.Add(PlayerName);
        }

        protected override int ReadFromByteArray(ByteArrayReader reader)
        {
            base.ReadFromByteArray(reader);
            Count = reader.ReadInt32();
            PlayerName = reader.ReadString();
            return reader.Index;
        }
    }

    public class GameUpdatePacketPlayerList : Packet
    {
        public List<string> PlayerNames;

        public override Packet Copy()
        {
            return new GameUpdatePacketPlayerList();
        }

        public override void BuildAsByteArray(ByteArrayBuilder builder)
        {
            base.BuildAsByteArray(builder);
            builder.AddList(PlayerNames);
        }

        protected override int ReadFromByteArray(ByteArrayReader reader)
        {
            base.ReadFromByteArray(reader);
            PlayerNames = reader.ReadStringList();
            return reader.Index;
        }
    }
}