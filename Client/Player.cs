using System;

namespace Client
{
    public class Player
    {
        public Player(int id, char sprite, ConsoleColor color)
        {
            Id = id;
            Sprite = sprite;
            Color = color;
        }

        public int Id { get; private set; }

        public int X { get; set; }

        public int Y { get; set; }

        public char Sprite { get; private set; }

        public ConsoleColor Color { get; private set; }

        public void Draw()
        {
            Console.ForegroundColor = Color;
            Console.SetCursorPosition(X, Y);
            Console.Write(Sprite);
        }

        public void Remove()
        {
            Console.SetCursorPosition(X, Y);
            Console.Write(' ');
        }

        public void Reset()
        {
            Console.SetCursorPosition(0, 0);
        }

        public void MoveLeft()
        {
            if (X > 0)
                X--;
        }

        public void MoveRight()
        {
            if (X < Console.BufferWidth - 1)
                X++;
        }

        public void MoveUp()
        {
            if (Y > 0)
                Y--;
        }

        public void MoveDown()
        {
            if (Y < Console.BufferHeight - 1)
                Y++;
        }
    }
}
