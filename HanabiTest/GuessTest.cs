using System.Collections.Generic;
using Hanabi;
using NUnit.Framework;

namespace HanabiTest
{
    [TestFixture]
    public class GuessTest
    {
        [Test]
        public void GetProbability_ExcludeAllBlueOneCards_ReturnsZeroForBlueOneCard()
        {
            FakeGameProvider gameProvider = new FakeGameProvider
            {
                Colors = new List<Color> {Color.Blue, Color.Green},
                Numbers = new List<Number> {Number.One, Number.Two}
            };

            gameProvider.FullDeckMatrix = gameProvider.CreateEmptyMatrix();
            gameProvider.FullDeckMatrix[Number.One, Color.Blue] = 2;
            gameProvider.FullDeckMatrix[Number.Two, Color.Blue] = 2;
            gameProvider.FullDeckMatrix[Number.One, Color.Green] = 3;
            gameProvider.FullDeckMatrix[Number.Two, Color.Green] = 4;

            Guess guess = new Guess(gameProvider);

            List<Card> excludedCards = new List<Card>
            {
                new Card(Color.Blue, Number.One),
                new Card(Color.Blue, Number.One),
            };

            List<Card> cardsToSearch = new List<Card>{new Card(Color.Blue, Number.One)};

            double result = guess.GetProbability(cardsToSearch, excludedCards);

            Assert.AreEqual(0, result);
        }

        [Test]
        public void GetProbability_ExcludeAllCardsWithNominalLessThan5_ReturnsPoint2ForRed5()
        {
            FakeGameProvider gameProvider =
                new FakeGameProvider
                {
                    Colors = new List<Color> {Color.Yellow, Color.Blue, Color.Green, Color.Red, Color.White},
                    Numbers = new List<Number> {Number.One, Number.Two, Number.Three, Number.Four, Number.Five}
                };

            gameProvider.FullDeckMatrix = gameProvider.CreateEmptyMatrix();
            gameProvider.FullDeckMatrix[Number.Five, Color.Blue] = 1;
            gameProvider.FullDeckMatrix[Number.Five, Color.Green] = 1;
            gameProvider.FullDeckMatrix[Number.Five, Color.Red] = 1;
            gameProvider.FullDeckMatrix[Number.Five, Color.White] = 1;
            gameProvider.FullDeckMatrix[Number.Five, Color.Yellow] = 1;


            Guess guess = new Guess(gameProvider);

            List<Card> excludedCards = new List<Card>();

            List<Card> cardsToSearch = new List<Card> { new Card(Color.Red, Number.Five) };

            double result = guess.GetProbability(cardsToSearch, excludedCards);

            Assert.AreEqual(0.2, result);
        }

        [Test]
        public void GetProbability_ExcludeAllCardsExceptYellowTwo_ReturnsOneForYellowTwo()
        {
            FakeGameProvider gameProvider = new FakeGameProvider
            {
                Colors = new List<Color> {Color.Yellow, Color.Red},
                Numbers = new List<Number> {Number.One, Number.Two}
            };

            gameProvider.FullDeckMatrix = gameProvider.CreateEmptyMatrix();
            gameProvider.FullDeckMatrix[Number.One, Color.Yellow] = 2;
            gameProvider.FullDeckMatrix[Number.Two, Color.Yellow] = 2;
            gameProvider.FullDeckMatrix[Number.One, Color.Red] = 2;
            gameProvider.FullDeckMatrix[Number.Two, Color.Red] = 2;

            Guess guess = new Guess(gameProvider);

            List<Card> cardsToExclude = new List<Card>
            {
                new Card(Color.Red, Number.One),
                new Card(Color.Red, Number.One),
                new Card(Color.Red, Number.Two),
                new Card(Color.Red, Number.Two),
                new Card(Color.Yellow, Number.One),
                new Card(Color.Yellow, Number.One)
            };

            List<Card> cardsToSearch = new List<Card> { new Card(Color.Yellow, Number.Two) };

            double result = guess.GetProbability(cardsToSearch, cardsToExclude);

            Assert.AreEqual(1.0, result);
        }
    }
}
