using System;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Hanabi
{
    public class ClueToPlayFinder
    {
        private readonly IBoardContext _boardContext;
        private readonly IPlayerContext _playerContext;

        public ClueToPlayFinder(IBoardContext boardContext, IPlayerContext playerContext)
        {
            Contract.Requires<ArgumentNullException>(boardContext != null);
            Contract.Requires<ArgumentNullException>(playerContext != null);
            
            _boardContext = boardContext;
            _playerContext = playerContext;
        }

        public ClueAndAction Find()
        {
            var expectedCards = _boardContext.GetExpectedCards();

            var cardsToPlay =
                _playerContext.Hand
                .Where(cih => expectedCards.Contains(cih.Card))
                .OrderBy(cih => cih.Card.Rank)
                .ToList();

            if (cardsToPlay.Any())
            {
                foreach (var card in cardsToPlay)
                {
                    foreach (var clue in ClueDetailInfo.CreateClues(card, _playerContext.Player))
                    {
                        var playerContext = _playerContext.Clone();
                        playerContext.PossibleClue = clue;
                        
                        PlayerActionPredictor predictor = new PlayerActionPredictor(_boardContext, playerContext);
                        IPlayCardStrategy playStrategy =
                            PlayStrategyFabric.Create(playerContext.Player.GameProvider, playerContext);

                        IDiscardStrategy discardStrategy =
                            DiscardStrategyFabric.Create(playerContext.Player.GameProvider, playerContext);

                        var action = predictor.Predict(playStrategy, discardStrategy);

                        if (action.PlayCard) return new ClueAndAction {Action = action, Clue = clue};
                    }
                }
            }
            
            return null;
        }
    }
}
