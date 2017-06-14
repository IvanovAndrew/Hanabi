using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Hanabi
{
    public class Guess
    {
        const int NumberCount = 5;
        const int ColorCount = 5;
        
        public int[,] Matrix;
        private readonly int[,] _Defaultmatrix;

        private Guess()
        {
            _Defaultmatrix = new int[NumberCount, ColorCount]
            {
                {3,3,3,3,3},
                {2,2,2,2,2},
                {2,2,2,2,2},
                {2,2,2,2,2},
                {1,1,1,1,1},
            };

            Matrix = _Defaultmatrix;
        }

        public static Guess Create(bool isSpecialGame = false)
        {
            Contract.Ensures(Contract.Result<Guess>() != null);

            return new Guess();
        }

        public void NumberIs(Number value)
        {
            int row = (int)value;

            for(int n = 0; n < NumberCount; n++)
            {
                if (n == row) continue;

                for(int c = 0; c < ColorCount; c++)
                {
                    Matrix[n, c] = 0;
                }
            }
        }

        public void NumberIsNot(Number value)
        {
            int row = (int)value;

            for (int c = 0; c < NumberCount; c++)
            {
                Matrix[row, c] = 0;
            }
        }

        public void ColorIs(Color color)
        {
            int column = (int)color;

            for (int c = 0; c < ColorCount; c++)
            {
                if (c == column) continue;

                for (int row = 0; row < NumberCount; row++)
                {
                    Matrix[row, c] = 0;
                }
            }
        }

        public void ColorIsNot(Color color)
        {
            int column = (int)color;

            for(int i = 0; i < NumberCount; i++)
            {
                Matrix[i, column] = 0;
            }
        }

        public double GetProbability(IEnumerable<Card> cardsToSearch, IEnumerable<Card> thrown, IEnumerable<Card> otherPlayersCards)
        {
            Contract.Requires(cardsToSearch.Any());
            Contract.Requires(otherPlayersCards.Any());
            
            Contract.Ensures(0.0 <= Contract.Result<double>());
            Contract.Ensures(Contract.Result<double>() <= 1.0);
            
            int[,] thrownMatrix = CardsToMatrixConverter.Encode(thrown);
            int[,] otherPlayersCardsMatrix = CardsToMatrixConverter.Encode(otherPlayersCards);

            int[,] guessMatrix = GetCorrectedGuess(thrownMatrix, otherPlayersCardsMatrix);

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

        private int[,] GetCorrectedGuess(int[,] thrown, int[,] otherPlayers)
        {
            int[,] situation = new int[NumberCount, ColorCount];

            for (int i = 0; i < NumberCount; i++)
            {
                for (int j = 0; j < ColorCount; j++)
                {
                    situation[i, j] = Math.Max(Matrix[i, j] - (thrown[i, j] + otherPlayers[i, j]), 0);
                }
            }

            return situation;
        }

        public bool KnowAboutNominalAndColor()
        {
            return CardsToMatrixConverter.Decode(Matrix).Count == 1;
        }
    }
}