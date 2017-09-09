namespace Hanabi
{
    public class ClueAboutNominalFinder : IClueVisitor
    {
        private Number _nominal;

        public ClueAboutNominalFinder(Number nominal)
        {
            _nominal = nominal;
        }

        public bool Visit(IsNominal clue)
        {
            return clue.Nominal == _nominal;
        }

        public bool Visit(IsNotNominal clue)
        {
            return false;
        }

        public bool Visit(IsColor clue)
        {
            return false;
        }

        public bool Visit(IsNotColor clue)
        {
            return false;
        }
    }
}
