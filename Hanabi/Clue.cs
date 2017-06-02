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
    }
}
