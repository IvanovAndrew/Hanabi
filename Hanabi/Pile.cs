using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Hanabi
{
    [ContractClass(typeof(PileContract))]
    public abstract class Pile
    {
        protected Matrix Matrix;
        protected IGameProvider Provider;
        protected CardsToMatrixConverter Converter;

        protected Pile(IGameProvider provider)
        {
            Contract.Requires<ArgumentNullException>(provider != null);

            Provider = provider;
            Matrix = provider.CreateEmptyMatrix();
            Converter = new CardsToMatrixConverter(provider);
        }

        public IReadOnlyList<Card> Cards => Converter.Decode(Matrix);

        public abstract bool AddCard(Card card);
    }

    [ContractClassFor(typeof(Pile))]
    public abstract class PileContract : Pile
    {
        public override bool AddCard(Card card)
        {
            Contract.Requires<ArgumentNullException>(card != null);

            throw new NotImplementedException();
        }

        protected PileContract(IGameProvider provider) : base(provider)
        {
            Contract.Requires<ArgumentNullException>(provider != null);
        }
    }
}