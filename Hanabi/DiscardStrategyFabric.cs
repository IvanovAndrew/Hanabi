using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Hanabi
{
    public class DiscardStrategyFabric
    {
        public static IDiscardStrategy Create(IEnumerable<Guess> guesses)
        {
            Contract.Requires<ArgumentNullException>(guesses != null);
            Contract.Requires(guesses.Any());

            Contract.Ensures(Contract.Result<IDiscardStrategy>() != null);

            return new DiscardStrategy(guesses);
        }

        public static IDiscardStrategy Create(IGameProvider gameProvider, IPlayerContext playerContext)
        {
            Contract.Requires<ArgumentNullException>(gameProvider != null);
            Contract.Requires<ArgumentNullException>(playerContext != null);
            Contract.Ensures(Contract.Result<IDiscardStrategy>() != null);

            List<Guess> guesses = new List<Guess>();
            foreach (var cardInHand in playerContext.Hand)
            {
                var guess = new Guess(gameProvider, cardInHand);

                foreach (var clue in playerContext.GetCluesAboutCard(cardInHand))
                {
                    clue.Accept(guess);
                }
                guesses.Add(guess);
            }

            return new DiscardStrategy(guesses);
        }
    }
}
