using System.Collections.Generic;
using System.Linq;

namespace Hanabi
{
    public abstract class ClueType
    {
        public abstract ClueType Revert();

        public abstract bool Accept(IClueVisitor visitor);

        public bool IsStraightClue { get; protected set; }

        public abstract bool IsSubtleClue(IEnumerable<Card> expectedCards);
    }

    public class ClueAboutRank : ClueType
    {
        public Rank Rank { get; }

        public ClueAboutRank(Rank rank) 
        {
            Rank = rank;
            IsStraightClue = true;
        }

        public override ClueType Revert()
        {
            return new ClueAboutNotRank(Rank);
        }


        private bool EqualsCore(ClueAboutRank clueAboutRank)
        {
            return Rank == clueAboutRank.Rank;
        }


        public override bool Equals(object obj)
        {
            if (obj is ClueAboutRank rank)
                return EqualsCore(rank);
            return false;
        }


        public override int GetHashCode()
        {
            return Rank.GetHashCode();
        }


        public override bool Accept(IClueVisitor visitor)
        {
            return visitor.Visit(this);
        }


        public override bool IsSubtleClue(IEnumerable<Card> expectedCards)
        {
            // тонкая подсказка на ранг карты
            // тогда среди ожидаемых карт должна существовать одна или две карты таким же рангом.
            // все остальные должны быть рангом старше
            int diff = expectedCards.Count(card => card.Rank == Rank);

            return (diff == 1 || diff == 2) &&
                   expectedCards
                       .Where(card => card.Rank != Rank)
                       .All(card => card.Rank > Rank);
        }


        public override string ToString()
        {
            return Rank.ToString();
        }
    }

    public class ClueAboutNotRank : ClueType
    {
        public Rank Rank { get; }

        public ClueAboutNotRank(Rank rank)
        {
            Rank = rank;
            IsStraightClue = false;
        }


        public override ClueType Revert()
        {
            return new ClueAboutRank(Rank);
        }


        public override bool Accept(IClueVisitor visitor)
        {
            return visitor.Visit(this);
        }


        public override bool IsSubtleClue(IEnumerable<Card> expectedCards)
        {
            return false;
        }


        private bool EqualsCore(ClueAboutNotRank clueAboutNotRank)
        {
            return Rank == clueAboutNotRank.Rank;
        }


        public override bool Equals(object obj)
        {
            if (obj is ClueAboutNotRank rank)
                return EqualsCore(rank);
            return false;
        }


        public override int GetHashCode()
        {
            return Rank.GetHashCode();
        }


        public override string ToString()
        {
            return $"Not {Rank}";
        }
    }

    public class ClueAboutColor : ClueType
    {
        public Color Color { get; }

        public ClueAboutColor(Color color) 
        {
            Color = color;
            IsStraightClue = true;
        }


        public override ClueType Revert()
        {
            return new ClueAboutNotColor(Color);
        }


        public override bool Accept(IClueVisitor visitor)
        {
            return visitor.Visit(this);
        }


        public override bool IsSubtleClue(IEnumerable<Card> expectedCards)
        {
            // TODO подумать над реализацией
            var rankFive = Hanabi.Rank.Five;
            return expectedCards.Any(card => card.Color == Color && card.Rank == rankFive);
        }


        private bool EqualsCore(ClueAboutColor clueAboutColor)
        {
            return Color == clueAboutColor.Color;
        }


        public override bool Equals(object obj)
        {
            if (obj is ClueAboutColor color)
                return EqualsCore(color);
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

    public class ClueAboutNotColor : ClueType
    {
        public Color Color { get; }

        public ClueAboutNotColor(Color color)
        {
            Color = color;
            IsStraightClue = false;
        }

        public override ClueType Revert()
        {
            return new ClueAboutColor(Color);
        }

        public override bool Accept(IClueVisitor visitor)
        {
            return visitor.Visit(this);
        }

        public override bool IsSubtleClue(IEnumerable<Card> expectedCards)
        {
            return false;
        }

        public override int GetHashCode()
        {
            return Color.GetHashCode();
        }

        private bool EqualsCore(ClueAboutNotColor clueAboutNotColor)
        {
            return Color == clueAboutNotColor.Color;
        }

        public override bool Equals(object obj)
        {
            if (obj is ClueAboutNotColor color)
                return EqualsCore(color);
            return false;
        }

        public override string ToString()
        {
            return $"Not {Color}";
        }
    }
}