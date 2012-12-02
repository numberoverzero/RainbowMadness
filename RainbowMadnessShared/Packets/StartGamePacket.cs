using System;
using Engine.Networking.Packets;

namespace RainbowMadnessShared
{
    public class StartGamePacket : Packet
    {
        public override Packet Copy()
        {
            return new StartGamePacket();
        }
    }
}
