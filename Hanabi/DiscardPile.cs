using System.Diagnostics.Contracts;

namespace Hanabi
{
    public class DiscardPile : Pile
    {
        public override bool AddCard(Card card)
        {
            Contract.Requires(card != null);

            _cards.Add(card);
            return true;
        }
    }
}