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

                result[Nominal.One, color] = 3;
                result[Nominal.Two, color] = 2;
                result[Nominal.Three, color] = 2;
                result[Nominal.Four, color] = 2;
                result[Nominal.Five, color] = 1;
            }

            return result;
        }

        public Matrix CreateMatrix(int[,] table)
        {
            Contract.Requires(table.GetLength(0) == Nominals.Count);
            Contract.Requires(table.GetLength(1) == Colors.Count);

            Matrix result = CreateEmptyMatrix();
            for (int i = 0; i < Nominals.Count; i++)
            {
                for (int j = 0; j < Colors.Count; j++)
                {
                    result[(Nominal) i, Colors[j]] = table[i, j];
                }
            }

            return result;
        }

        public IReadOnlyList<Color> Colors
        {
            get { return new[] { Color.Blue, Color.Green, Color.Red, Color.White, Color.Yellow }; }
        }

        public IReadOnlyList<Nominal> Nominals
        {
            get { return new[] { Nominal.One, Nominal.Two, Nominal.Three, Nominal.Four, Nominal.Five, }; }
        }

        public int GetMaximumScore()
        {
            return Colors.Count * Nominals.Count;
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
