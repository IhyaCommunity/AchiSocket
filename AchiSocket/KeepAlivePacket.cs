using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleSocket
{
    class KeepAlivePacket : Packet
    {

        public override byte[] GetBytes()
        {
            return new byte[] {PacketType.KeepAlive};
        }

        public override byte[] GetBody()
        {
            return new byte[0];
        }

        public override byte[] GetHeader()
        {
            return new byte[HeaderLength] {PacketType.KeepAlive};
        }
    }
}
