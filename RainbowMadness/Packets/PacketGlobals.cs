using System;
using Engine.DataStructures;
using Engine.Networking.Packets;

namespace RainbowMadness.Packets
{
    public static class PacketGlobals
    {
        private static readonly BidirectionalDict<string, int> Mapping = new BidirectionalDict<string, int>();

        public static void Initialize()
        {
            Packet.GetTypeFunction = TypeFunc;
            Packet.GetNameFunction = NameFunc;
            Packet.BuildPacketFunction = ToPacket;
            Mapping.Add(0000, "NullPacket");
            Mapping.Add(0001, "ChatPacket");
            Mapping.Add(0002, "AuthenticateUserPacket");
            Mapping.Add(0003, "AuthenticateUserResponsePacket");
        }

        public static int TypeFunc(string packetName)
        {
            return Mapping[packetName];
        }

        public static string NameFunc(int type)
        {
            return Mapping[type];
        }

        public static Packet ToPacket(byte[] bytes)
        {
            var reader = new ByteArrayReader(bytes, 0);
            var type = reader.ReadInt32();
            var name = Packet.GetNameFunction(type);
            return (Packet) Activator.CreateInstance(Type.GetType(name));
        }
    }
}