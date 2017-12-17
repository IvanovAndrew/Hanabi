using System.Diagnostics.Contracts;
using System.Linq;

namespace Hanabi
{
    public class DiscardPile : Pile
    {
        public override bool AddCard(Card card)
        {
            Matrix[card]++;
            return true;
        }

        public DiscardPile(IGameProvider provider) : base(provider)
        {
        }

        public DiscardPile Clone()
        {
            var contractResult = Contract.Result<DiscardPile>();
            Contract.Ensures(contractResult != null);
            Contract.Ensures(contractResult.Cards.Count == this.Cards.Count);
            Contract.Ensures(Contract.ForAll(contractResult.Cards, card => Cards.Contains(card)));

            var clone = new DiscardPile(this.Provider);

            foreach (var card in Cards)
            {
                clone.AddCard(card);
            }
            return clone;
        }
    }
}