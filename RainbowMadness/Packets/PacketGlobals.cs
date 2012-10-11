using System;
using Engine.DataStructures;
using Engine.Networking.Packets;

namespace RainbowMadness.Packets
{
    public static class PacketGlobals
    {
        private static readonly BidirectionalDict<string, int> Mapping = new BidirectionalDict<string, int>();

        private static int _nextType;

        public static void Initialize()
        {
            Packet.GetTypeFunction = TypeFunc;
            Packet.GetNameFunction = NameFunc;
            Packet.BuildPacketFunction = ToPacket;
            AddPacket("NullPacket");
            AddPacket("ChatPacket");
            AddPacket("AuthenticateUserPacket");
            AddPacket("AuthenticateUserResponsePacket");
        }

        private static void AddPacket(string packetName)
        {
            Mapping.Add(_nextType, packetName);
            _nextType++;
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
            var typeInt = reader.ReadInt32();
            var name = Packet.GetNameFunction(typeInt);
            var type = Type.GetType(name);
            if (type == null) return Packet.EmptyPacket;
            var packet = (Packet) Activator.CreateInstance(type);
            packet.FromByteArray(bytes, 0);
            return packet;
        }
    }
}