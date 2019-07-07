using log4net;
using System;

namespace Hanabi
{
    class Program
    {
        static void Main(string[] args)
        {
            int n = 1; //100
            //int n = 1000;
            while (n-- > 0)
            {
                Play();
            }
        }

        public static void Play()
        {
            Logger.Init();
            Logger.Log.Info("START!");

            IGameProvider provider = new GameProvider();

            int numOfPlayers = 4;
            Logger.Log.InfoFormat("Number of players: {0}", numOfPlayers);

            var game = new Game(provider, numOfPlayers);

            int score = game.Play();
            Logger.Log.Info("");
            Logger.Log.Info("Score: " + score);
            Console.WriteLine("Score: " + score);
        }
    }
}
