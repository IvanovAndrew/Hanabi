using System.Collections.Generic;
using System.Linq;

namespace Hanabi
{
    public class PlayCardEstimator : IEstimator
    {
        private readonly IPlayCardStrategy _strategy;

        public PlayCardEstimator(IPlayCardStrategy strategy)
        {
            _strategy = strategy;
        }

        /// <summary>
        /// Возвращает
        /// {карты, по которым есть тонкая подсказка} U
        /// {карты, \forall P(play) >= play_threshold} U
        /// {единицы, о которых игрок знает при условии, что единицу можно сыграть}
        /// </summary>
        /// <param name="boardContext"></param>
        /// <param name="playerContext"></param>
        /// <returns></returns>
        public IList<Card> GetPossibleCards(IBoardContext boardContext, IPlayerContext playerContext)
        {
            var estimates = _strategy.EstimateCardToPlayProbability(boardContext);

            var result =
                playerContext
                    .Hand
                    .Where(cardInHand => playerContext.IsSubtleClue(cardInHand, boardContext.GetExpectedCards()))
                    .Select(cardInHand => cardInHand.Card)
                    .ToList();

            if (!result.Any())
            {
                Probability maxProbability = estimates.Values.Max();

                if (maxProbability >= Player.PlayProbabilityThreshold)
                {
                    result =
                        (from e in estimates
                            where e.Value == maxProbability
                            orderby e.Value descending
                            select e.Key.Card).ToList();
                }
            }

            if (!result.Any())
            {
                if (boardContext.GetExpectedCards().Any(c => c.Rank == Rank.One))
                {
                    var cards = playerContext.Hand.Where(c => c.Card.Rank == Rank.One);

                    foreach (CardInHand card in cards)
                    {
                        var knowsAboutRank
                            = playerContext.GetCluesAboutCard(card)
                                .Where(c => c.IsStraightClue)
                                .Any(c => ClueDetailInfo.GetClueInfo(c).Rank == Rank.One);

                        if (knowsAboutRank)
                        {
                            result.Add(card.Card);
                        }
                    }
                }
            }

            return result;
        }
    }
}