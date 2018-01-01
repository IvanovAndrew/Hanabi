using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Hanabi
{
    public class ClueCreator
    {
        private readonly PlayerContext _playerContext;
        private readonly IBoardContext _boardContext;
        private readonly IPlayCardStrategy _playCardStrategy;
        private readonly IDiscardStrategy _discardStrategy;

        public ClueCreator(IBoardContext boardContext, PlayerContext playerContext, IPlayCardStrategy playCardStrategy, IDiscardStrategy discardStrategy)
        {
            Contract.Requires<ArgumentNullException>(boardContext != null);
            Contract.Requires<ArgumentNullException>(playerContext != null);
            Contract.Requires<ArgumentNullException>(playCardStrategy != null);
            Contract.Requires<ArgumentNullException>(discardStrategy != null);

            _boardContext = boardContext;
            _playerContext = playerContext;
            _playCardStrategy = playCardStrategy;
            _discardStrategy = discardStrategy;
        }

        public ClueType CreateClueToPlay(IPlayerContext playerContext)
        {
            var expectedCards = _boardContext.GetExpectedCards();

            // сразу уберём карты, о которых игрок знает.
            var cardsToSearch = expectedCards.Except(playerContext.Player.GetKnownCards()).ToList();

            if (!cardsToSearch.Any()) return null;

            var cardsToPlay =
                playerContext.Hand
                    .Where(cardInHand => cardsToSearch.Contains(cardInHand.Card))
                    .ToList();

            if (!cardsToPlay.Any()) return null;

            cardsToPlay =
                cardsToPlay.OrderBy(cardInHand => (int)cardInHand.Card.Rank).ToList();

            var clues = new List<ClueType>();


            return null;
        }

        public ClueType CreateClue()
        {
            // если можно подсказать так, чтобы игрок сходил, то подсказать.

            // подсказать так, чтобы игрок не выкинул карты, которыми можно сходить
            // если не выходит, то подсказать так, чтобы игрок не выкинул уникальные карты
            throw new NotImplementedException();
        }
    }
}
