using Engine.DataStructures;
using Engine.Networking.Packets;

namespace RainbowMadness.Packets
{
    public static class PacketGlobals
    {
        private static readonly BidirectionalDict<string, int> _mapping = new BidirectionalDict<string, int>();

        public static void Initialize()
        {
            Packet.GetTypeFunction = PacketGlobals.Type;
            Packet.GetNameFunction = PacketGlobals.Name;
            _mapping.Add(0000, "NullPacket");
            _mapping.Add(0000, "ChatPacket");
            _mapping.Add(0000, "AuthenticateUserPacket");
            _mapping.Add(0000, "AuthenticateUserResponsePacket");
            
        }

        public static int Type(string packetName)
        {
            return _mapping[packetName];
        }

        public static string Name(int type)
        {
            return _mapping[type];
        }
    }
}