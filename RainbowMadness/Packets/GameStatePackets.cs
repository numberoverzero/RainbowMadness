using Engine.DataStructures;
using Engine.Networking.Packets;
using RainbowMadness.Data;

namespace RainbowMadness.Packets
{
    public class GameStateRequestPacket : Packet
    {

    }

    public class GameUpdatePacketTopCard : Packet
    {
        public Card TopCard;

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
}