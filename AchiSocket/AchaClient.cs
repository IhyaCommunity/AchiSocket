using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SimpleSocket
{
    public class AchaClient : IDisposable
    {
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public int ReconnectInterval { get; set; } = 3000;

        private TcpClient _client;
        private Timer _reconnectTimer;
        private AchiSocket _socket;

        public AchaClient(string host, int port)
        {
            _client = new TcpClient();
            IpAddress = host;
            Port = port;
        }

        public void Connect(Action<AchiSocket> onResult=null)
        {
            try
            {
                _client.BeginConnect(IpAddress, Port, ar =>
                {

                    var client = (TcpClient) ar.AsyncState;
                    try
                    {
                        client.EndConnect(ar);
                        _socket = new AchiSocket(client);
                        _socket.Disconnected += OnDisconnected;
                        onResult?.Invoke(_socket);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.StackTrace);
                        onResult?.Invoke(null);
                    }
                }, _client);
                
            }
            catch (Exception e)
            { 
                Console.WriteLine(e.Message);
                onResult?.Invoke(null);
            }
        }


        private void OnDisconnected()
        {
            _reconnectTimer = new Timer(o =>
            {
                Console.WriteLine("Tring reconnect");
                Connect(s =>
                {
                    if (s != null) _reconnectTimer?.Dispose();
                });
            }, null, 0, ReconnectInterval);
        }

        public void Dispose()
        {
            _client?.Dispose();
            _reconnectTimer?.Dispose();
            _socket?.Dispose();
        }
    }
}
