using System;

namespace Hanabi
{
    class Program
    {
        static void Main(string[] args)
        {
            var game = new Game(new GameMode(2));
            int score = game.Play();
            Console.WriteLine("Score: " + score);
        }
    }
}
