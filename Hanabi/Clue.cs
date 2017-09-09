namespace Hanabi
{
    public abstract class Clue
    {
        public abstract Clue Revert();

        public abstract bool Accept(IClueVisitor visitor);
    }

    public class ClueAboutNominal : Clue
    {
        public Nominal Nominal { get; private set; }

        public ClueAboutNominal(Nominal nominal)
        {
            Nominal = nominal;
        }

        public override Clue Revert()
        {
            return new ClueAboutNotNominal(Nominal);
        }

        private bool EqualsCore(ClueAboutNominal clue)
        {
            return Nominal == clue.Nominal;
        }

        public override bool Equals(object obj)
        {
            if (obj is ClueAboutNominal)
                return EqualsCore((ClueAboutNominal) obj);
            return false;
        }

        public override int GetHashCode()
        {
            return Nominal.GetHashCode();
        }

        public override bool Accept(IClueVisitor visitor)
        {
            return visitor.Visit(this);
        }

        public override string ToString()
        {
            return Nominal.ToString();
        }
    }

    public class ClueAboutNotNominal : Clue
    {
        public Nominal Nominal { get; private set; }

        public ClueAboutNotNominal(Nominal nominal)
        {
            Nominal = nominal;
        }

        public override Clue Revert()
        {
            return new ClueAboutNominal(Nominal);
        }

        public override bool Accept(IClueVisitor visitor)
        {
            return visitor.Visit(this);
        }

        private bool EqualsCore(ClueAboutNotNominal clue)
        {
            return Nominal == clue.Nominal;
        }

        public override bool Equals(object obj)
        {
            if (obj is ClueAboutNotNominal)
                return EqualsCore((ClueAboutNotNominal) obj);
            return false;
        }

        public override int GetHashCode()
        {
            return Nominal.GetHashCode();
        }
    }

    public class ClueAboutColor : Clue
    {
        public Color Color { get; private set; }

        public ClueAboutColor(Color color)
        {
            Color = color;
        }

        public override Clue Revert()
        {
 	        return new ClueAboutNotColor(Color);
        }

        public override bool Accept(IClueVisitor visitor)
        {
            return visitor.Visit(this);
        }

        private bool EqualsCore(ClueAboutColor clue)
        {
            return Color == clue.Color;
        }

        public override bool Equals(object obj)
        {
            if (obj is ClueAboutColor)
                return EqualsCore(obj as ClueAboutColor);
            return false;
        }

        public override int GetHashCode()
        {
            return Color.GetHashCode();
        }

        public override string ToString()
        {
            return Color.ToString();
        }
    }

    public class ClueAboutNotColor : Clue
    {
        public Color Color { get; private set; }

        public ClueAboutNotColor(Color color)
        {
            Color = color;
        }

        public override Clue Revert()
        {
            return new ClueAboutColor(Color);
        }

        public override bool Accept(IClueVisitor visitor)
        {
            return visitor.Visit(this);
        }

        public override int GetHashCode()
        {
            return Color.GetHashCode();
        }

        private bool EqualsCore(ClueAboutNotColor clue)
        {
            return Color == clue.Color;
        }

        public override bool Equals(object obj)
        {
            if (obj is ClueAboutNotColor)
                return EqualsCore((ClueAboutNotColor) obj);
            return false;
        }
    }
}
