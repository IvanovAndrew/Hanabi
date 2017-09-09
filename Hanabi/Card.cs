using System;
using System.Diagnostics.Contracts;

namespace Hanabi
{
    public class Card
    {
        public Nominal Nominal { get; private set; }
        public Color Color {get; private set; }

        public Card(Nominal nominal, Color color) : this(color, nominal)
        {
            
        }

        public Card(Color color, Nominal nominal)
        {
            Nominal = nominal;
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

            Nominal? nextNominal = card.Nominal.GetNextNumber();

            return nextNominal != null ? new Card(nextNominal.Value, card.Color) : null;
        }

        
        public static Card GetCardInFireworkBefore(Card card)
        {
            Contract.Requires<ArgumentNullException>(card != null);

            Nominal? previousNominal = card.Nominal.GetPreviousNumber();

            return previousNominal != null ? new Card(previousNominal.Value, card.Color) : null;
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