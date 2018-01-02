﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Hanabi
{
    public class BoardContext : IBoardContext
    {
        private readonly FireworkPile _fireworkPile;
        private readonly DiscardPile _discardPile;
        
        private readonly IList<Card> _possiblePlayed = new List<Card>();
        private readonly IList<Card> _possibleDiscarded = new List<Card>();

        private readonly PilesAnalyzer _pilesAnalyzer;
        private readonly IEnumerable<Card> _otherPlayerCards;

        private BoardContext(FireworkPile fireworkPile, DiscardPile discardPile, PilesAnalyzer pilesAnalyzer, IEnumerable<Card> otherPlayerCards)
        {
            _fireworkPile = fireworkPile;
            _discardPile = discardPile;
            _pilesAnalyzer = pilesAnalyzer;
            _otherPlayerCards = otherPlayerCards;
        }

        public static IBoardContext Create(Board board, PilesAnalyzer pilesAnalyzer, IEnumerable<Card> otherPlayersCards)
        {
            Contract.Requires<ArgumentNullException>(board != null);
            Contract.Requires<ArgumentNullException>(pilesAnalyzer != null);
            Contract.Requires<ArgumentNullException>(otherPlayersCards != null);

            Contract.Ensures(Contract.Result<IBoardContext>() != null);
            
            return new BoardContext(board.FireworkPile, board.DiscardPile, pilesAnalyzer, otherPlayersCards);
        }

        public static IBoardContext Create(
            FireworkPile fireworkPile,
            DiscardPile discardPile,
            PilesAnalyzer pilesAnalyzer,
            IEnumerable<Card> otherPlayersCards)
        {
            Contract.Requires<ArgumentNullException>(fireworkPile != null);
            Contract.Requires<ArgumentNullException>(discardPile != null);
            Contract.Requires<ArgumentNullException>(pilesAnalyzer != null);
            Contract.Requires<ArgumentNullException>(otherPlayersCards != null);

            return new BoardContext(fireworkPile, discardPile, pilesAnalyzer, otherPlayersCards);
        }

        public void AddToFirework(Card card)
        {
            _possiblePlayed.Add(card);
        }

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
            return _pilesAnalyzer.GetThrownCards(_fireworkPile, _discardPile).Concat(_otherPlayerCards);
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
            var newContext = BoardContext.Create(_fireworkPile, _discardPile, _pilesAnalyzer, otherPlayerCards);

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
    }
}