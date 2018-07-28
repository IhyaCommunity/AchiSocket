using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleSocket
{
    class PacketType
    {
        public const byte BinaryPacket = 1;
        public const byte TextPacket = 2;
        public const byte KeepAlive = 0;
    }
}
