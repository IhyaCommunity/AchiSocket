# AchiSocket

AchiSocket is a fast TCP communication library to make socket communication simple and reliable. It is asynchronous, event-driven and handles reconnection and keep-alive automatically.


## Usage
### Creating a Server
```
var server = new AchaServer(5900);
            server.StartListening(socket =>
            {
                Console.WriteLine("Client connected");
                socket.StartReceive(packet =>
                {
                    Console.WriteLine("Received: " + packet.GetBody().Length);
                    
                });
            });
```
            
### Connecting as a Client

```
            var _client = new AchaClient(ip, port);
            _client.Connect(async socket => 
            {
                if (socket == null)
                {
                    Console.WriteLine("Failed to connect");
                    return;
                }
                await socket.SendAsync(new TextPacket("Hello from client"));
            });
```
