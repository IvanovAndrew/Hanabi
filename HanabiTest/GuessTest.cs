using System.Collections.Generic;
using Hanabi;
using NUnit.Framework;

namespace HanabiTest
{
    [TestFixture]
    public class GuessTest
    {
        [Test]
        public void GetProbability_ExcludeAllRedTwoCards_ReturnsZeroForRedTwoCard()
        {
            FakeGameProvider gameProvider = GameProviderFabric.Create(Color.Red);

            Guess guess = new Guess(gameProvider);

            List<Card> excludedCards = new List<Card>
            {
                new Card(Color.Red, Nominal.Two),
                new Card(Color.Red, Nominal.Two),
            };

            List<Card> cardsToSearch = new List<Card>{new Card(Color.Red, Nominal.Two)};

            double result = guess.GetProbability(cardsToSearch, excludedCards);

            Assert.AreEqual(0, result);
        }

        [Test]
        public void GetProbability_ExcludeAllCardsExceptYellowTwo_ReturnsOneForYellowTwo()
        {
            IGameProvider gameProvider = GameProviderFabric.Create(new List<Color>{Color.Red, Color.Yellow});

            Guess guess = new Guess(gameProvider);

            List<Card> cardsToExclude = new List<Card>
            {
                new Card(Color.Red, Nominal.One),
                new Card(Color.Red, Nominal.One),
                new Card(Color.Red, Nominal.One),
                new Card(Color.Red, Nominal.Two),
                new Card(Color.Red, Nominal.Two),
                new Card(Color.Red, Nominal.Three),
                new Card(Color.Red, Nominal.Three),
                new Card(Color.Red, Nominal.Four),
                new Card(Color.Red, Nominal.Four),
                new Card(Color.Red, Nominal.Five),
                new Card(Color.Yellow, Nominal.One),
                new Card(Color.Yellow, Nominal.One),
                new Card(Color.Yellow, Nominal.One),
                // miss yellow two.
                new Card(Color.Yellow, Nominal.Three),
                new Card(Color.Yellow, Nominal.Three),
                new Card(Color.Yellow, Nominal.Four),
                new Card(Color.Yellow, Nominal.Four),
                new Card(Color.Yellow, Nominal.Five),
            };

            List<Card> cardsToSearch = new List<Card> { new Card(Color.Yellow, Nominal.Two) };

            double result = guess.GetProbability(cardsToSearch, cardsToExclude);

            Assert.AreEqual(1.0, result);
        }

        [Test]
        public void KnowAllAboutNominalAndColor_ClueAboutRedAndClueAboutTwo_ReturnsTrue()
        {
            IGameProvider gameProvider = GameProviderFabric.Create(Color.Red);

            Guess guess = new Guess(gameProvider);

            guess.Visit(new ClueAboutColor(Color.Red));
            guess.Visit(new ClueAboutNominal(Nominal.Two));

            bool result = guess.KnowAboutNominalAndColor();

            Assert.IsTrue(result);
        }

        [Test]
        public void KnowAllAboutNominalAndColor_ClueAboutColorOnly_ReturnsFalse()
        {
            IGameProvider gameProvider = GameProviderFabric.Create(new List<Color>{Color.Red, Color.Yellow});

            Guess guess = new Guess(gameProvider);
            guess.Visit(new ClueAboutColor(Color.Red));

            bool result = guess.KnowAboutNominalAndColor();

            Assert.IsFalse(result);
        }

        [Test]
        public void KnowAllAboutNominalAndColor_ClueAboutNominalOnly_ReturnsFalse()
        {
            IGameProvider gameProvider = GameProviderFabric.Create(Color.Red, Color.Yellow);

            Guess guess = new Guess(gameProvider);
            guess.Visit(new ClueAboutNominal(Nominal.Two));

            bool result = guess.KnowAboutNominalAndColor();

            Assert.IsFalse(result);
        }

        [Test]
        public void KnowAboutNominalOrColor_ClueAboutRedAndClueAboutTwo_ReturnsTrue()
        {
            IGameProvider gameProvider = GameProviderFabric.Create(Color.Red, Color.Yellow);

            Guess guess = new Guess(gameProvider);

            guess.Visit(new ClueAboutColor(Color.Red));
            guess.Visit(new ClueAboutNominal(Nominal.Two));

            bool result = guess.KnowAboutNominalOrColor();

            Assert.IsTrue(result);
        }

        [Test]
        public void KnowAboutNominalOrColor_ClueAboutColorOnly_ReturnsTrue()
        {
            IGameProvider gameProvider = GameProviderFabric.Create(Color.Red, Color.Yellow);

            Guess guess = new Guess(gameProvider);
            guess.Visit(new ClueAboutColor(Color.Red));

            bool result = guess.KnowAboutNominalOrColor();

            Assert.IsTrue(result);
        }

        [Test]
        public void KnowAboutNominalOrColor_ClueAboutNominalOnly_ReturnsTrue()
        {
            IGameProvider gameProvider = GameProviderFabric.Create(Color.Red, Color.Yellow);

            Guess guess = new Guess(gameProvider);
            guess.Visit(new ClueAboutNominal(Nominal.Two));

            bool result = guess.KnowAboutNominalOrColor();

            Assert.IsTrue(result);
        }

        [Test]
        public void KnowAboutNominalOrColor_Default_ReturnsFalse()
        {
            IGameProvider gameProvider = GameProviderFabric.Create(Color.Red, Color.Yellow);

            Guess guess = new Guess(gameProvider);

            bool result = guess.KnowAboutNominalOrColor();

            Assert.IsFalse(result);
        }

        [Test]
        public void KnowAboutNominalOrColor_NotAnyColorsExceptRed_ReturnsTrue()
        {
            IGameProvider gameProvider = GameProviderFabric.Create(Color.Red, Color.Yellow);

            Guess guess = new Guess(gameProvider);
            guess.Visit(new ClueAboutNotColor(Color.Yellow));

            bool result = guess.KnowAboutNominalOrColor();

            Assert.IsTrue(result);
        }
    }
}