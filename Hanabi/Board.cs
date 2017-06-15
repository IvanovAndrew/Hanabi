using System.Diagnostics.Contracts;

namespace Hanabi
{
    public class Board
    {
        public const int MaxClueCounter = 8;
        public const int MaxBlowCounter = 3;

        public Deck Deck;

        public FireworkPile FireworkPile;
        public DiscardPile DiscardPile;

        private int _clueCounter;
        public int ClueCounter
        {
            get
            {
                return _clueCounter;
            }
            set
            {
                if (0 <= value && value <= MaxClueCounter)
                    _clueCounter = value;
            }
        }

        private int _blowCounter;
        public int BlowCounter
        {
            get
            {
                return _blowCounter;
            }
            set
            {
                if (0 <= value && value <= MaxBlowCounter)
                    _blowCounter = value;
            }
        }

        private Board()
        {
            ClueCounter = MaxClueCounter;
            BlowCounter = MaxBlowCounter;
        }

        public static Board Create(bool isSpecial)
        {
            Contract.Ensures(Contract.Result<Board>() != null);

            Board board = new Board
            {
                FireworkPile = new FireworkPile(),
                DiscardPile = new DiscardPile(),
                Deck = Deck.Create(isSpecial)
            };

            return board;
        }
    }
}
