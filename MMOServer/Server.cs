using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    public class Server
    {
        private readonly Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private List<Client> _clients = new List<Client>();
        private int lastId;

        public Server(IPAddress iPAddress, int port)
        {
            _serverSocket.Bind(new IPEndPoint(iPAddress, port));
            _serverSocket.Listen(0);
            _serverSocket.BeginAccept(AcceptCallback, null);
        }

        public string GetListenIpPort()
        {
            return _serverSocket.LocalEndPoint.ToString();
        }

        private void AcceptCallback(IAsyncResult result)
        {
            Client client = new Client
            {
                Id = lastId++,
                Socket = _serverSocket.EndAccept(result)
            };

            Thread thread = new Thread(HandleClient);
            thread.Start(client);
            _clients.Add(client);

            Console.WriteLine($"Пользователь {client.Id} подключился");

            _serverSocket.BeginAccept(AcceptCallback, null);
        }

        private void HandleClient(object o)
        {
            Client client = (Client)o;
            MemoryStream ms = new MemoryStream(new byte[256], 0, 256, true, true);
            BinaryWriter writer = new BinaryWriter(ms);
            BinaryReader reader = new BinaryReader(ms);

            while (true)
            {
                ms.Position = 0;

                try
                {
                    client.Socket.Receive(ms.GetBuffer());
                }
                catch
                {
                    client.Socket.Shutdown(SocketShutdown.Both);
                    client.Socket.Disconnect(true);
                    _clients.Remove(client);
                    Console.WriteLine($"Пользователь {client.Id} отключился");
                    return;
                }

                int code = reader.ReadInt32();

                switch (code)
                {
                    case 0:
                        writer.Write(client.Id);
                        client.Socket.Send(ms.GetBuffer());
                        break;
                    case 1:
                        foreach (var c in _clients)
                        {
                            if (c.Socket != client.Socket)
                            {
                                c.Socket.Send(ms.GetBuffer());
                            }
                        }
                        break;
                }
            }
        }
    }
}
