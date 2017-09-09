using System;

namespace Hanabi
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.InitLogger();
            Logger.Log.Info("START!");

            IGameProvider provider = new GameProvider();

            int numOfPlayers = 3;
            Logger.Log.InfoFormat("Number of players: {0}", numOfPlayers);

            var game = new Game(provider, numOfPlayers);
            

            int score = game.Play();
            Logger.Log.Info("");
            Logger.Log.Info("Score: " + score);
            Logger.Log.InfoFormat("Firework: {0}", game.Board.FireworkPile);
            Console.WriteLine("Score: " + score);
        }
    }
}
