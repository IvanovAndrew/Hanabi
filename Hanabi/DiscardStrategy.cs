using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hanabi
{
    public class DiscardStrategy : IDiscardStrategy
    {
        private readonly IReadOnlyList<Guess> _guesses;

        public DiscardStrategy(IEnumerable<Guess> guesses)
        {
            if (guesses == null) throw new ArgumentNullException(nameof(guesses));

            _guesses = guesses.ToList().AsReadOnly();
        }

        /// <summary>
        /// Для каждой карты оценивает вероятность быть сброшенной
        /// </summary>
        /// <param name="boardContext"></param>
        /// <returns></returns>
        public IDictionary<CardInHand, Probability> EstimateDiscardProbability(IBoardContext boardContext)
        {
            if (boardContext == null) throw new ArgumentNullException(nameof(boardContext));

            var result = new ConcurrentDictionary<CardInHand, Probability>();

            // для каждой карты посчитаем вероятность того, что она будет сброшена
            Parallel.ForEach(
                _guesses,
                guess =>
                {
                    // я считаю вероятность того, что карта будет сброшена
                    // в идеале можно определить как вероятность того, что эта карта принадлежит множеству карт, которые уже не нужны
                    // то есть {все карты} \ {карты, которые ещё можно положить на стол}
                    var temp = guess.GetProbability(boardContext.GetWhateverToPlayCards(), boardContext.GetExcludedCards());

                    var probability = new Probability(1 - temp.Value);

                    result.TryAdd(guess.CardInHand, probability);
                }
            );
            
            return result;
        }
    }
}
