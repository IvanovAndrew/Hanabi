using System;
using System.Diagnostics.Contracts;

namespace Hanabi
{
    public class Card
    {
        public Number Nominal { get; private set; }
        public Color Color {get; private set; }

        public Card(Number number, Color color) : this(color, number)
        {
            
        }

        public Card(Color color, Number number)
        {
            Nominal = number;
            Color = color;
        }

        public override String ToString()
        {
            return String.Format("{0} {1}", Color, Nominal);
        }

        private bool EqualsCore(Card card)
        {
            return Color == card.Color && Nominal == card.Nominal;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Card)) return false;
            return EqualsCore((Card) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) Nominal * 397) ^ (int) Color;
            }
        }


        public static Card GetCardInFireworkAfter(Card card)
        {
            Contract.Requires<ArgumentNullException>(card != null);

            Number? nextNumber = card.Nominal.GetNextNumber();

            return nextNumber != null ? new Card(nextNumber.Value, card.Color) : null;
        }

        
        public static Card GetCardInFireworkBefore(Card card)
        {
            Contract.Requires<ArgumentNullException>(card != null);

            Number? previousNumber = card.Nominal.GetPreviousNumber();

            return previousNumber != null ? new Card(previousNumber.Value, card.Color) : null;
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

    public static class NumberExtension
    {
        public static Number? GetNextNumber(this Number number)
        {
            if (number == Number.Five) return null;

            return (Number) ((int) number + 1);
        }

        public static Number? GetPreviousNumber(this Number number)
        {
            if (number == Number.One) return null;

            return (Number) ((int) number - 1);
        }
    }

    public enum Color
    {
        Blue,
        Green,
        Red,
        Yellow,
        White,
        Multicolor,
    }
}