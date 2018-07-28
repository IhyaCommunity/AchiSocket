using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleSocket
{
    public abstract class Packet
    {
        //byte PacketType { get; }
        //byte[] Bytes { get; set; }

        public abstract byte[] GetBytes();
        public abstract byte[] GetBody();
        public abstract byte[] GetHeader();
        protected const int HeaderLength = 1;

        public static Type CheckType(byte[] bytes)
        {
            switch (bytes[0])
            {
                case PacketType.KeepAlive:
                    return typeof(KeepAlivePacket);
                case PacketType.BinaryPacket:
                    return typeof(BinaryPacket);
                case PacketType.TextPacket:
                    return typeof(TextPacket);
                default:
                    return typeof(BinaryPacket);
            }
        }
    }
}

