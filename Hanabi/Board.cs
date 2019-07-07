using System;

namespace Hanabi
{
    public class Board
    {
        private const int MaxClueCounter = 8;
        private const int MaxBlowCounter = 3;

        public Deck Deck { get; }

        public FireworkPile FireworkPile;
        public DiscardPile DiscardPile;

        private int _clueCounter;
        public int ClueCounter
        {
            get => _clueCounter;
            set
            {
                if (value < 0 || MaxClueCounter < value )
                    throw new ArgumentOutOfRangeException();

                _clueCounter = value;
            }
        }

        private int _blowCounter;
        public int BlowCounter
        {
            get => _blowCounter;
            set
            {
                if (value < 0 || MaxBlowCounter < value)
                    throw new ArgumentOutOfRangeException();

                _blowCounter = value;
            }
        }

        private Board(Deck deck)
        {
            ClueCounter = MaxClueCounter;
            BlowCounter = MaxBlowCounter;
            Deck = deck;
        }

        public static Board Create(IGameProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException();

            var cards = new CardsToMatrixConverter(provider).Decode(provider.CreateFullDeckMatrix());

            Board board = new Board(new Deck(cards))
            {
                FireworkPile = new FireworkPile(provider),
                DiscardPile = new DiscardPile(provider),
            };

            return board;
        }
    }
}
