using System;
using System.Collections.Generic;

namespace Hanabi
{
    public class ExtendedGameProvider : IGameProvider
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

            foreach (var number in Nominals)
            {
                result[number, Color.Multicolor] = 1;
            }
            return result;
        }

        public IReadOnlyList<Color> Colors
        {
            get
            {
                return new[] { Color.Blue, Color.Green, Color.Red, Color.White, Color.Yellow, Color.Multicolor, };
            }
        }
        public IReadOnlyList<Nominal> Nominals
        {
            get { return new[] { Nominal.One, Nominal.Two, Nominal.Three, Nominal.Four, Nominal.Five, }; }
        }

        public int ColorToInt(Color color)
        {
            for (int i = 0; i < Colors.Count; i++)
            {
                if (Colors[i] == color) return i;
            }
            throw new ArgumentOutOfRangeException("color", "Unknown color");
        }

        public int GetMaximumScore()
        {
            return Colors.Count * Nominals.Count;
        }
    }
}
