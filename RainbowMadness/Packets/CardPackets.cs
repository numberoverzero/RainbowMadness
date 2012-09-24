using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Networking.Packets;

namespace RainbowMadness.Packets
{
    public class RequestCardPacket : Packet
    {
        public override byte[] AsByteArray()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }

    
}
