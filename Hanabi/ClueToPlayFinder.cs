using System;
using System.Collections.Generic;
using System.Linq;

namespace Hanabi
{
    public class ClueToPlayFinder
    {
        private readonly IBoardContext _boardContext;
        private readonly IPlayerContext _playerContext;

        public ClueToPlayFinder(IBoardContext boardContext, IPlayerContext playerContext)
        {
            _boardContext = boardContext?? throw new ArgumentNullException(nameof(boardContext));
            _playerContext = playerContext?? throw new ArgumentNullException(nameof(playerContext));
        }

        public ClueAndAction Find()
        {
            var expectedCards = _boardContext.GetExpectedCards();

            var expectedThatCanPlay =
                _playerContext.Hand
                    .Where(cih => expectedCards.Contains(cih.Card))
                    .ToList();

            var cardsToPlay = 
                expectedThatCanPlay.Where(cih => cih.Card.Rank == Rank.Five)
                .Concat(
                    expectedThatCanPlay
                    .Where(cih => cih.Card.Rank != Rank.Five)
                    .OrderBy(cih => cih.Card.Rank)
                    )
                .ToList();


            List<ClueAndAction> possibleClues = new List<ClueAndAction>();
            if (cardsToPlay.Any())
            {
                var clues = 
                    cardsToPlay
                        .Select(card => ClueDetailInfo.CreateClues(card, _playerContext))
                        .Aggregate((acc, list) => acc.Concat(list).ToList())
                        .Distinct();

                foreach (var clue in clues)
                {
                    var playerContext = _playerContext.Clone();
                    playerContext.PossibleClue = clue;
                        
                    PlayerActionPredictor predictor = new PlayerActionPredictor(_boardContext, playerContext);
                    IPlayCardStrategy playStrategy =
                        PlayStrategyFabric.Create(playerContext.Player.GameProvider, playerContext);

                    IDiscardStrategy discardStrategy =
                        DiscardStrategyFabric.Create(playerContext.Player.GameProvider, playerContext);

                    var action = predictor.Predict(playStrategy, discardStrategy);

                    if ((action.Outcome & OutcomeFlags.Play) > 0)
                        possibleClues.Add(new ClueAndAction { Action = action, Clue = clue });
                }
            }

            if (possibleClues.Count <= 1) return possibleClues.FirstOrDefault();

            // раз возможных подсказок больше одной, то выберем ту, которая затрагивает большее число карт
            // из тех, которыми можно сыграть
            int max = 0;
            ClueAndAction clueAndAction = null;
            foreach (var clue in possibleClues)
            {
                int cardsAffected = CardsToClue(clue.Clue);
                if (cardsAffected > max)
                {
                    max = cardsAffected;
                    clueAndAction = clue;
                }
            }

            return clueAndAction;
        }

        private int CardsToClue(ClueType clue)
        {
            if (clue == null) throw new ArgumentNullException(nameof(clue));

            return _playerContext.Hand
                    .Select(cih => cih.Card)
                    .Where(_boardContext.GetWhateverToPlayCards().Contains)
                    .Select(card => new ClueAndCardMatcher(card))
                    .Count(clue.Accept);
        }
    }
}
