namespace Hanabi
{
    public class ClueAboutRankVisitor : IClueVisitor
    {
        public Rank? Rank
        {
            get;
            private set;
        }

        public bool Visit(ClueAboutRank clue)
        {
            Rank = clue.Rank;
            return true;
        }

        public bool Visit(ClueAboutNotRank clue)
        {
            return false;
        }

        public bool Visit(ClueAboutColor clue)
        {
            return false;
        }

        public bool Visit(ClueAboutNotColor clue)
        {
            return false;
        }
    }
}
