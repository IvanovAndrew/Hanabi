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

        public Guess(IGameProvider provider)
        {
            _provider = provider;
            _converter = new CardsToMatrixConverter(provider);
            _matrix = provider.CreateFullDeckMatrix();
        }

        public bool Visit(IsColor clue)
        {
            foreach (var color in _provider.Colors)
            {
                if (color == clue.Color) continue;

                foreach (Number number in _provider.Numbers)
                {
                    _matrix[number, color] = 0;
                }
            }

            return true;
        }

        public bool Visit(IsNotColor clue)
        {
            foreach (var number in _provider.Numbers)
            {
                _matrix[number, clue.Color] = 0;
            }
            
            return true;
        }

        public bool Visit(IsNominal clue)
        {
            foreach (var number in _provider.Numbers)
            {
                if (clue.Nominal == number) continue;

                foreach (var color in _provider.Colors)
                {
                    _matrix[number, color] = 0;
                }
            }

            return true;
        }

        public bool Visit(IsNotNominal clue)
        {
            foreach (var color in _provider.Colors)
            {
                _matrix[clue.Nominal, color] = 0;
            }

            return true;
        }

        /// <summary>
        /// P (card in {cardsToSearch})
        /// </summary>
        /// <param name="cardsToSearch"></param>
        /// <param name="excludedCards"></param>
        /// <returns></returns>
        public double GetProbability(IEnumerable<Card> cardsToSearch, IEnumerable<Card> excludedCards)
        {
            Contract.Requires<ArgumentNullException>(cardsToSearch != null);
            Contract.Requires<ArgumentException>(cardsToSearch.Any());

            Contract.Requires<ArgumentNullException>(excludedCards != null);

            Contract.Ensures(0.0 <= Contract.Result<double>());
            Contract.Ensures(Contract.Result<double>() <= 1.0);

            Matrix excludedCardsMatrix = _converter.Encode(excludedCards);

            Matrix guessMatrix = GetCorrectedGuess(excludedCardsMatrix);

            int positiveWays =
                cardsToSearch.Sum(card => guessMatrix[card.Nominal, card.Color]);

            int allWays = 0;
            foreach (Number number in _provider.Numbers)
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

            foreach (var number in _provider.Numbers)
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
    }
}