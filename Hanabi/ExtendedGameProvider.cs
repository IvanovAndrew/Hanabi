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

                result[Rank.One, color] = 3;
                result[Rank.Two, color] = 2;
                result[Rank.Three, color] = 2;
                result[Rank.Four, color] = 2;
                result[Rank.Five, color] = 1;
            }

            foreach (var number in Nominals)
            {
                result[number, Color.Multicolor] = 1;
            }
            return result;
        }

        public IReadOnlyList<Color> Colors => 
            new[] { Color.Blue, Color.Green, Color.Red, Color.White, Color.Yellow, Color.Multicolor, };

        public IReadOnlyList<Rank> Nominals => 
            new[] { Rank.One, Rank.Two, Rank.Three, Rank.Four, Rank.Five, };

        public int ColorToInt(Color color)
        {
            for (int i = 0; i < Colors.Count; i++)
            {
                if (Colors[i] == color) return i;
            }
            throw new ArgumentOutOfRangeException(nameof(color), "Unknown color");
        }

        public int GetMaximumScore()
        {
            return Colors.Count * Nominals.Count;
        }
    }
}
