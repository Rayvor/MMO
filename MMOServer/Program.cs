using System;
using System.Net;

namespace Server
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.Title = "Server";

            Server server = new Server(IPAddress.Parse("127.0.0.1"), 2048)
            {
                ConnectedCallback = (cliendId) => Console.WriteLine($"Пользователь {cliendId} подключился"),
                DisconnectedCallback = (cliendId) => Console.WriteLine($"Пользователь {cliendId} отключился")
            };

            server.Start();

            Console.WriteLine("Сервер запущен {0}", server.GetListenIpPort());

            Console.ReadLine();
        }
    }
}
