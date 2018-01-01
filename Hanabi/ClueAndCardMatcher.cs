namespace Hanabi
{
    public class ClueAndCardMatcher : IClueVisitor
    {
        public Card Card { get; }

        public ClueAndCardMatcher(Card card)
        {
            Card = card;
        }

        public bool Visit(ClueAboutRank clueAboutRank)
        {
            return clueAboutRank.Rank == Card.Rank;
        }

        public bool Visit(ClueAboutNotRank clueAboutNotRank)
        {
            return clueAboutNotRank.Rank != Card.Rank;
        }

        public bool Visit(ClueAboutColor clueAboutColor)
        {
            return clueAboutColor.Color == Card.Color;
        }

        public bool Visit(ClueAboutNotColor clueAboutNotColor)
        {
            return clueAboutNotColor.Color != Card.Color;
        }
    }
}