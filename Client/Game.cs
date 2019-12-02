using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using System.Net;

namespace Client
{
    public class Game
    {
        private readonly Player _player;
        private bool _playing;
        private Socket _serverSocket;
        private List<Player> players = new List<Player>();

        private static MemoryStream ms = new MemoryStream(new byte[256], 0, 256, true, true);
        private BinaryWriter writer = new BinaryWriter(ms);
        private BinaryReader reader = new BinaryReader(ms);

        public Game(string iPAddress, int port)
        {
            Connect(iPAddress, port);
            _player = CreatePlayer();
        }

        public void Start()
        {
            _playing = true;

            SendInfo(PlayerInfo.Position);

            Task.Run(() => 
            {
                while(_playing)
                {
                    GetInfo();
                }
            });

            while (_playing)
            {
                _player.Draw();

                var key = Console.ReadKey(true).Key;

                _player.Remove();

                switch (key)
                {
                    case ConsoleKey.LeftArrow:
                        _player.MoveLeft();
                        break;
                    case ConsoleKey.RightArrow:
                        _player.MoveRight();
                        break;
                    case ConsoleKey.UpArrow:
                        _player.MoveUp();
                        break;
                    case ConsoleKey.DownArrow:
                        _player.MoveDown();
                        break;
                    case ConsoleKey.Escape:
                        Stop();
                        break;
                }

                SendInfo(PlayerInfo.Position);
            }
        }

        private void Connect(string iPAddress, int port)
        {
            _serverSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);

            Console.WriteLine("Подключение...");
            IPAddress ip = IPAddress.Parse(iPAddress);
            _serverSocket.Connect(ip, port);
            Console.WriteLine("Подключенно.");

            Thread.Sleep(1000);
            Console.Clear();
        }

        private void Stop()
        {
            _player.Reset();
            Console.ResetColor();
            _playing = false;
        }

        private Player CreatePlayer()
        {
            Console.Write("Введите спрайт: ");

            char spr = Convert.ToChar(Console.ReadLine());
            Console.Clear();

            Console.WriteLine("Выберите цвет:");
            for (int i = 0; i < 15; i++)
            {
                Console.ForegroundColor = (ConsoleColor)i;
                Console.WriteLine(i);
            }
            Console.ResetColor();
            ConsoleColor color = (ConsoleColor)int.Parse(Console.ReadLine());
            Console.Clear();

            Random random = new Random();
            int x = random.Next(1, 5);
            int y = random.Next(1, 5);

            Console.WriteLine("Получение индентификатора");
            SendInfo(PlayerInfo.Id);
            int id = GetInfo();
            Console.WriteLine("Получен Id");

            Thread.Sleep(1000);
            Console.Clear();

            return new Player(id, spr, color) { X = x, Y = y };
        }

        private void SendInfo(PlayerInfo playerInfo)
        {
            ms.Position = 0;

            switch (playerInfo)
            {
                case PlayerInfo.Id:
                    writer.Write(0);
                    _serverSocket.Send(ms.GetBuffer());
                    break;
                case PlayerInfo.Position:
                    writer.Write(1);
                    writer.Write(_player.Id);
                    writer.Write(_player.X);
                    writer.Write(_player.Y);
                    writer.Write(_player.Sprite);
                    writer.Write((int)_player.Color);
                    _serverSocket.Send(ms.GetBuffer());
                    break;
                default:
                    break;
            }
        }

        private int GetInfo()
        {
            ms.Position = 0;
            _serverSocket.Receive(ms.GetBuffer());

            int code = reader.ReadInt32();

            switch (code)
            {
                case 0:
                    return reader.ReadInt32();
                case 1:
                    int id = reader.ReadInt32();
                    int x = reader.ReadInt32();
                    int y = reader.ReadInt32();

                    Player player = players.FirstOrDefault(p => p.Id == id);
                    if (player != null)
                    {
                        player.Remove();
                        player.X = x;
                        player.Y = y;
                        player.Draw();
                    }
                    else
                    {
                        char sprite = reader.ReadChar();
                        ConsoleColor color = (ConsoleColor)reader.ReadInt32();
                        Player newPlayer = new Player(id, sprite, color) { X = x, Y = y };
                        players.Add(newPlayer);
                        newPlayer.Draw();
                    }

                    break;
            }

            return -1;
        }
    }
}
