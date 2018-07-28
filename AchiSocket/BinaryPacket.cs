using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SimpleSocket
{
    public class BinaryPacket : Packet
    {
        public const byte PacketType = 1;

        private byte[] _data;

        public BinaryPacket(byte[] data)
        {
            _data = data;
        }

        public override byte[] GetBytes()
        {
            var header = GetHeader();
            var array = new byte[header.Length + _data.Length];
            Array.Copy(header, array, header.Length);
            Array.Copy(_data, 0, array, header.Length, _data.Length);
            return array;
        }

        public override byte[] GetBody()
        {
            return _data;
        }

        public override byte[] GetHeader()
        {
            return new byte[HeaderLength] { PacketType};
        }

        public static BinaryPacket FromRaw(byte[] bytes)
        {
            var data = new byte[bytes.Length - HeaderLength];
            Array.Copy(bytes, HeaderLength, data, 0, data.Length);
            return new BinaryPacket(data);
        }
    }
}