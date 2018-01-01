using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Hanabi
{
    public class Guess : IClueVisitor
    {
        private readonly IGameProvider _provider;
        private readonly Matrix _matrix;
        private readonly CardsToMatrixConverter _converter;

        public CardInHand CardInHand { get; }

        public Guess(IGameProvider provider, CardInHand cardInHand)
        {
            _provider = provider;
            _converter = new CardsToMatrixConverter(provider);
            _matrix = provider.CreateFullDeckMatrix();

            CardInHand = cardInHand;
        }

        #region IClueVisitor implementation
        public bool Visit(ClueAboutColor clueAboutColor)
        {
            foreach (var color in _provider.Colors)
            {
                if (color == clueAboutColor.Color) continue;

                foreach (Rank number in _provider.Ranks)
                {
                    _matrix[number, color] = 0;
                }
            }

            return true;
        }

        public bool Visit(ClueAboutNotColor clueAboutNotColor)
        {
            foreach (var number in _provider.Ranks)
            {
                _matrix[number, clueAboutNotColor.Color] = 0;
            }

            return true;
        }

        public bool Visit(ClueAboutRank clueAboutRank)
        {
            foreach (var number in _provider.Ranks)
            {
                if (clueAboutRank.Rank == number) continue;

                foreach (var color in _provider.Colors)
                {
                    _matrix[number, color] = 0;
                }
            }

            return true;
        }

        public bool Visit(ClueAboutNotRank clueAboutNotRank)
        {
            foreach (var color in _provider.Colors)
            {
                _matrix[clueAboutNotRank.Rank, color] = 0;
            }

            return true;
        }
        

        #endregion
        

        /// <summary>
        /// P (card in {cardsToSearch})
        /// </summary>
        /// <param name="cardsToSearch"></param>
        /// <param name="excludedCards"></param>
        /// <returns></returns>
        public Probability GetProbability(IEnumerable<Card> cardsToSearch, IEnumerable<Card> excludedCards)
        {
            Contract.Requires<ArgumentNullException>(cardsToSearch != null);
            Contract.Requires<ArgumentNullException>(excludedCards != null);

            Contract.Ensures(Contract.Result<Probability>() != null);

            Matrix excludedCardsMatrix = _converter.Encode(excludedCards);

            Matrix guessMatrix = GetCorrectedGuess(excludedCardsMatrix);

            int positiveWays =
                cardsToSearch.Sum(card => guessMatrix[card.Rank, card.Color]);

            int allWays = 0;
            foreach (Rank number in _provider.Ranks)
            {
                foreach (var color in _provider.Colors)
                {
                    allWays += guessMatrix[number, color];
                }
            }

            return new Probability(positiveWays / (double) allWays);
        }

        private Matrix GetCorrectedGuess(Matrix excludedCards)
        {
            Contract.Requires<ArgumentNullException>(excludedCards != null);
            Contract.Ensures(Contract.Result<Matrix>().Sum() > 0);

            Matrix situation = _provider.CreateEmptyMatrix();

            foreach (var number in _provider.Ranks)
            {
                foreach (var color in _provider.Colors)
                {
                    situation[number, color] = Math.Max(_matrix[number, color] - excludedCards[number, color], 0);
                }
            }
            
            return situation;
        }

        public bool KnowAboutNominalAndColor()
        {
            return _converter.Decode(_matrix)
                        .Distinct()
                        .ToList().Count == 1;
        }

        public bool KnowAboutRankOrColor()
        {
            IEnumerable<Card> cards = _converter.Decode(_matrix);
            
            bool knowAboutNominal = 
                cards
                    .Select(card => card.Rank)
                    .Distinct()
                    .ToList()
                    .Count == 1;

            bool knowAboutColor =
                cards.Select(card => card.Color)
                    .Distinct()
                    .ToList()
                    .Count == 1;

            return knowAboutNominal || knowAboutColor;
        }
    }
}