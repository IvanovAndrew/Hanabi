using System.Diagnostics.Contracts;

namespace Hanabi
{
    public class Board
    {
        private const int MaxClueCounter = 8;
        private const int MaxBlowCounter = 3;

        private readonly Deck _deck;

        public Deck Deck
        {
            get { return _deck; }
        }

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

        private Board(Deck deck)
        {
            ClueCounter = MaxClueCounter;
            BlowCounter = MaxBlowCounter;
            _deck = deck;
        }

        public static Board Create(IGameProvider provider)
        {
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
