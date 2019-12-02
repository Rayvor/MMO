using System.Net.Sockets;

namespace Server
{
    public class Client
    {
        public int Id { get; set; }
        public Socket Socket { get; set; }
    }
}
