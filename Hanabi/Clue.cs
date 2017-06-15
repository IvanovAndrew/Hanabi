namespace Hanabi
{
    public abstract class Clue
    {
        public abstract Clue Revert();

        public abstract void Accept(IClueVisitor visitor);
    }

    public class IsValue : Clue
    {
        public Number Value { get; private set; }

        public IsValue(Number value)
        {
            Value = value;
        }

        public override Clue Revert()
        {
            return new IsNotValue(Value);
        }

        private bool EqualsCore(IsValue clue)
        {
            return Value == clue.Value;
        }

        public override bool Equals(object obj)
        {
            if (obj is IsValue)
                return EqualsCore((IsValue) obj);
            return false;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode() * GetType().GetHashCode();
        }

        public override void Accept(IClueVisitor visitor)
        {
            visitor.Update(this);
        }
    }

    public class IsNotValue : Clue
    {
        public Number Value { get; private set; }

        public IsNotValue(Number value)
        {
            Value = value;
        }

        public override Clue Revert()
        {
            return new IsValue(Value);
        }

        public override void Accept(IClueVisitor visitor)
        {
            visitor.Update(this);
        }

        private bool EqualsCore(IsNotValue clue)
        {
            return Value == clue.Value;
        }

        public override bool Equals(object obj)
        {
            if (obj is IsNotValue)
                return EqualsCore((IsNotValue) obj);
            return false;
        }


        public override int GetHashCode()
        {
            return Value.GetHashCode() * GetType().GetHashCode();
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

        public override void Accept(IClueVisitor visitor)
        {
            visitor.Update(this);
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
            return Color.GetHashCode() * typeof(IsColor).GetHashCode();
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

        public override void Accept(IClueVisitor visitor)
        {
            visitor.Update(this);
        }

        public override int GetHashCode()
        {
            return Color.GetHashCode() * typeof(IsNotColor).GetHashCode();
        }

        private bool EqualsCore(IsNotColor clue)
        {
            return Color == clue.Color;
        }

        public override bool Equals(object obj)
        {
            if (obj is IsNotColor)
                return EqualsCore((IsNotColor) obj);
            return false;
        }
    }
}
