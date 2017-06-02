using System.Collections.Generic;

namespace Hanabi
{
    public class DiscardPile
    {
        private List<Card> _cards = new List<Card>();
        public IReadOnlyList<Card> Cards { get{ return _cards.AsReadOnly();} }

        public void AddCard(Card card)
        {
            _cards.Add(card);
        }
    }
}