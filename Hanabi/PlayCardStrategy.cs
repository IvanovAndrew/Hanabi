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

            var dict = new Dictionary<CardInHand, Probability>();

            foreach (var guess in _guesses)
            {
                var probability = guess.GetProbability(cardsToPlay, boardContext.GetExcludedCards());

                dict[guess.CardInHand] = probability;
            }

            return dict;
        }
    }
}
