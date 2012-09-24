using Engine.DataStructures;
using Engine.Networking.Packets;
using Engine.Utility;

namespace RainbowMadness.Packets
{
    public class AuthenticateUserPacket : Packet
    {
        public string Username { get; set; }

        public override byte[] AsByteArray()
        {
            var b = new ByteArrayBuilder();
            b.Add(Type.AsByteArray());
            b.Add(Username.WithTermChar().GetBytesUTF8());
            return b.GetByteArray();
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
            // type(4) | message(n)
            var reader = new ByteArrayReader(bytes, startIndex);
            reader.ReadInt32(); // Type
            Username = reader.ReadString();
            return reader.Index;
        }
    }

    public class AuthenticateUserResponsePacket : Packet
    {
        public string Username { get; set; }
        public bool Success { get; set; }

        public override byte[] AsByteArray()
        {
            var b = new ByteArrayBuilder();
            b.Add(Type.AsByteArray());
            b.Add(Success.AsByteArray());
            b.Add(Username.WithTermChar().GetBytesUTF8());
            return b.GetByteArray();
        }

        public override int FromByteArray(byte[] bytes, int startIndex)
        {
            // type(4) | Success (1) | message(n)

            var reader = new ByteArrayReader(bytes, startIndex);
            reader.ReadInt32(); // Type
            Success = reader.ReadBool();
            Username = reader.ReadString();
            return reader.Index;
        }
    }
}