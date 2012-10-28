using System;
using System.Collections.Generic;
using Engine.DataStructures;
using Engine.Networking.Packets;
using RainbowMadness.Data;

namespace RainbowMadness.Packets
{
    public class DrawCardRequestPacket : Packet
    {
        public override Packet Copy()
        {
            return new DrawCardRequestPacket();
        }
    }

    public class DrawCardResponsePacket : Packet
    {
        public Card Card;
        public bool IsCardDrawn;
        public string Reason;

        public override Packet Copy()
        {
            return new DrawCardResponsePacket();
        }

        public override void BuildAsByteArray(ByteArrayBuilder builder)
        {
            base.BuildAsByteArray(builder);
            builder.Add(Card);
            builder.Add(IsCardDrawn);
            builder.Add(Reason);
        }

        protected override int ReadFromByteArray(ByteArrayReader reader)
        {
            base.ReadFromByteArray(reader);
            Card = reader.Read<Card>();
            IsCardDrawn = reader.ReadBool();
            Reason = reader.ReadString();
            return reader.Index;
        }
    }

    public class PlayerHandRequestPacket : Packet
    {
        public string PlayerName;

        public override Packet Copy()
        {
            return new PlayerHandRequestPacket();
        }

        public override void BuildAsByteArray(ByteArrayBuilder builder)
        {
            base.BuildAsByteArray(builder);
            builder.Add(PlayerName);
        }

        protected override int ReadFromByteArray(ByteArrayReader reader)
        {
            base.ReadFromByteArray(reader);
            PlayerName = reader.ReadString();
            return reader.Index;
        }
    }

    public class PlayerHandResponsePacket : Packet
    {
        public bool IsValidRequest;
        public string PlayerName;
        public List<Card> Cards;

        public override Packet Copy()
        {
            return new PlayerHandResponsePacket();
        }

        public override void BuildAsByteArray(ByteArrayBuilder builder)
        {
            base.BuildAsByteArray(builder);
            builder.Add(IsValidRequest);
            builder.Add(PlayerName);
            builder.Add(Cards);
        }

        protected override int ReadFromByteArray(ByteArrayReader reader)
        {
            base.ReadFromByteArray(reader);
            IsValidRequest = reader.ReadBool();
            PlayerName = reader.ReadString();
            Cards = reader.ReadList<Card>();
            return reader.Index;
        }
    }

    public class PlayCardRequestPacket : Packet
    {
        public Card Card;

        public override Packet Copy()
        {
            return new PlayCardRequestPacket();
        }

        public override void BuildAsByteArray(ByteArrayBuilder builder)
        {
            base.BuildAsByteArray(builder);
            builder.Add(Card);
        }

        protected override int ReadFromByteArray(ByteArrayReader reader)
        {
            base.ReadFromByteArray(reader);
            Card = reader.Read<Card>();
            return reader.Index;
        }
    }

    public class PlayCardResponsePacket : Packet
    {
        public bool IsPlayed;
        public String Message;

        public override Packet Copy()
        {
            return new PlayCardResponsePacket();
        }

        public override void BuildAsByteArray(ByteArrayBuilder builder)
        {
            base.BuildAsByteArray(builder);
            builder.Add(IsPlayed);
            builder.Add(Message);
        }

        protected override int ReadFromByteArray(ByteArrayReader reader)
        {
            base.ReadFromByteArray(reader);
            IsPlayed = reader.ReadBool();
            Message = reader.ReadString();
            return reader.Index;
        }
    }
}