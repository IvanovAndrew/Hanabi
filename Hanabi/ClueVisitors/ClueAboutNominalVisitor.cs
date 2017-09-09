namespace Hanabi
{
    public class ClueAboutNominalVisitor : IClueVisitor
    {
        private Nominal? _nominal;

        public Nominal? Nominal
        {
            get { return _nominal; }
        }

        public bool Visit(ClueAboutNominal clue)
        {
            _nominal = clue.Nominal;
            return true;
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
