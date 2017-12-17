namespace Hanabi
{
    public class ClueAboutColorVisitor : IClueVisitor
    {
        public Color? Color { get; private set; }

        public bool Visit(ClueAboutRank clue)
        {
            return false;
        }

        public bool Visit(ClueAboutNotRank clue)
        {
            return false;
        }

        public bool Visit(ClueAboutColor clue)
        {
            Color = clue.Color;
            return true;
        }

        public bool Visit(ClueAboutNotColor clue)
        {
            return false;
        }
    }
}
