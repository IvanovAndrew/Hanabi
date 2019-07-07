using System;
using System.Collections.Generic;

namespace Hanabi
{
    public class CardsToMatrixConverter
    {
        private readonly IGameProvider _provider;

        public CardsToMatrixConverter(IGameProvider provider)
        {
            _provider = provider?? throw new ArgumentNullException(nameof(provider));
        }

        public Matrix Encode(IEnumerable<Card> cards)
        {
            if (cards == null)
                throw new ArgumentNullException(nameof(cards));

            Matrix result = _provider.CreateEmptyMatrix();

            foreach (Card card in cards)
            {
                result[card]++;
            }

            return result;
        }

        public IReadOnlyList<Card> Decode(Matrix matrix)
        {
            if (matrix == null)
                throw new ArgumentNullException(nameof(matrix));

            var result = new List<Card>();

            foreach (var number in _provider.Ranks)
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
