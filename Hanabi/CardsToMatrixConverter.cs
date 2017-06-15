﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Hanabi
{
    public static class CardsToMatrixConverter
    {
        public static int[,] Encode(IEnumerable<Card> cards, bool isSpecial = false)
        {
            Contract.Requires<ArgumentNullException>(cards != null);
            Contract.Ensures(Contract.Result<int[,]>() != null);

            int colorCount = isSpecial ? 6 : 5;
            int[,] result = new int[5, colorCount];

            foreach (Card card in cards)
            {
                result[(int) card.Nominal, (int) card.Color]++;
            }

            return result;
        }

        public static IReadOnlyList<Card> Decode(int[,] matrix, bool isSpecial = false)
        {
            Contract.Requires<ArgumentNullException>(matrix != null);
            Contract.Ensures(Contract.Result<IReadOnlyList<Card>>() != null);
            
            int colorCount = isSpecial ? 6 : 5;

            var result = new List<Card>();

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < colorCount; j++)
                {
                    for (int k = 0; k < matrix[i, j]; k++)
                    {
                        result.Add(Card.CreateCard((Number)i, (Color)j));
                    }
                }
            }

            return result;
        }
    }
}
