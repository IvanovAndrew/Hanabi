using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Hanabi
{
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

        public IReadOnlyList<Card> Cards
        {
            get { return Converter.Decode(Matrix); }
        }

        public abstract bool AddCard(Card card);
    }
}
