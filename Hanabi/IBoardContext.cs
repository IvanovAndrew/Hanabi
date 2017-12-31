using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Hanabi
{
    [ContractClass(typeof(BoardContextContract))]
    public interface IBoardContext
    {
        IEnumerable<Card> GetExpectedCards();
        IEnumerable<Card> GetUniqueCards();
        IEnumerable<Card> GetWhateverToPlayCards();
        IEnumerable<Card> GetExcludedCards();

        void AddToFirework(Card card);
        void Discard(Card card);
    }

    [ContractClassFor(typeof(IBoardContext))]
    abstract class BoardContextContract : IBoardContext
    {
        public IEnumerable<Card> GetUniqueCards()
        {
            Contract.Ensures(Contract.Result<IEnumerable<Card>>() != null);

            throw new NotSupportedException();
        }

        public IEnumerable<Card> GetWhateverToPlayCards()
        {
            Contract.Ensures(Contract.Result<IEnumerable<Card>>() != null);

            throw new NotSupportedException();
        }

        public IEnumerable<Card> GetExcludedCards()
        {
            Contract.Ensures(Contract.Result<IEnumerable<Card>>() != null);

            throw new NotSupportedException();
        }

        public IEnumerable<Card> GetExpectedCards()
        {
            Contract.Ensures(Contract.Result<IEnumerable<Card>>() != null);
            throw new NotSupportedException();
        }

        public void AddToFirework(Card card)
        {
            Contract.Requires(card != null);
        }

        public void Discard(Card card)
        {
            Contract.Requires(card != null);
        }
    }
}