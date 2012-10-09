using Engine.DataStructures;
using Engine.Networking.Packets;
using Engine.Utility;

namespace RainbowMadness.Packets
{
    public class ChatPacket : Packet
    {
        public string Message { get; set; }

        public override byte[] AsByteArray()
        {
            var b = new ByteArrayBuilder();
            b.Add(Type);
            b.Add(Message);
            return b.GetByteArray();
        }

        public override int FromByteArray(byte[] bytes, int startIndex)
        {
            // Type | Message
            var reader = new ByteArrayReader(bytes, startIndex);
            reader.ReadInt32();
            Message = reader.ReadString();
            return reader.Index;
        }
    }
}