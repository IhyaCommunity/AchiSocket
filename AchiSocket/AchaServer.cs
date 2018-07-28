using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleSocket
{
    public class AchaServer
    {

        public int Port { get; set; }
        public Action<AchiSocket> OnConnected;
        private TcpListener _listener = null;
        private Thread _listenThread;
        public ManualResetEvent allDone = new ManualResetEvent(false);

        public AchaServer(int port)
        {
            Port = port;
            _listener = new TcpListener(IPAddress.Any, port);
        }

        public void StartListening(Action<AchiSocket> onConnected = null)
        {
            _listenThread = new Thread(() =>
            {
                Start(onConnected);
            });
            _listenThread.Start();
        }

        public void Start(Action<AchiSocket> onConnected = null)
        {
            try
            {
                _listener.Start();
                while (true)
                {
                    allDone.Reset();
                    _listener.BeginAcceptTcpClient(ar =>
                    {
                        allDone.Set();
                        var listener = (TcpListener) ar.AsyncState;
                        var socket = listener.EndAcceptTcpClient(ar);
                        if (onConnected != null) OnConnected += onConnected;
                        OnConnected?.Invoke(new AchiSocket(socket));

                    }, _listener);

                    allDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            //_listenThread = new Thread(async () =>
            //{
            //    while (true)
            //    {
            //        try
            //        {
            //            var client = await _listener.AcceptTcpClientAsync();

            //            if (onConnected != null)
            //                OnConnected += onConnected;
            //            OnConnected?.Invoke(new AchiSocket(client));

            //        }
            //        catch (Exception e)
            //        {
            //            if (e is ThreadAbortException)
            //                return;
            //            Console.WriteLine("------------- Listening thread Exception: " + e.Message);
            //        }
            //    }
            //});
            //_listenThread.Start();
        }


        public void Stop()
        {
            try
            {
                _listener.Stop();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            try
            {
                _listenThread?.Abort();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

    }
}
