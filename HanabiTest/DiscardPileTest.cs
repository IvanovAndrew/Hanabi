using Hanabi;
using NUnit.Framework;

namespace HanabiTest
{
    [TestFixture]
    public class DiscardPileTest
    {
        [Test]
        public void ToMatrix_EmptyPile_ZeroMatrix()
        {
            DiscardPile pile = new DiscardPile();
            int[,] actual = pile.ToMatrix();

            int[,] expected = new int[5, 5]
            {
                {0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0},
            };

            TestHelper.AreMatrixEqual(expected, actual);
        }

        [Test]
        public void ToMatrix_PileWithBlueOneCard_OnlyOne()
        {
            DiscardPile discardPile = new DiscardPile();
            discardPile.AddCard(new BlueCard(Number.One));

            int[,] actual = discardPile.ToMatrix();

            int[,] expected = new int[5, 5]
            {
                {1, 0, 0, 0, 0},
                {0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0},
            };
            TestHelper.AreMatrixEqual(expected, actual);
        }
    }
}
