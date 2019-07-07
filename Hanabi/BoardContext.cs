using System;
using System.Collections.Generic;
using System.Linq;

namespace Hanabi
{
    public class BoardContext : IBoardContext
    {
        private FireworkPile _fireworkPile;
        private DiscardPile _discardPile;
        
        private readonly IList<Card> _possiblePlayed = new List<Card>();
        private readonly IList<Card> _possibleDiscarded = new List<Card>();

        private PilesAnalyzer _pilesAnalyzer;
        private IEnumerable<Card> _otherPlayerCards;

        private BoardContext()
        {
            
        }

        public void AddToFirework(Card card)
        {
            _possiblePlayed.Add(card);
        }

        public int BlowCounter { get; set; }

        public IEnumerable<Card> GetExpectedCards()
        {
            return GetPossibleFireworkPile().GetExpectedCards();
        }

        public IEnumerable<Card> GetUniqueCards()
        {
            var possibleFirework = GetPossibleFireworkPile();
            var possibleDiscardPile = GetPossibleDiscardPile();

            return _pilesAnalyzer.GetUniqueCards(possibleFirework, possibleDiscardPile);
        }

        public IEnumerable<Card> GetWhateverToPlayCards()
        {
            var possibleFirework = GetPossibleFireworkPile();
            var possibleDiscardPile = GetPossibleDiscardPile();

            return _pilesAnalyzer.GetCardsWhateverToPlay(possibleFirework, possibleDiscardPile);
        }

        public IEnumerable<Card> GetExcludedCards()
        {
            return 
                _pilesAnalyzer
                    .GetThrownCards(_fireworkPile, _discardPile)
                    .Concat(_otherPlayerCards);
        }

        private FireworkPile GetPossibleFireworkPile()
        {
            var possibleFirework = _fireworkPile.Clone();
            foreach (var card in _possiblePlayed)
            {
                possibleFirework.AddCard(card);
            }
            return possibleFirework;
        }

        private DiscardPile GetPossibleDiscardPile()
        {
            var possibleDiscardPile = _discardPile.Clone();
            foreach (var card in _possibleDiscarded)
            {
                possibleDiscardPile.AddCard(card);
            }
            return possibleDiscardPile;
        }

        public void Discard(Card card)
        {
            _possibleDiscarded.Add(card);
        }

        public IBoardContext ChangeContext(IEnumerable<Card> otherPlayerCards)
        {
            var newContext =
                new BoardContext.Builder()
                    .WithFireworkPile(_fireworkPile)
                    .WithDiscardPile(_discardPile)
                    .WithPilesAnalyzer(_pilesAnalyzer)
                    .WithOtherPlayersCards(otherPlayerCards)
                    .WithBlowCounter(BlowCounter)
                    .Build();

            foreach (var card in _possiblePlayed)
            {
                newContext.AddToFirework(card);
            }

            foreach (var card in _possibleDiscarded)
            {
                newContext.Discard(card);
            }

            return newContext;
        }

        public class Builder
        {
            private readonly BoardContext _boardContext;

            public Builder()
            {
                _boardContext = new BoardContext();
            }

            public Builder WithBoard(Board board)
            {
                if (board == null)
                    throw new ArgumentNullException(nameof(board));

                _boardContext._fireworkPile = board.FireworkPile;
                _boardContext._discardPile = board.DiscardPile;
                _boardContext.BlowCounter = board.BlowCounter;

                return this;
            }

            public Builder WithFireworkPile(FireworkPile fireworkPile)
            {
                _boardContext._fireworkPile = fireworkPile ?? throw new ArgumentNullException(nameof(fireworkPile));
                return this;
            }

            public Builder WithDiscardPile(DiscardPile discardPile)
            {
                _boardContext._discardPile = discardPile ?? throw new ArgumentNullException(nameof(discardPile));
                return this;
            }

            public Builder WithBlowCounter(int blowCounter)
            {
                if (blowCounter < 0)
                    throw new ArgumentOutOfRangeException(nameof(blowCounter));

                _boardContext.BlowCounter = blowCounter;
                return this;
            }

            public Builder WithPilesAnalyzer(PilesAnalyzer analyzer)
            {
                _boardContext._pilesAnalyzer = analyzer ?? throw new ArgumentNullException(nameof(analyzer));
                return this;
            }

            public Builder WithOtherPlayersCards(IEnumerable<Card> otherPlayersCards)
            {
                _boardContext._otherPlayerCards = otherPlayersCards;
                return this;
            }

            public BoardContext Build()
            {
                return _boardContext;
            }
        }
    }
}