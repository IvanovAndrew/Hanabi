namespace Hanabi
{
    public class ClueAboutNominalVisitor : IClueVisitor
    {
        private Number? _nominal;

        public Number? Nominal
        {
            get { return _nominal; }
        }

        public bool Visit(IsNominal clue)
        {
            _nominal = clue.Nominal;
            return true;
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
