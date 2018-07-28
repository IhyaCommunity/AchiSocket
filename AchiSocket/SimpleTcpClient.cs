using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSocket
{
    class SimpleTcpClient
    {
        public bool AutoReconnect { get; set; }
        public bool IsConnected { get; set; }

        private TcpClient _client;

        public SimpleTcpClient()
        {
            _client = new TcpClient();
        }


        public async Task<TResult> ConnectAsync(string ip, int port, bool autoReconnect)
        {
            try
            {
                await _client.ConnectAsync(ip, port);
                IsConnected = true;
                AutoReconnect = autoReconnect;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new TResult(e);
            }
            return new TResult();
        }

        public async Task<TResult> SendDataAsync(byte[] data)
        {
            try
            {
                var stream = _client.GetStream();
                await stream.WriteAsync(data, 0, data.Length);
            }
            catch (Exception e)
            {
                return new TResult(e);
            }
            return new TResult();
        }


        //public async Task<TResult> ReadPacketAsync()
        //{
            
        //}




    }
}