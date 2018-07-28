using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleSocket
{
    public class TextPacket : Packet
    {
        public string Text { get; set; }

        public TextPacket(string text)
        {
            Text = text;
        }

        public override byte[] GetBytes()
        {
            var data = Encoding.ASCII.GetBytes(Text);
            var header = GetHeader();
            var array = new byte[header.Length + data.Length];
            Array.Copy(header, array, header.Length);
            Array.Copy(data, 0, array, header.Length, data.Length);
            return array;
        }

        public override byte[] GetBody()
        {
            return Text == null ? new byte[0] : Encoding.ASCII.GetBytes(Text);
        }

        public override byte[] GetHeader()
        {
            return new byte[HeaderLength] {PacketType.TextPacket};
        }

        internal static Packet FromRaw(byte[] bytes)
        {
            var data = new byte[bytes.Length - HeaderLength];
            Array.Copy(bytes, HeaderLength, data, 0, data.Length);
            var text = Encoding.ASCII.GetString(data);
            return new TextPacket(text);
        }
    }
}