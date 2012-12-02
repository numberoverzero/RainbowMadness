using Engine.DataStructures;
using Engine.Networking.Packets;

namespace RainbowMadnessShared
{
    public class PlayCardRequestPacket : Packet
    {
        public int CardIndex;
        public Card Card;

        public override Packet Copy()
        {
            return new PlayCardRequestPacket();
        }

        public override void BuildAsByteArray(ByteArrayBuilder builder)
        {
            base.BuildAsByteArray(builder);
            builder.Add(CardIndex);
            builder.Add(Card);
        }

        protected override int ReadFromByteArray(ByteArrayReader reader)
        {
            base.ReadFromByteArray(reader);
            CardIndex = reader.ReadInt32();
            Card = reader.Read<Card>();
            return reader.Index;
        }
    }
}