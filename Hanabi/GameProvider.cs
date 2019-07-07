using System;
using System.Collections.Generic;

namespace Hanabi
{
    public class GameProvider : IGameProvider
    {
        public Matrix CreateEmptyMatrix()
        {
            return new Matrix(this);
        }

        public Matrix CreateFullDeckMatrix()
        {
            Matrix result = CreateEmptyMatrix();

            foreach (var color in Colors)
            {
                if (color == Color.Multicolor) continue;

                result[Rank.One, color] = 3;
                result[Rank.Two, color] = 2;
                result[Rank.Three, color] = 2;
                result[Rank.Four, color] = 2;
                result[Rank.Five, color] = 1;
            }

            return result;
        }

        public Matrix CreateMatrix(int[,] table)
        {
            if (table.GetLength(0) != Ranks.Count) throw new ArgumentException("Dimensions mismatch");
            if (table.GetLength(1) != Colors.Count) throw new ArgumentException("Dimensions mismatch");

            Matrix result = CreateEmptyMatrix();
            for (int i = 0; i < Ranks.Count; i++)
            {
                for (int j = 0; j < Colors.Count; j++)
                {
                    result[(Rank) i, Colors[j]] = table[i, j];
                }
            }

            return result;
        }

        public IReadOnlyList<Color> Colors => 
            new[] { Color.Blue, Color.Green, Color.Red, Color.White, Color.Yellow };

        public IReadOnlyList<Rank> Ranks => 
            new[] { Rank.One, Rank.Two, Rank.Three, Rank.Four, Rank.Five, };

        public int GetMaximumScore()
        {
            return Colors.Count * Ranks.Count;
        }

        public int ColorToInt(Color color)
        {
            int i = 0;
            foreach (var c in Colors)
            {
                if (c == color)
                    return i;
                i++;
            }
            throw new InvalidOperationException("Unknown color: " + color.ToString());
        }
    }
}
