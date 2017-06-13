using System;

namespace Hanabi
{
    public abstract class Card
    {
        public readonly Number Nominal;
        public abstract Color Color {get;}

        protected Card(Number number)
        {
            Nominal = number;
        }

        public static Card CreateCard(Number nominal, Color color)
        {
            switch (color)
            {
                case Color.Blue:
                    return new BlueCard(nominal);

                case Color.Green:
                    return new GreenCard(nominal);

                case Color.Red:
                    return new RedCard(nominal);

                case Color.White:
                    return new WhiteCard(nominal);

                case Color.Yellow:
                    return new YellowCard(nominal);

                case Color.Multicolor:
                    return new MulticolorCard(nominal);

                default:
                    throw new ArgumentException("Unknown color");
            }
        }

        public override String ToString()
        {
            return Color.ToString() + " " + Nominal.ToString();
        }

        public bool EqualsInternal(Card card)
        {
            return this.Color == card.Color && this.Nominal == card.Nominal;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Card)) return false;
            return EqualsInternal((Card) obj);
        }

        public override int GetHashCode()
        {
            return Nominal.GetHashCode() * 27 + Color.GetHashCode() * 13;
        }
    }

    public enum Number
    {
        One = 0,
        Two = 1,
        Three = 2,
        Four = 3,
        Five = 4
    }

    public enum Color
    {
        Blue = 0,
        Green = 1,
        Red = 2,
        Yellow = 3,
        White = 4,
        Multicolor = 5,
    }

    public class YellowCard : Card
    {
        public override Color Color
        {
	        get { return Color.Yellow; }
        }

        public YellowCard(Number number) : base(number)
        {
            
        }
    }

    public class RedCard : Card
    {
        public override Color Color
        {
	        get { return Color.Red; }
        }

        public RedCard(Number number) : base(number)
        {
            
        }
    }

    public class BlueCard : Card
    {
        public override Color Color
        {
            get { return Color.Blue; }
        }

        public BlueCard(Number number) : base(number)
        {
        }
    }

    public class GreenCard : Card 
    {
        public override Color Color
        {
	        get { return Color.Green; }
        }

        public GreenCard(Number number) : base(number)
        {
        }
    }

    public class WhiteCard : Card
    {
        public override Color Color
        {
	        get { return Color.White; }
        }

        public WhiteCard(Number number) : base(number)
        {
        }
    }

    public class MulticolorCard : Card
    {
        public override Color Color
        {
            get { return Color.Multicolor; }
        }

        public MulticolorCard(Number number) : base(number)
        {
        }
    }
}