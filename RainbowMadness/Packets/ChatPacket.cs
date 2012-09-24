using System.Text;
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
            b.Add(Type.AsByteArray());
            b.Add(Message.WithTermChar().GetBytesUTF8());
            return b.GetByteArray().WithSizePrefix();
        }

        /// <summary>
        /// <para>
        /// Returns the position of the last character of the object in the byte array.
        /// </para>
        /// <para>
        /// Returns a number less than startIndex if the object does not start at the given index.
        /// </para>
        /// </summary>
        /// <param name="bytes"/><param name="startIndex"/>
        /// <returns/>
        public override int FromByteArray(byte[] bytes, int startIndex)
        {
            // size(4) | type(4) | message(n)
            var reader = new ByteArrayReader(bytes, startIndex);
            reader.ReadInt32(); // Size
            reader.ReadInt32(); // Type
            Message = reader.ReadString();
            return reader.Index;
        }
    }
}