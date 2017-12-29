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
        /// {карты, \forall P(play) >= play_threshold}
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
                    .Where(cardInHand => playerContext.IsSubtleClue(cardInHand, boardContext.Firework))
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

            return result;
        }
    }
}