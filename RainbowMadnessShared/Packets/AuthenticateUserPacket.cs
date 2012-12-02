using Engine.DataStructures;
using Engine.Networking.Packets;
using Engine.Utility;

namespace RainbowMadnessShared
{
    public class RequestAuthPacket : Packet
    {
        public override Packet Copy()
        {
            return new RequestAuthPacket();
        }
    }

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

    public class ServerDisconnectPacket : Packet
    {
        public override Packet Copy()
        {
            return new ServerDisconnectPacket();
        }
    }

    public class UserDisconnectPacket : Packet
    {
        public override Packet Copy()
        {
            return new UserDisconnectPacket();
        }
    }
}