using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Hanabi
{
    public class BoardContext : IBoardContext
    {
        public FireworkPile Firework { get; }
        public DiscardPile DiscardPile { get; }
        public IEnumerable<Card> UniqueCards => _pilesAnalyzer.GetUniqueCards(Firework, DiscardPile);

        public IEnumerable<Card> WhateverToPlayCards => _pilesAnalyzer.GetCardsWhateverToPlay(Firework, DiscardPile);

        public IEnumerable<Card> ExcludedCards => _pilesAnalyzer.GetThrownCards(Firework, DiscardPile).Concat(_otherPlayerCards);

        private readonly PilesAnalyzer _pilesAnalyzer;
        private readonly IEnumerable<Card> _otherPlayerCards;

        private BoardContext(FireworkPile firework, DiscardPile discardPile, PilesAnalyzer pilesAnalyzer, IEnumerable<Card> otherPlayerCards)
        {
            Firework = firework;
            DiscardPile = discardPile;
            _pilesAnalyzer = pilesAnalyzer;
            _otherPlayerCards = otherPlayerCards;
        }

        public static BoardContext Create(Board board, PilesAnalyzer pilesAnalyzer, IEnumerable<Card> otherPlayersCards)
        {
            Contract.Requires<ArgumentNullException>(board != null);
            Contract.Requires<ArgumentNullException>(pilesAnalyzer != null);
            Contract.Requires<ArgumentNullException>(otherPlayersCards != null);

            Contract.Ensures(Contract.Result<BoardContext>() != null);
            
            return new BoardContext(board.FireworkPile, board.DiscardPile, pilesAnalyzer, otherPlayersCards);
        }

        public static BoardContext Create(
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

        [ContractInvariantMethod]
        private void CardContextInvariant()
        {
            Contract.Invariant(Firework != null);
            Contract.Invariant(UniqueCards != null);
            Contract.Invariant(ExcludedCards != null);
        }
    }
}

