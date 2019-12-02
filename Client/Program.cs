using System;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Client";
            Console.CursorVisible = false;

            Game game = new Game("127.0.0.1", 2048);
            game.Start();
        }
    }
}
