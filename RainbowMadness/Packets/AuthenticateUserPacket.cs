using Engine.DataStructures;
using Engine.Networking.Packets;
using Engine.Utility;

namespace RainbowMadness.Packets
{
    public class AuthenticateUserPacket : Packet
    {
        public string Username { get; set; }

        public override Packet Copy()
        {
            return new AuthenticateUserPacket();
        }

        public override void BuildAsByteArray(ByteArrayBuilder builder)
        {
            base.BuildAsByteArray(builder);
            builder.Add(Username);
        }

        protected override int ReadFromByteArray(ByteArrayReader reader)
        {
            base.ReadFromByteArray(reader);
            Username = reader.ReadString();
            return reader.Index;
        }
        
    }

    public class AuthenticateUserResponsePacket : Packet
    {
        public string Username { get; set; }
        public bool Success { get; set; }

        public override Packet Copy()
        {
            return new AuthenticateUserResponsePacket();
        }

        public override void BuildAsByteArray(ByteArrayBuilder builder)
        {
            base.BuildAsByteArray(builder);
            builder.Add(Username);
            builder.Add(Success);
        }

        protected override int ReadFromByteArray(ByteArrayReader reader)
        {
            base.ReadFromByteArray(reader);
            Username = reader.ReadString();
            Success = reader.ReadBool();
            return reader.Index;
        }
        
    }
}