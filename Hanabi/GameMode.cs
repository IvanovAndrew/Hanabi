using System;
using System.Diagnostics.Contracts;

namespace Hanabi
{
    public class GameMode
    {
        public const int MinPlayerCount = 2;
        public const int MaxPlayerCount = 5;

        public int PlayersCount
        {
            get;
            private set;
        }

        public bool IsSpecialMode { get; private set; }

        public int ColorsCount
        {
            get { return IsSpecialMode ? 6 : 5; }
        }

        public GameMode(int playersCount, bool isSpecialMode = false)
        {
            Contract.Requires<ArgumentOutOfRangeException>(MinPlayerCount <= playersCount, "Too less players!");
            Contract.Requires<ArgumentOutOfRangeException>(playersCount <= MaxPlayerCount, "Too many players!");

            PlayersCount = playersCount;
            IsSpecialMode = isSpecialMode;
        }
    }
}
