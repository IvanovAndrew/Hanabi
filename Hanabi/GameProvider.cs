using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

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

                result[Number.One, color] = 3;
                result[Number.Two, color] = 2;
                result[Number.Three, color] = 2;
                result[Number.Four, color] = 2;
                result[Number.Five, color] = 1;
            }

            return result;
        }

        public Matrix CreateMatrix(int[,] table)
        {
            Contract.Requires(table.GetLength(0) == Numbers.Count);
            Contract.Requires(table.GetLength(1) == Colors.Count);

            Matrix result = CreateEmptyMatrix();
            for (int i = 0; i < Numbers.Count; i++)
            {
                for (int j = 0; j < Colors.Count; j++)
                {
                    result[(Number) i, Colors[j]] = table[i, j];
                }
            }

            return result;
        }

        public IReadOnlyList<Color> Colors
        {
            get { return new[] { Color.Blue, Color.Green, Color.Red, Color.White, Color.Yellow }; }
        }

        public IReadOnlyList<Number> Numbers
        {
            get { return new[] { Number.One, Number.Two, Number.Three, Number.Four, Number.Five, }; }
        }

        public int GetMaximumScore()
        {
            return Colors.Count * Numbers.Count;
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
