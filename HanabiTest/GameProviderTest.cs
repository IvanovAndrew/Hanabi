using Hanabi;
using NUnit.Framework;

namespace HanabiTest
{
    [TestFixture]
    public class GameProviderTest
    {
        [Test]
        public void CreateEmptyMatrix_Default_ZeroMatrix()
        {
            IGameProvider provider = new GameProvider();
            Matrix actualMatrix = provider.CreateEmptyMatrix();

            Matrix expectedMatrix = new Matrix(provider);

            TestHelper.AreMatrixEqual(expectedMatrix, actualMatrix, provider);
        }

        [Test]
        public void CreateFullDeckMatrix_Default_ThreeCardsEachColorWithValueOne()
        {
            IGameProvider provider = new GameProvider();
            Matrix actualMatrix = provider.CreateFullDeckMatrix();

            foreach (var color in provider.Colors)
            {
                Assert.AreEqual(3, actualMatrix[Number.One, color]);
            }
        }

        [Test]
        public void CreateFullDeckMatrix_Default_TwoCardsEachColorWithValueTwo()
        {
            IGameProvider provider = new GameProvider();
            Matrix actualMatrix = provider.CreateFullDeckMatrix();

            foreach (var color in provider.Colors)
            {
                Assert.AreEqual(2, actualMatrix[Number.Two, color]);
            }
        }

        [Test]
        public void CreateFullDeckMatrix_Default_TwoCardsEachColorWithValueThree()
        {
            IGameProvider provider = new GameProvider();
            Matrix actualMatrix = provider.CreateFullDeckMatrix();

            foreach (var color in provider.Colors)
            {
                Assert.AreEqual(2, actualMatrix[Number.Three, color]);
            }
        }

        [Test]
        public void CreateFullDeckMatrix_Default_TwoCardsEachColorWithValueFour()
        {
            IGameProvider provider = new GameProvider();
            Matrix actualMatrix = provider.CreateFullDeckMatrix();

            foreach (var color in provider.Colors)
            {
                Assert.AreEqual(2, actualMatrix[Number.Four, color]);
            }
        }

        [Test]
        public void CreateFullDeckMatrix_Default_OneCardEachColorWithValueFive()
        {
            IGameProvider provider = new GameProvider();
            Matrix actualMatrix = provider.CreateFullDeckMatrix();

            foreach (var color in provider.Colors)
            {
                Assert.AreEqual(1, actualMatrix[Number.Five, color]);
            }
        }
    }
}
