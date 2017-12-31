using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;

namespace Hanabi
{
    public class PlayCardStrategy : IPlayCardStrategy
    {
        private readonly IEnumerable<Guess> _guesses;

        public PlayCardStrategy(IEnumerable<Guess> guesses)
        {
            Contract.Requires<ArgumentNullException>(guesses != null);
            Contract.Requires(guesses.Any());

            _guesses = guesses;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="boardContext"></param>
        /// <returns></returns>
        public IDictionary<CardInHand, Probability> EstimateCardToPlayProbability(IBoardContext boardContext)
        {
            var cardsToPlay = boardContext.GetExpectedCards();

            var dict = new ConcurrentDictionary<CardInHand, Probability>();
            Parallel.ForEach(
                _guesses,
                guess =>
                {
                    var probability = guess.GetProbability(cardsToPlay, boardContext.GetExcludedCards());

                    if (!dict.TryAdd(guess.CardInHand, probability))
                        throw new Exception("Коллизии при добавлении в словарь");
                }
            );

            return dict;
        }
    }
}
