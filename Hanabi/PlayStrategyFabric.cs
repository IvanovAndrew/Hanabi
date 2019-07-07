using System;
using System.Collections.Generic;

namespace Hanabi
{
    public static class PlayStrategyFabric
    {
        public static IPlayCardStrategy Create(IGameProvider gameProvider, IPlayerContext playerContext)
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

            return new PlayCardStrategy(guesses);
        }
    }
}
