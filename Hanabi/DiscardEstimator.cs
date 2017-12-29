using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Hanabi
{
    public class DiscardEstimator : IEstimator
    {
        private readonly IDiscardStrategy _strategy;

        public DiscardEstimator(IDiscardStrategy strategy)
        {
            Contract.Requires<ArgumentNullException>(strategy != null);

            _strategy = strategy;
        }

        public IList<Card> GetPossibleCards(IBoardContext boardContext, IPlayerContext playerContext)
        {
            var discardEstimates =
                _strategy.EstimateDiscardProbability(boardContext);

            var knowAboutRankOrColorDict = new Dictionary<CardInHand, bool>();

            foreach (var cardInHand in playerContext.Hand)
            {
                knowAboutRankOrColorDict[cardInHand] = playerContext.KnowAboutRankOrColor(cardInHand);
            }

            // сперва ищем те, которые точно можно выкинуть
            var cardsToDiscard = discardEstimates
                .Where(entry => entry.Value == Probability.Maximum)
                .Select(dp => dp.Key.Card)
                .ToList();

            if (cardsToDiscard.Any()) return cardsToDiscard;

            Probability maxProbability;
            // не нашли. 
            // тогда поищем среди тех карт, на которые не было прямых подсказок
            if (playerContext.Hand.Any(cardInHand => !knowAboutRankOrColorDict[cardInHand]))
            {
                maxProbability = discardEstimates
                    .Where(entry => !knowAboutRankOrColorDict[entry.Key])
                    .Max(entry => entry.Value);

                cardsToDiscard = discardEstimates
                    .Where(entry => !knowAboutRankOrColorDict[entry.Key] && entry.Value == maxProbability)
                    .Select(entry => entry.Key.Card).ToList();

                if (cardsToDiscard.Any()) return cardsToDiscard;
            }
            

            // снова не нашли.
            // тогда ищем карту с наибольшей вероятностью

            maxProbability = discardEstimates.Max(entry => entry.Value);

            return discardEstimates
                    .Where(entry => entry.Value == maxProbability)
                    .Select(entry => entry.Key.Card)
                    .ToList();
        }
    }
}