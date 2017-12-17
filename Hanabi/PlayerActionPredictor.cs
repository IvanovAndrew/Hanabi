using System.Diagnostics.Contracts;
using System.Linq;

namespace Hanabi
{
    public class PlayerActionPredictor
    {
        private readonly IBoardContext _boardContext;
        private readonly IPlayerContext _playerContext;

        public PlayerActionPredictor(
            IBoardContext boardContext,
            IPlayerContext playerContext)
        {
            _boardContext = boardContext;
            _playerContext = playerContext;
        }

        public PlayerAction Predict(IPlayCardStrategy playCardStrategy, IDiscardStrategy discardStrategy)
        {
            Contract.Requires(playCardStrategy != null);
            Contract.Requires(discardStrategy != null);

            Contract.Ensures(Contract.Result<PlayerAction>() != null);

            IEstimator playCardEstimator = new PlayCardEstimator(playCardStrategy);
            var cardsToPlay = playCardEstimator.GetPossibleCards(_boardContext, _playerContext).ToList();

            var expectedCards = _boardContext.Firework.GetExpectedCards();

            if (cardsToPlay.Any())
            {
                var possibleBlowCards = cardsToPlay.Except(expectedCards);
                if (!possibleBlowCards.Any())
                {
                    // карта/карты, которыми собирается сыграть игрок, к взрыву не приводят.
                    if (cardsToPlay.Any(c => c.Rank == Rank.Five))
                    {
                        return new PlayCardWithRankFiveAction {CardsToPlay = cardsToPlay};
                    }
                    else
                    {
                        return new PlayCardAction{CardsToPlay = cardsToPlay.ToList()};
                    }
                }
                else
                {
                    // одна из карт, которыми скорее всего сыграют, приведёт к взрыву.
                    return new BlowCardAction(){ CardsToBlow = possibleBlowCards.ToList()};
                }
            }

            // раз игрок сыграть не может, то будет сбрасывать...

            IEstimator discardEstimator = new DiscardEstimator(discardStrategy);
            var cardsToDiscard = discardEstimator.GetPossibleCards(_boardContext, _playerContext);

            var uniqueCardsToDiscard = cardsToDiscard.Intersect(_boardContext.UniqueCards).ToList();
            if (uniqueCardsToDiscard.Any())
                return new DiscardUniqueCardAction() {CardsToDiscard = uniqueCardsToDiscard};

            var cardsWhateverToPlay = cardsToDiscard.Intersect(_boardContext.WhateverToPlayCards).ToList();
            if (cardsWhateverToPlay.Any())
                return new DiscardCardWhateverToPlayAction() {CardsToDiscard = cardsWhateverToPlay};
            
            return new DiscardNoNeedCard();
        }
    }
}
