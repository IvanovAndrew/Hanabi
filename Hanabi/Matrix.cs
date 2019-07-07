using System;

namespace Hanabi
{
    public class Matrix
    {
        private readonly int[,] _matrix;
        private readonly IGameProvider _gameProvider;

        public Matrix(IGameProvider provider)
        {
            _gameProvider = provider ?? throw new ArgumentNullException(nameof(provider));
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
                if (card == null) throw new ArgumentNullException(nameof(card));

                return this[card.Rank, card.Color];
            }
            set
            {
                if (card == null) throw new ArgumentNullException(nameof(card));

                this[card.Rank, card.Color] = value;
            }
        }

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
