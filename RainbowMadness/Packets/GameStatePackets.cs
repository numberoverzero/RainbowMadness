using Engine.DataStructures;
using Engine.Networking.Packets;

namespace RainbowMadness.Packets
{
    public class GameStateRequestPacket : Packet
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
}