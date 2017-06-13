namespace Hanabi
{
    public abstract class Clue
    {
        public abstract void UpdateGuess(Guess guess);

        public abstract Clue Revert();
    }

    public class IsValue : Clue
    {
        private readonly Number _value;

        public IsValue(Number value)
        {
            _value = value;
        }

        public override void UpdateGuess(Guess guess)
        {
            guess.NumberIs(_value);
        }

        public override Clue Revert()
        {
            return new IsNotValue(_value);
        }

        private bool EqualsInternal(IsValue clue)
        {
            return _value == clue._value;
        }

        public override bool Equals(object obj)
        {
            if (obj is IsValue)
                return EqualsInternal((IsValue) obj);
            return false;
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode() * GetType().GetHashCode();
        }
    }

    public class IsNotValue : Clue
    {
        private readonly Number _value;

        public IsNotValue(Number value)
        {
            _value = value;
        }

        public override void UpdateGuess(Guess guess)
        {
            guess.NumberIsNot(_value);
        }

        public override Clue Revert()
        {
            return new IsValue(_value);
        }

        private bool EqualsInternal(IsNotValue clue)
        {
            return _value == clue._value;
        }

        public override bool Equals(object obj)
        {
            if (obj is IsNotValue)
                return EqualsInternal((IsNotValue) obj);
            return false;
        }


        public override int GetHashCode()
        {
            return _value.GetHashCode() * GetType().GetHashCode();
        }
    }

    public class IsColor : Clue
    {
        private readonly Color _color;

        public IsColor(Color color)
        {
            _color = color;
        }

        public override void UpdateGuess(Guess guess)
        {
            guess.ColorIs(_color);
        }

        public override Clue Revert()
        {
 	        return new IsNotColor(_color);
        }

        private bool EqualsInternal(IsColor clue)
        {
            return _color == clue._color;
        }

        public override bool Equals(object obj)
        {
            if (obj is IsColor)
                return EqualsInternal(obj as IsColor);
            return false;
        }

        public override int GetHashCode()
        {
            return _color.GetHashCode() * typeof(IsColor).GetHashCode();
        }
    }

    public class IsNotColor : Clue
    {
        private readonly Color _color;

        public IsNotColor(Color color)
        {
            _color = color;
        }

        public override void UpdateGuess(Guess guess)
        {
            guess.ColorIsNot(_color);
        }

        public override Clue Revert()
        {
            return new IsColor(_color);
        }

        public override int GetHashCode()
        {
            return _color.GetHashCode() * typeof(IsNotColor).GetHashCode();
        }

        private bool EqualsInternal(IsNotColor clue)
        {
            return _color == clue._color;
        }

        public override bool Equals(object obj)
        {
            if (obj is IsNotColor)
                return EqualsInternal(obj as IsNotColor);
            return false;
        }
    }
}
