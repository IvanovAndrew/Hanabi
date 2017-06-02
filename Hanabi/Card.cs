using System;

namespace Hanabi
{
    public abstract class Card
    {
        public Number Nominal;
        public abstract Color Color {get;}

        public override String ToString()
        {
            return Color.ToString() + " " + Nominal.ToString();
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
    }

    public class RedCard : Card
    {
        public override Color Color
        {
	        get { return Color.Red; }
        }
    }

    public class BlueCard : Card
    {
        public override Color Color
        {
            get { return Color.Blue; }
        }
    }

    public class GreenCard : Card 
    {
        public override Color Color
        {
	        get { return Color.Green; }
        }
    }

    public class WhiteCard : Card
    {
        public override Color Color
        {
	        get { return Color.White; }
        }
    }

    public class MulticolorCard : Card
    {
        public override Color Color
        {
            get { return Color.Multicolor; }
        }
    }
}