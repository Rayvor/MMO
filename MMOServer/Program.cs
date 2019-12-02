using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Linq;

namespace Server
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.Title = "Server";

            Server server = new Server(IPAddress.Any, 2048);

            Console.WriteLine(server.GetListenIpPort());

            Console.ReadLine();
        }
    }
}
