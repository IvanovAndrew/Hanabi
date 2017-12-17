using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Hanabi
{
    public static class PlayStrategyFabric
    {
        public static IPlayCardStrategy Create(IGameProvider gameProvider, IPlayerContext playerContext)
        {
            Contract.Requires<ArgumentNullException>(gameProvider != null);
            Contract.Requires<ArgumentNullException>(playerContext != null);

            Contract.Ensures(Contract.Result<IPlayCardStrategy>() != null);

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
