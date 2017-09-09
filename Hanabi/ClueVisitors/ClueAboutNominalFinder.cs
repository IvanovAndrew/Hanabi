namespace Hanabi
{
    public class ClueAboutNominalFinder : IClueVisitor
    {
        private Nominal _nominal;

        public ClueAboutNominalFinder(Nominal nominal)
        {
            _nominal = nominal;
        }

        public bool Visit(ClueAboutNominal clue)
        {
            return clue.Nominal == _nominal;
        }

        public bool Visit(ClueAboutNotNominal clue)
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
