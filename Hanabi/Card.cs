using System;

namespace Hanabi
{
    public class Card
    {
        public Rank Rank { get; }
        public Color Color {get; }

        public Card(Rank rank, Color color) : this(color, rank)
        {
            
        }

        public Card(Color color, Rank rank)
        {
            Rank = rank;
            Color = color;
        }

        public override string ToString()
        {
            return $"{Color} {Rank}";
        }

        private bool EqualsCore(Card card)
        {
            return Color == card.Color && Rank == card.Rank;
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
                return ((int) Rank * 397) ^ (int) Color;
            }
        }


        public static Card GetPreviousCardInFirework(Card card)
        {
            if (card == null)
                throw new ArgumentNullException(nameof(card));

            Rank? previousRank = card.Rank.GetPreviousNumber();

            return previousRank != null ? new Card(previousRank.Value, card.Color) : null;
        }

        public static bool operator ==(Card first, Card second)
        {
            if (Object.Equals(first, null)) return Object.Equals(second, null);

            return first.Equals(second);
        }

        public static bool operator !=(Card first, Card second)
        {
            return !(first == second);
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

    public enum Rank
    {
        One = 0,
        Two = 1,
        Three = 2,
        Four = 3,
        Five = 4
    }
}