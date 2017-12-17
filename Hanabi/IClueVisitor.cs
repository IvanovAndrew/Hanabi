namespace Hanabi
{
    public interface IClueVisitor
    {
        bool Visit(ClueAboutRank clue);
        bool Visit(ClueAboutNotRank clue);

        bool Visit(ClueAboutColor clue);
        bool Visit(ClueAboutNotColor clue);
    }
}
