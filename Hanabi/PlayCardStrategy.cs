using System;
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
        public CardProbability EstimateCardToPlayProbability(IBoardContext boardContext)
        {
            var cardsToPlay = boardContext.Firework.GetExpectedCards();

            var dict = new CardProbability();
            Parallel.ForEach(
                _guesses,
                guess =>
                {
                    var probability = guess.GetProbability(cardsToPlay, boardContext.ExcludedCards);

                    if (!dict.TryAdd(guess.CardInHand, probability))
                        throw new Exception("Коллизии при добавлении в словарь");
                }
            );

            return dict;
        }
    }
}
