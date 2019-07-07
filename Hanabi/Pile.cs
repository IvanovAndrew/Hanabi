using System;
using System.Collections.Generic;

namespace Hanabi
{
    public abstract class Pile
    {
        protected Matrix Matrix;
        protected IGameProvider Provider;
        protected CardsToMatrixConverter Converter;

        protected Pile(IGameProvider provider)
        {
            Provider = provider ?? throw new ArgumentNullException(nameof(provider));
            Matrix = provider.CreateEmptyMatrix();
            Converter = new CardsToMatrixConverter(provider);
        }

        public IReadOnlyList<Card> Cards => Converter.Decode(Matrix);

        public abstract bool AddCard(Card card);
    }
}