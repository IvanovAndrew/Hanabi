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
        public bool Visit(ClueAboutColor clue)
        {
            foreach (var color in _provider.Colors)
            {
                if (color == clue.Color) continue;

                foreach (Rank number in _provider.Nominals)
                {
                    _matrix[number, color] = 0;
                }
            }

            return true;
        }

        public bool Visit(ClueAboutNotColor clue)
        {
            foreach (var number in _provider.Nominals)
            {
                _matrix[number, clue.Color] = 0;
            }

            return true;
        }

        public bool Visit(ClueAboutRank clue)
        {
            foreach (var number in _provider.Nominals)
            {
                if (clue.Rank == number) continue;

                foreach (var color in _provider.Colors)
                {
                    _matrix[number, color] = 0;
                }
            }

            return true;
        }

        public bool Visit(ClueAboutNotRank clue)
        {
            foreach (var color in _provider.Colors)
            {
                _matrix[clue.Rank, color] = 0;
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
        public double GetProbability(IEnumerable<Card> cardsToSearch, IEnumerable<Card> excludedCards)
        {
            Contract.Requires<ArgumentNullException>(cardsToSearch != null);
            Contract.Requires<ArgumentNullException>(excludedCards != null);

            Contract.Ensures(0.0 <= Contract.Result<double>());
            Contract.Ensures(Contract.Result<double>() <= 1.0);

            Matrix excludedCardsMatrix = _converter.Encode(excludedCards);

            Matrix guessMatrix = GetCorrectedGuess(excludedCardsMatrix);

            int positiveWays =
                cardsToSearch.Sum(card => guessMatrix[card.Rank, card.Color]);

            int allWays = 0;
            foreach (Rank number in _provider.Nominals)
            {
                foreach (var color in _provider.Colors)
                {
                    allWays += guessMatrix[number, color];
                }
            }

            return positiveWays / (double) allWays;
        }

        private Matrix GetCorrectedGuess(Matrix excludedCards)
        {
            Matrix situation = _provider.CreateEmptyMatrix();

            foreach (var number in _provider.Nominals)
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