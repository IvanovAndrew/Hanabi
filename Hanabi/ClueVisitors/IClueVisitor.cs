namespace Hanabi
{
    public interface IClueVisitor
    {
        bool Visit(IsNominal clue);
        bool Visit(IsNotNominal clue);

        bool Visit(IsColor clue);
        bool Visit(IsNotColor clue);
    }
}
