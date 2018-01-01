namespace Hanabi
{
    public interface IClueVisitor
    {
        bool Visit(ClueAboutRank clueAboutRank);
        bool Visit(ClueAboutNotRank clueAboutNotRank);

        bool Visit(ClueAboutColor clueAboutColor);
        bool Visit(ClueAboutNotColor clueAboutNotColor);
    }
}
