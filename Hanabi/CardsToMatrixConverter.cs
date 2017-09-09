using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Hanabi
{
    public class CardsToMatrixConverter
    {
        private readonly IGameProvider _provider;

        public CardsToMatrixConverter(IGameProvider provider)
        {
            _provider = provider;
        }

        public Matrix Encode(IEnumerable<Card> cards)
        {
            Contract.Requires<ArgumentNullException>(cards != null);
            Contract.Ensures(Contract.Result<Matrix>() != null);

            Matrix result = _provider.CreateEmptyMatrix();

            foreach (Card card in cards)
            {
                result[card]++;
            }

            return result;
        }

        public IReadOnlyList<Card> Decode(Matrix matrix)
        {
            Contract.Requires<ArgumentNullException>(matrix != null);
            Contract.Ensures(Contract.Result<IReadOnlyList<Card>>() != null);

            var result = new List<Card>();

            foreach (var number in _provider.Nominals)
            {
                foreach (var color in _provider.Colors)
                {
                    for (int i = 0; i < matrix[number, color]; i++)
                    {
                        result.Add(new Card(color, number));
                    }
                }
            }

            return result.AsReadOnly();
        }
    }
}
