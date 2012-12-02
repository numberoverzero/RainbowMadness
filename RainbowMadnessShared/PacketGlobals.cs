using Engine.Networking.Packets;

namespace RainbowMadnessShared
{
    public static class PacketGlobals
    {
        public static void Initialize()
        {
            var builder = new PacketBuilder();
            builder.RegisterPackets(
                new RequestAuthPacket(),
                new AuthenticateUserPacket(),
                new UserDisconnectPacket(),
                new ServerDisconnectPacket(),
                new StartGamePacket(),
                new PlayCardRequestPacket(),
                new GameStatePacket()
                );
            Packet.Builder = builder;
        }
    }
}