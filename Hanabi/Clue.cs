namespace Hanabi
{
    public abstract class Clue
    {
        public abstract Clue Revert();

        public abstract bool Accept(IClueVisitor visitor);

        public bool IsStraightClue { get; protected set; }
    }

    public class ClueAboutRank : Clue
    {
        public Rank Rank { get; }

        public ClueAboutRank(Rank rank)
        {
            Rank = rank;
            IsStraightClue = true;
        }

        public override Clue Revert()
        {
            return new ClueAboutNotRank(Rank);
        }

        private bool EqualsCore(ClueAboutRank clue)
        {
            return Rank == clue.Rank;
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

        public override string ToString()
        {
            return Rank.ToString();
        }
    }

    public class ClueAboutNotRank : Clue
    {
        public Rank Rank { get; }

        public ClueAboutNotRank(Rank rank)
        {
            Rank = rank;
            IsStraightClue = false;
        }

        public override Clue Revert()
        {
            return new ClueAboutRank(Rank);
        }

        public override bool Accept(IClueVisitor visitor)
        {
            return visitor.Visit(this);
        }

        private bool EqualsCore(ClueAboutNotRank clue)
        {
            return Rank == clue.Rank;
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

    public class ClueAboutColor : Clue
    {
        public Color Color { get; }

        public ClueAboutColor(Color color)
        {
            Color = color;
            IsStraightClue = true;
        }

        public override Clue Revert()
        {
            return new ClueAboutNotColor(Color);
        }

        public override bool Accept(IClueVisitor visitor)
        {
            return visitor.Visit(this);
        }

        private bool EqualsCore(ClueAboutColor clue)
        {
            return Color == clue.Color;
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

    public class ClueAboutNotColor : Clue
    {
        public Color Color { get; }

        public ClueAboutNotColor(Color color)
        {
            Color = color;
            IsStraightClue = false;
        }

        public override Clue Revert()
        {
            return new ClueAboutColor(Color);
        }

        public override bool Accept(IClueVisitor visitor)
        {
            return visitor.Visit(this);
        }

        public override int GetHashCode()
        {
            return Color.GetHashCode();
        }

        private bool EqualsCore(ClueAboutNotColor clue)
        {
            return Color == clue.Color;
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