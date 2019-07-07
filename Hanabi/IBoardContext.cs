using System;
using System.Collections.Generic;

namespace Hanabi
{
    public interface IBoardContext
    {
        int BlowCounter { get; }

        IEnumerable<Card> GetExpectedCards();
        IEnumerable<Card> GetUniqueCards();
        IEnumerable<Card> GetWhateverToPlayCards();
        IEnumerable<Card> GetExcludedCards();

        void AddToFirework(Card card);
        void Discard(Card card);
        IBoardContext ChangeContext(IEnumerable<Card> otherPlayerCards);
    }
}