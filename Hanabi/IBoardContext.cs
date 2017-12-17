using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Hanabi
{
    [ContractClass(typeof(BoardContextContract))]
    public interface IBoardContext
    {
        FireworkPile Firework { get; }
        DiscardPile DiscardPile { get; }
        IEnumerable<Card> UniqueCards { get; }
        IEnumerable<Card> WhateverToPlayCards { get; }
        IEnumerable<Card> ExcludedCards { get; }
    }

    [ContractClassFor(typeof(IBoardContext))]
    abstract class BoardContextContract : IBoardContext
    {
        public FireworkPile Firework
        {
            get
            {
                Contract.Ensures(Contract.Result<FireworkPile>() != null);

                throw new NotSupportedException();
            }
        }

        public DiscardPile DiscardPile
        {
            get
            {
                Contract.Ensures(Contract.Result<DiscardPile>() != null);

                throw new NotSupportedException();
            }
        }

        public IEnumerable<Card> UniqueCards
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<Card>>() != null);

                throw new NotSupportedException();
            }
        }

        public IEnumerable<Card> WhateverToPlayCards
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<Card>>() != null);

                throw new NotSupportedException();
            }
        }

        public IEnumerable<Card> ExcludedCards
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<Card>>() != null);

                throw new NotSupportedException();
            }
        }
    }
}
