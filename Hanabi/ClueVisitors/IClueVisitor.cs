namespace Hanabi
{
    public interface IClueVisitor
    {
        bool Visit(ClueAboutNominal clue);
        bool Visit(ClueAboutNotNominal clue);

        bool Visit(ClueAboutColor clue);
        bool Visit(ClueAboutNotColor clue);
    }
}
