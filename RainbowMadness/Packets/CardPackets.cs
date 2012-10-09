using System;
using System.Collections.Generic;
using Engine.DataStructures;
using Engine.Networking.Packets;
using RainbowMadness.Data;

namespace RainbowMadness.Packets
{
    public class DrawCardRequestPacket : Packet
    {
        public override byte[] AsByteArray()
        {
            var b = new ByteArrayBuilder();
            b.Add(Type);
            return b.GetByteArray();
        }

        public override int FromByteArray(byte[] bytes, int startIndex)
        {
            // Type
            var reader = new ByteArrayReader(bytes, startIndex);
            reader.ReadInt32();
            return reader.Index;
        }
    }

    public class DrawCardResponsePacket : Packet
    {
        public Card Card;
        public bool IsCardDrawn;
        public string Reason;

        public override byte[] AsByteArray()
        {
            var b = new ByteArrayBuilder();
            b.Add(Type);
            b.Add(Card);
            b.Add(IsCardDrawn);
            b.Add(Reason);
            return b.GetByteArray();
        }

        public override int FromByteArray(byte[] bytes, int startIndex)
        {
            // Type | Card | IsCardDrawn | Reason
            var reader = new ByteArrayReader(bytes, startIndex);
            reader.ReadInt32();
            Card = reader.Read<Card>(); 
            IsCardDrawn = reader.ReadBool();
            Reason = reader.ReadString();
            return reader.Index;
        }
    }

    public class TopCardRequestPacket : Packet
    {
        public override byte[] AsByteArray()
        {
            var b = new ByteArrayBuilder();
            b.Add(Type);
            return b.GetByteArray();
        }

        public override int FromByteArray(byte[] bytes, int startIndex)
        {
            // Type
            var reader = new ByteArrayReader(bytes, startIndex);
            reader.ReadInt32();
            return reader.Index;
        }
    }

    public class TopCardResponsePacket : Packet
    {
        public Card Card;

        public override byte[] AsByteArray()
        {
            var b = new ByteArrayBuilder();
            b.Add(Type);
            b.Add(Card);
            return b.GetByteArray();
        }

        public override int FromByteArray(byte[] bytes, int startIndex)
        {
            // Type | Card
            var reader = new ByteArrayReader(bytes, startIndex);
            reader.ReadInt32();
            Card = reader.Read<Card>();
            return reader.Index;
        }
    }

    public class PlayerHandRequestPacket : Packet
    {
        public string PlayerName;

        public override byte[] AsByteArray()
        {
            var b = new ByteArrayBuilder();
            b.Add(Type);
            b.Add(PlayerName);
            return b.GetByteArray();
        }

        public override int FromByteArray(byte[] bytes, int startIndex)
        {
            // Type
            var reader = new ByteArrayReader(bytes, startIndex);
            reader.ReadInt32();
            PlayerName = reader.ReadString();
            return reader.Index;
        }
    }

    public class PlayerHandResponsePacket : Packet
    {
        public bool IsValidRequest;
        public string PlayerName;
        public List<Card> Cards;

        public override byte[] AsByteArray()
        {
            var b = new ByteArrayBuilder();
            b.Add(IsValidRequest);
            b.Add(PlayerName);
            b.Add(Cards);
            return b.GetByteArray();
        }

        public override int FromByteArray(byte[] bytes, int startIndex)
        {
            // Type | IsValid | Name | Cards
            var reader = new ByteArrayReader(bytes, startIndex);
            reader.ReadInt32();
            IsValidRequest = reader.ReadBool();
            PlayerName = reader.ReadString();
            Cards = reader.ReadList<Card>();
            return reader.Index;
        }
    }
}