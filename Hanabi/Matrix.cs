using System;
using System.Diagnostics.Contracts;

namespace Hanabi
{
    public class Matrix
    {
        private readonly int[,] _matrix;
        private readonly IGameProvider _gameProvider;

        public Matrix(IGameProvider provider)
        {
            Contract.Requires<ArgumentNullException>(provider != null);

            _gameProvider = provider;
            _matrix = new int[provider.Ranks.Count, provider.Colors.Count];
        }

        public int this[Rank rank, Color color]
        {
            get => _matrix[(int) rank, _gameProvider.ColorToInt(color)];
            set => _matrix[(int) rank, _gameProvider.ColorToInt(color)] = value;
        }

        public int this[Card card]
        {
            get
            {
                Contract.Requires<ArgumentNullException>(card != null);

                return this[card.Rank, card.Color];
            }
            set
            {
                Contract.Requires<ArgumentNullException>(card != null);

                this[card.Rank, card.Color] = value;
            }
        }

        [Pure]
        public int Sum()
        {
            int sum = 0;
            foreach (var rank in _gameProvider.Ranks)
            {
                foreach (var color in _gameProvider.Colors)
                {
                    sum += this[rank, color];
                }
            }

            return sum;
        }
    }
}
