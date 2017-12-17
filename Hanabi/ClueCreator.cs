using System;
using System.Diagnostics.Contracts;

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

        public Clue CreateClue()
        {
            // если можно подсказать так, чтобы игрок сходил, то подсказать.

            // подсказать так, чтобы игрок не выкинул карты, которыми можно сходить
            // если не выходит, то подсказать так, чтобы игрок не выкинул уникальные карты
            throw new NotImplementedException();
        }
    }
}
