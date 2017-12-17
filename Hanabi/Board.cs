using System;
using System.Diagnostics.Contracts;

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
                Contract.Requires<ArgumentOutOfRangeException>(0 <= value && value <= MaxClueCounter);

                _clueCounter = value;
            }
        }

        private int _blowCounter;
        public int BlowCounter
        {
            get => _blowCounter;
            set
            {
                Contract.Requires<ArgumentOutOfRangeException>(0 <= value && value <= MaxBlowCounter);

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
            Contract.Requires<ArgumentNullException>(provider != null);
            Contract.Ensures(Contract.Result<Board>() != null);

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
