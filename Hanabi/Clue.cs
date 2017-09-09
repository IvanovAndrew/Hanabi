namespace Hanabi
{
    public abstract class Clue
    {
        public abstract Clue Revert();

        public abstract bool Accept(IClueVisitor visitor);

        public abstract bool IsConcreteClue();
    }

    public class IsNominal : Clue
    {
        public Number Nominal { get; private set; }

        public IsNominal(Number nominal)
        {
            Nominal = nominal;
        }

        public override Clue Revert()
        {
            return new IsNotNominal(Nominal);
        }

        private bool EqualsCore(IsNominal clue)
        {
            return Nominal == clue.Nominal;
        }

        public override bool Equals(object obj)
        {
            if (obj is IsNominal)
                return EqualsCore((IsNominal) obj);
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

        public override bool IsConcreteClue()
        {
            return true;
        }

        public override string ToString()
        {
            return Nominal.ToString();
        }
    }

    public class IsNotNominal : Clue
    {
        public Number Nominal { get; private set; }

        public IsNotNominal(Number nominal)
        {
            Nominal = nominal;
        }

        public override Clue Revert()
        {
            return new IsNominal(Nominal);
        }

        public override bool Accept(IClueVisitor visitor)
        {
            return visitor.Visit(this);
        }

        private bool EqualsCore(IsNotNominal clue)
        {
            return Nominal == clue.Nominal;
        }

        public override bool Equals(object obj)
        {
            if (obj is IsNotNominal)
                return EqualsCore((IsNotNominal) obj);
            return false;
        }

        public override bool IsConcreteClue()
        {
            return false;
        }

        public override int GetHashCode()
        {
            return Nominal.GetHashCode();
        }
    }

    public class IsColor : Clue
    {
        public Color Color { get; private set; }

        public IsColor(Color color)
        {
            Color = color;
        }

        public override Clue Revert()
        {
 	        return new IsNotColor(Color);
        }

        public override bool Accept(IClueVisitor visitor)
        {
            return visitor.Visit(this);
        }

        private bool EqualsCore(IsColor clue)
        {
            return Color == clue.Color;
        }

        public override bool Equals(object obj)
        {
            if (obj is IsColor)
                return EqualsCore(obj as IsColor);
            return false;
        }

        public override int GetHashCode()
        {
            return Color.GetHashCode();
        }

        public override bool IsConcreteClue()
        {
            return true;
        }

        public override string ToString()
        {
            return Color.ToString();
        }
    }

    public class IsNotColor : Clue
    {
        public Color Color { get; private set; }

        public IsNotColor(Color color)
        {
            Color = color;
        }

        public override Clue Revert()
        {
            return new IsColor(Color);
        }

        public override bool Accept(IClueVisitor visitor)
        {
            return visitor.Visit(this);
        }

        public override int GetHashCode()
        {
            return Color.GetHashCode();
        }

        private bool EqualsCore(IsNotColor clue)
        {
            return Color == clue.Color;
        }

        public override bool IsConcreteClue()
        {
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj is IsNotColor)
                return EqualsCore((IsNotColor) obj);
            return false;
        }
    }
}
