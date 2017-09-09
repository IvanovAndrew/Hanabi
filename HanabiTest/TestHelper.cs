using System.Collections.Generic;
using System.Linq;
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

    public static class GameProviderFabric
    {
        public static FakeGameProvider Create(Color color)
        {
            return Create(new List<Color> {color}.AsReadOnly());
        }

        public static FakeGameProvider Create(Color one, Color two)
        {
            return Create(new List<Color> {one, two});
        }

        public static FakeGameProvider Create(IEnumerable<Color> colors)
        {
            FakeGameProvider gameProvider = new FakeGameProvider
            {
                Colors = colors.ToList(),
                Nominals = new List<Nominal> { Nominal.One, Nominal.Two, Nominal.Three, Nominal.Four, Nominal.Five }
            };

            gameProvider.FullDeckMatrix = gameProvider.CreateEmptyMatrix();

            foreach (var color in colors)
            {
                gameProvider.FullDeckMatrix[Nominal.One, color] = 3;
                gameProvider.FullDeckMatrix[Nominal.Two, color] = 2;
                gameProvider.FullDeckMatrix[Nominal.Three, color] = 2;
                gameProvider.FullDeckMatrix[Nominal.Four, color] = 2;
                gameProvider.FullDeckMatrix[Nominal.Five, color] = 1;
            }

            return gameProvider;
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
