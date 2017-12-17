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
            _gameProvider = provider;
            _matrix = new int[provider.Nominals.Count, provider.Colors.Count];
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
    }
}
