using Engine.Networking.Packets;

namespace RainbowMadness.Packets
{
    public static class PacketGlobals
    {
        public static void Initialize()
        {
            var builder = new PacketBuilder();
            builder.RegisterPackets(
                new ChatPacket(),
                new AuthenticateUserPacket(),
                new AuthenticateUserResponsePacket(),
                new DrawCardRequestPacket(),
                new DrawCardResponsePacket(),
                new PlayerHandRequestPacket(),
                new PlayerHandResponsePacket(),
                new PlayCardRequestPacket(),
                new PlayCardResponsePacket(),
                new GameUpdateRequestPacket(),
                new GameUpdatePacketTopCard(),
                new GameUpdatePacketPlayerHandSize(),
                new GameUpdatePacketPlayerList()
                );
            Packet.Builder = builder;
        }
    }
}