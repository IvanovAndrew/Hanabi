using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Hanabi
{
    public class Guess : IClueVisitor
    {
        const int NumberCount = 5;
        const int ColorCount = 5;
        
        public int[,] Matrix;
        private readonly int[,] _defaultmatrix;

        private Guess()
        {
            _defaultmatrix = new int[NumberCount, ColorCount]
            {
                {3,3,3,3,3},
                {2,2,2,2,2},
                {2,2,2,2,2},
                {2,2,2,2,2},
                {1,1,1,1,1},
            };

            Matrix = _defaultmatrix;
        }

        public static Guess Create(bool isSpecialGame = false)
        {
            Contract.Ensures(Contract.Result<Guess>() != null);

            return new Guess();
        }

        public void Update(IsColor clue)
        {
            int column = (int)clue.Color;

            for (int c = 0; c < ColorCount; c++)
            {
                if (c == column) continue;

                for (int row = 0; row < NumberCount; row++)
                {
                    Matrix[row, c] = 0;
                }
            }
        }

        public void Update(IsNotColor clue)
        {
            int column = (int) clue.Color;

            for (int i = 0; i < NumberCount; i++)
            {
                Matrix[i, column] = 0;
            }
        }

        public void Update(IsValue clue)
        {
            int row = (int)clue.Value;

            for (int n = 0; n < NumberCount; n++)
            {
                if (n == row) continue;

                for (int c = 0; c < ColorCount; c++)
                {
                    Matrix[n, c] = 0;
                }
            }
        }

        public void Update(IsNotValue clue)
        {
            int row = (int)clue.Value;

            for (int c = 0; c < NumberCount; c++)
            {
                Matrix[row, c] = 0;
            }
        }

        public double GetProbability(IEnumerable<Card> cardsToSearch, IEnumerable<Card> excludedCards)
        {
            Contract.Requires<ArgumentNullException>(cardsToSearch != null);
            Contract.Requires(cardsToSearch.Any());

            Contract.Requires<ArgumentNullException>(excludedCards != null);
            Contract.Requires(excludedCards.Any());

            Contract.Ensures(0.0 <= Contract.Result<double>());
            Contract.Ensures(Contract.Result<double>() <= 1.0);

            int[,] excludedCardsMatrix = CardsToMatrixConverter.Encode(excludedCards);

            int[,] guessMatrix = GetCorrectedGuess(excludedCardsMatrix);

            int positiveWays =
                cardsToSearch.Sum(card => guessMatrix[(int)card.Nominal, (int)card.Color]);

            int allWays = 0;
            for (int i = 0; i < NumberCount; i++)
            {
                for (int j = 0; j < ColorCount; j++)
                {
                    allWays += guessMatrix[i, j];
                }
            }

            return positiveWays / (double) allWays;
        }

        private int[,] GetCorrectedGuess(int[,] excludedCards)
        {
            int[,] situation = new int[NumberCount, ColorCount];

            for (int i = 0; i < NumberCount; i++)
            {
                for (int j = 0; j < ColorCount; j++)
                {
                    situation[i, j] = Math.Max(Matrix[i, j] - excludedCards[i, j], 0);
                }
            }

            return situation;
        }

        public bool KnowAboutNominalAndColor()
        {
            return CardsToMatrixConverter.Decode(Matrix)
                        .Distinct()
                        .ToList().Count == 1;
        }
    }
}