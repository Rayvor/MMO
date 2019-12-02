using System;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Client";
            Console.CursorVisible = false;

            Game game = new Game();
            game.Start();
        }
    }
}
