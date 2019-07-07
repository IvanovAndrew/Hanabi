using System;
using System.Collections.Generic;

namespace Hanabi
{
    public interface IPlayerContext
    {
        Player Player { get; }
        IEnumerable<CardInHand> Hand { get; }
        ClueType PossibleClue { get; set; }

        bool IsSubtleClue(CardInHand cardInHand, IEnumerable<Card> expectedCards);
        IList<ClueType> GetCluesAboutCard(CardInHand cardInHand);
        bool KnowAboutRankOrColor(CardInHand cardInHand);
        IPlayerContext Clone();
    }
}
