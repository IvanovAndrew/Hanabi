using System;
using System.Collections.Generic;

namespace Hanabi
{
    public class DiscardStrategyFabric
    {
        public static IDiscardStrategy Create(IEnumerable<Guess> guesses)
        {
            if (guesses == null) throw new ArgumentNullException(nameof(guesses));

            return new DiscardStrategy(guesses);
        }

        public static IDiscardStrategy Create(IGameProvider gameProvider, IPlayerContext playerContext)
        {
            if (gameProvider == null) throw new ArgumentNullException(nameof(gameProvider));
            if (playerContext == null) throw new ArgumentNullException(nameof(playerContext));

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
