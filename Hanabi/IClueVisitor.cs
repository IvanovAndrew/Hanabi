namespace Hanabi
{
    public interface IClueVisitor
    {
        void Update(IsValue clue);
        void Update(IsNotValue clue);

        void Update(IsColor clue);
        void Update(IsNotColor clue);
    }
}
