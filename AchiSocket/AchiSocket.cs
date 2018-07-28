using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleSocket
{
    public class AchiSocket : IDisposable
    {
        public bool IsConnected => _client.Connected;
        public Action Disconnected;

        private TcpClient _client;
        private int _sendTimeout = 8000;
        private Action<Packet> OnPacketReceived;
        private bool _busy = false;
        private Timer _keepAliveTimer;
        private int _keepAliveDuration;
        private Thread _receiveThread;


        public AchiSocket(TcpClient socket)
        {
            _client = socket;
            _keepAliveTimer = new Timer(SendKeepAlive, null, _keepAliveDuration, _keepAliveDuration);
        }


        public void StartReceive(Action<Packet> onReceived)
        {
            _receiveThread = new Thread(async () =>
            {
                try
                {
                    var stream = _client.GetStream();
                    while (true)
                    {
                        try
                        {
                            if (_client.Available > 0)
                            {
                                var bytes = new byte[sizeof(int)];
                                var rdLen = await stream.ReadAsync(bytes, 0, bytes.Length);
                                if (rdLen != bytes.Length) continue;
                                var len = BitConverter.ToInt32(bytes, 0);
                                bytes = new byte[len];
                                int ptr = 0;
                                while (ptr < len)
                                {
                                    rdLen = await stream.ReadAsync(bytes, ptr, len);
                                    ptr += rdLen;
                                }
                                Type type = Packet.CheckType(bytes);
                                if (type == typeof(BinaryPacket))
                                    onReceived?.Invoke(BinaryPacket.FromRaw(bytes));
                                if (type == typeof(TextPacket))
                                    onReceived?.Invoke(TextPacket.FromRaw(bytes));
                            }
                            Thread.Sleep(5);
                        }
                        catch (Exception e)
                        {
                            Console.Write(e.Message);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            });
            _receiveThread.Start();

        }

        private async void SendKeepAlive(object state)
        {
            if (_busy) return;
            if (!await SendAsync(new KeepAlivePacket()))
            {
                _keepAliveTimer.Dispose();
                Disconnected?.Invoke();
            }
        }


        //private void Send(Packet packet, Action<bool> onResult = null)
        //{
        //    try
        //    {
        //        _busy = true;
        //        var data = packet.GetBytes();
        //        var stream = _client.GetStream();
        //        stream.Write(BitConverter.GetBytes(data.Length), 0, sizeof(int));
        //        stream.BeginWrite(data, 0, data.Length, ar =>
        //        {
        //            var strm = (NetworkStream) ar.AsyncState;
        //            strm.EndWrite(ar);
        //            _busy = false;
        //            onResult?.Invoke(true);
        //        }, stream);
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.StackTrace);
        //        _busy = false;
        //        onResult?.Invoke(false);
        //    }
        //}


        public async Task<bool> SendAsync(Packet packet)
        {
            try
            {
                _busy = true;
                var data = packet.GetBytes();
                var stream = _client.GetStream();
                stream.Write(BitConverter.GetBytes(data.Length), 0, sizeof(int));
                await stream.WriteAsync(data, 0, data.Length);
                _busy = false;
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                _busy = false;
                return false;
            }
        }

        public void Dispose()
        {
            try
            {
                _client.Dispose();
                _receiveThread?.Abort();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

        }
    }
}