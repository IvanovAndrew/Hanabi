using System.Collections.Generic;
using Hanabi;
using NUnit.Framework;

namespace HanabiTest
{
    [TestFixture]
    public class GuessTest
    {
        private FakeGameProvider CreateGameProvider()
        {
            FakeGameProvider gameProvider = new FakeGameProvider
            {
                Colors = new List<Color> { Color.Yellow, Color.Red },
                Nominals = new List<Nominal> { Nominal.One, Nominal.Two }
            };

            gameProvider.FullDeckMatrix = gameProvider.CreateEmptyMatrix();
            gameProvider.FullDeckMatrix[Nominal.One, Color.Yellow] = 2;
            gameProvider.FullDeckMatrix[Nominal.Two, Color.Yellow] = 2;
            gameProvider.FullDeckMatrix[Nominal.One, Color.Red] = 2;
            gameProvider.FullDeckMatrix[Nominal.Two, Color.Red] = 2;

            return gameProvider;
        }


        [Test]
        public void GetProbability_ExcludeAllRedOneCards_ReturnsZeroForRedOneCard()
        {
            IGameProvider gameProvider = CreateGameProvider();

            Guess guess = new Guess(gameProvider);

            List<Card> excludedCards = new List<Card>
            {
                new Card(Color.Red, Nominal.One),
                new Card(Color.Red, Nominal.One),
            };

            List<Card> cardsToSearch = new List<Card>{new Card(Color.Red, Nominal.One)};

            double result = guess.GetProbability(cardsToSearch, excludedCards);

            Assert.AreEqual(0, result);
        }

        [Test]
        public void GetProbability_ExcludeAllCardsExceptYellowTwo_ReturnsOneForYellowTwo()
        {
            IGameProvider gameProvider = CreateGameProvider();

            Guess guess = new Guess(gameProvider);

            List<Card> cardsToExclude = new List<Card>
            {
                new Card(Color.Red, Nominal.One),
                new Card(Color.Red, Nominal.One),
                new Card(Color.Red, Nominal.Two),
                new Card(Color.Red, Nominal.Two),
                new Card(Color.Yellow, Nominal.One),
                new Card(Color.Yellow, Nominal.One)
            };

            List<Card> cardsToSearch = new List<Card> { new Card(Color.Yellow, Nominal.Two) };

            double result = guess.GetProbability(cardsToSearch, cardsToExclude);

            Assert.AreEqual(1.0, result);
        }

        [Test]
        public void KnowAllAboutNominalAndColor_ClueAboutRedAndClueAboutTwo_ReturnsTrue()
        {
            IGameProvider gameProvider = CreateGameProvider();

            Guess guess = new Guess(gameProvider);

            guess.Visit(new ClueAboutColor(Color.Red));
            guess.Visit(new ClueAboutNominal(Nominal.Two));

            bool result = guess.KnowAboutNominalAndColor();

            Assert.IsTrue(result);
        }

        [Test]
        public void KnowAllAboutNominalAndColor_ClueAboutColorOnly_ReturnsFalse()
        {
            IGameProvider gameProvider = CreateGameProvider();

            Guess guess = new Guess(gameProvider);
            guess.Visit(new ClueAboutColor(Color.Red));

            bool result = guess.KnowAboutNominalAndColor();

            Assert.IsFalse(result);
        }

        [Test]
        public void KnowAllAboutNominalAndColor_ClueAboutNominalOnly_ReturnsFalse()
        {
            IGameProvider gameProvider = CreateGameProvider();

            Guess guess = new Guess(gameProvider);
            guess.Visit(new ClueAboutNominal(Nominal.Two));

            bool result = guess.KnowAboutNominalAndColor();

            Assert.IsFalse(result);
        }

        [Test]
        public void KnowAboutNominalOrColor_ClueAboutRedAndClueAboutTwo_ReturnsTrue()
        {
            IGameProvider gameProvider = CreateGameProvider();

            Guess guess = new Guess(gameProvider);

            guess.Visit(new ClueAboutColor(Color.Red));
            guess.Visit(new ClueAboutNominal(Nominal.Two));

            bool result = guess.KnowAboutNominalOrColor();

            Assert.IsTrue(result);
        }

        [Test]
        public void KnowAboutNominalOrColor_ClueAboutColorOnly_ReturnsTrue()
        {
            IGameProvider gameProvider = CreateGameProvider();

            Guess guess = new Guess(gameProvider);
            guess.Visit(new ClueAboutColor(Color.Red));

            bool result = guess.KnowAboutNominalOrColor();

            Assert.IsTrue(result);
        }

        [Test]
        public void KnowAboutNominalOrColor_ClueAboutNominalOnly_ReturnsTrue()
        {
            IGameProvider gameProvider = CreateGameProvider();

            Guess guess = new Guess(gameProvider);
            guess.Visit(new ClueAboutNominal(Nominal.Two));

            bool result = guess.KnowAboutNominalOrColor();

            Assert.IsTrue(result);
        }

        [Test]
        public void KnowAboutNominalOrColor_Default_ReturnsFalse()
        {
            IGameProvider gameProvider = CreateGameProvider();

            Guess guess = new Guess(gameProvider);

            bool result = guess.KnowAboutNominalOrColor();

            Assert.IsFalse(result);
        }

        [Test]
        public void KnowAboutNominalOrColor_NotAnyColorsExceptRed_ReturnsTrue()
        {
            IGameProvider gameProvider = CreateGameProvider();

            Guess guess = new Guess(gameProvider);
            guess.Visit(new ClueAboutNotColor(Color.Yellow));

            bool result = guess.KnowAboutNominalOrColor();

            Assert.IsTrue(result);
        }
    }
}