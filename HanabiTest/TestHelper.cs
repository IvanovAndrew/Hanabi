using System.Collections.Generic;
using Hanabi;
using NUnit.Framework;

namespace HanabiTest
{
    public class FakeGameProvider : IGameProvider
    {
        public Matrix FullDeckMatrix { get; set; }
        
        public Matrix CreateEmptyMatrix()
        {
            return new Matrix(this);
        }

        public Matrix CreateFullDeckMatrix()
        {
            return FullDeckMatrix;
        }

        public IReadOnlyList<Color> Colors { get; set; }
        public IReadOnlyList<Nominal> Nominals { get; set; }
        
        public int ColorToInt(Color color)
        {
            for (int i = 0; i < Colors.Count; i++)
            {
                if (color == Colors[i]) return i;
            }
            return -1;
        }

        public int GetMaximumScore()
        {
            return Nominals.Count * Colors.Count;
        }
    }

    public static class TestHelper
    {
        public static void AreMatrixEqual(Matrix expected, Matrix actual, IGameProvider provider)
        {
            foreach (var number in provider.Nominals)
            {
                foreach (var color in provider.Colors)
                {
                    Assert.AreEqual(expected[number, color], actual[number, color]);
                }
            }

            Assert.Pass();
        }
    }
}
