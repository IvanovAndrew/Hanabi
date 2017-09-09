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

                result[Number.One, color] = 3;
                result[Number.Two, color] = 2;
                result[Number.Three, color] = 2;
                result[Number.Four, color] = 2;
                result[Number.Five, color] = 1;
            }

            foreach (var number in Numbers)
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
        public IReadOnlyList<Number> Numbers
        {
            get { return new[] { Number.One, Number.Two, Number.Three, Number.Four, Number.Five, }; }
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
            return Colors.Count * Numbers.Count;
        }
    }
}
