using System;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Hanabi
{
    public class ClueToPlayFinder
    {
        private readonly BoardContext _boardContext;
        private readonly PlayerContext _playerContext;

        public ClueToPlayFinder(BoardContext boardContext, PlayerContext playerContext)
        {
            Contract.Requires<ArgumentNullException>(boardContext != null);
            Contract.Requires<ArgumentNullException>(playerContext != null);
            
            _boardContext = boardContext;
            _playerContext = playerContext;
        }

        public ClueType Find(IPlayCardStrategy playStrategy, IDiscardStrategy discardStrategy)
        {
            var expectedCards = _boardContext.GetExpectedCards();

            var cardsToPlay =
                _playerContext.Hand.Where(cih => expectedCards.Contains(cih.Card))
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
                        var action = predictor.Predict(playStrategy, discardStrategy);

                        if (action.PlayCard) return clue;
                    }
                }
            }
            
            return null;
        }
    }
}
