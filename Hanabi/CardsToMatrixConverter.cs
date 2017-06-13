using System;
using System.Collections.Generic;

namespace Hanabi
{
    public static class CardsToMatrixConverter
    {
        public static int[,] Encode(IEnumerable<Card> cards, bool isSpecial = false)
        {
            if (cards == null) throw new ArgumentNullException();

            int colorCount = isSpecial ? 6 : 5;

            int[,] result = new int[5, colorCount];

            foreach (Card card in cards)
            {
                result[(int) card.Nominal, (int) card.Color]++;
            }

            return result;
        }

        public static List<Card> Decode(int[,] matrix, bool isSpecial = false)
        {
            if (matrix == null) throw new ArgumentNullException();

            int colorCount = isSpecial ? 6 : 5;

            List<Card> result = new List<Card>();

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
