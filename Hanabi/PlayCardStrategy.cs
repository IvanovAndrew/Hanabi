using System;
using System.Collections.Generic;
using System.Linq;

namespace Hanabi
{
    public class PlayCardStrategy : IPlayCardStrategy
    {
        private readonly IEnumerable<Guess> _guesses;

        public PlayCardStrategy(IEnumerable<Guess> guesses)
        {
            _guesses = guesses ?? throw new ArgumentNullException(nameof(guesses));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="boardContext"></param>
        /// <returns></returns>
        public IDictionary<CardInHand, Probability> EstimateCardToPlayProbability(IBoardContext boardContext)
        {
            if (boardContext == null) throw new ArgumentNullException(nameof(boardContext));

            var cardsToPlay = boardContext.GetExpectedCards();
            var excludedCards = boardContext.GetExcludedCards();

            var dict = new Dictionary<CardInHand, Probability>();

            foreach (var guess in _guesses)
            {
                var probability = guess.GetProbability(cardsToPlay, excludedCards);

                dict[guess.CardInHand] = probability;
            }

            return dict;
        }
    }
}