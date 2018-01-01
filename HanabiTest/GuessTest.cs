using System.Collections.Generic;
using Hanabi;
using NUnit.Framework;

namespace HanabiTest
{
    [TestFixture]
    public class GuessTest
    {
        private CardInHand CreateCardInHand(IGameProvider gameProvider, Card card)
        {
            var game = new Game(gameProvider, 2);
            return 
                new CardInHand(new Player(game), card);
        }

        [Test]
        public void GetProbability_ExcludeAllRedTwoCards_ReturnsZeroForRedTwoCard()
        {
            FakeGameProvider gameProvider = GameProviderFabric.Create(Color.Red);

            Guess guess = new Guess(gameProvider, 
                                CreateCardInHand(gameProvider, new Card(Color.Red, Rank.Three)));

            List<Card> excludedCards = new List<Card>
            {
                new Card(Color.Red, Rank.Two),
                new Card(Color.Red, Rank.Two),
            };

            List<Card> cardsToSearch = new List<Card>{new Card(Color.Red, Rank.Two)};

            Probability result = guess.GetProbability(cardsToSearch, excludedCards);

            Assert.AreEqual(0, result.Value);
        }

        [Test]
        public void GetProbability_ExcludeAllCardsExceptYellowTwo_ReturnsOneForYellowTwo()
        {
            IGameProvider gameProvider = GameProviderFabric.Create(new List<Color>{Color.Red, Color.Yellow});

            Guess guess = new Guess(gameProvider,
                CreateCardInHand(gameProvider, new Card(Color.Yellow, Rank.Two)));

            List<Card> cardsToExclude = new List<Card>
            {
                new Card(Color.Red, Rank.One),
                new Card(Color.Red, Rank.One),
                new Card(Color.Red, Rank.One),
                new Card(Color.Red, Rank.Two),
                new Card(Color.Red, Rank.Two),
                new Card(Color.Red, Rank.Three),
                new Card(Color.Red, Rank.Three),
                new Card(Color.Red, Rank.Four),
                new Card(Color.Red, Rank.Four),
                new Card(Color.Red, Rank.Five),
                new Card(Color.Yellow, Rank.One),
                new Card(Color.Yellow, Rank.One),
                new Card(Color.Yellow, Rank.One),
                // miss yellow two.
                new Card(Color.Yellow, Rank.Three),
                new Card(Color.Yellow, Rank.Three),
                new Card(Color.Yellow, Rank.Four),
                new Card(Color.Yellow, Rank.Four),
                new Card(Color.Yellow, Rank.Five),
            };

            List<Card> cardsToSearch = new List<Card> { new Card(Color.Yellow, Rank.Two) };

            Probability result = guess.GetProbability(cardsToSearch, cardsToExclude);

            Assert.AreEqual(1.0, result.Value);
        }

        [Test]
        public void GetProbability_EmptyCardsToSearch_ReturnsZero()
        {
            FakeGameProvider gameProvider = GameProviderFabric.Create(Color.Red);

            Guess guess = new Guess(gameProvider,
                CreateCardInHand(gameProvider, new Card(Color.Red, Rank.One)));

            List<Card> excludedCards = new List<Card>();
            List<Card> cardsToSearch = new List<Card>();

            Probability result = guess.GetProbability(cardsToSearch, excludedCards);

            Assert.AreEqual(0, result.Value);
        }

        [Test]
        public void KnowAllAboutNominalAndColor_ClueAboutRedAndClueAboutTwo_ReturnsTrue()
        {
            IGameProvider gameProvider = GameProviderFabric.Create(Color.Red);

            var redTwoCardInHand = CreateCardInHand(gameProvider, new Card(Color.Red, Rank.Two));
            var cardsToClue = new[] {redTwoCardInHand};
            Guess guess = new Guess(gameProvider, redTwoCardInHand);

            guess.Visit(new ClueAboutColor(Color.Red));
            guess.Visit(new ClueAboutRank(Rank.Two));

            bool result = guess.KnowAboutNominalAndColor();

            Assert.IsTrue(result);
        }

        [Test]
        public void KnowAllAboutNominalAndColor_ClueAboutColorOnly_ReturnsFalse()
        {
            IGameProvider gameProvider = GameProviderFabric.Create(new List<Color>{Color.Red, Color.Yellow});

            var redTwoCardInHand = CreateCardInHand(gameProvider, new Card(Color.Red, Rank.Two));
            var cardsToClue = new[] {redTwoCardInHand};

            Guess guess = new Guess(gameProvider, redTwoCardInHand);
            guess.Visit(new ClueAboutColor(Color.Red));

            bool result = guess.KnowAboutNominalAndColor();

            Assert.IsFalse(result);
        }

        [Test]
        public void KnowAllAboutNominalAndColor_ClueAboutNominalOnly_ReturnsFalse()
        {
            IGameProvider gameProvider = GameProviderFabric.Create(Color.Red, Color.Yellow);

            var redTwoCardInHand = CreateCardInHand(gameProvider, new Card(Color.Red, Rank.Two));
            var cardsToClue = new[] {redTwoCardInHand};

            Guess guess = new Guess(gameProvider, redTwoCardInHand);
            guess.Visit(new ClueAboutRank(Rank.Two));

            bool result = guess.KnowAboutNominalAndColor();

            Assert.IsFalse(result);
        }

        [Test]
        public void KnowAboutNominalOrColor_ClueAboutRedAndClueAboutTwo_ReturnsTrue()
        {
            IGameProvider gameProvider = GameProviderFabric.Create(Color.Red, Color.Yellow);

            var redTwoCardInHand = CreateCardInHand(gameProvider, new Card(Color.Red, Rank.Two));
            var cardToClue = new[] {redTwoCardInHand};

            Guess guess = new Guess(gameProvider, redTwoCardInHand);

            guess.Visit(new ClueAboutColor(Color.Red));
            guess.Visit(new ClueAboutRank(Rank.Two));

            bool result = guess.KnowAboutRankOrColor();

            Assert.IsTrue(result);
        }

        [Test]
        public void KnowAboutNominalOrColor_ClueAboutColorOnly_ReturnsTrue()
        {
            IGameProvider gameProvider = GameProviderFabric.Create(Color.Red, Color.Yellow);

            var redTwoCardInHand = CreateCardInHand(gameProvider, new Card(Color.Red, Rank.Two));
            var cardsToClue = new[] { redTwoCardInHand };

            Guess guess = new Guess(gameProvider, redTwoCardInHand);
            guess.Visit(new ClueAboutColor(Color.Red));

            bool result = guess.KnowAboutRankOrColor();

            Assert.IsTrue(result);
        }

        [Test]
        public void KnowAboutNominalOrColor_ClueAboutNominalOnly_ReturnsTrue()
        {
            IGameProvider gameProvider = GameProviderFabric.Create(Color.Red, Color.Yellow);

            var redTwoCardInHand = CreateCardInHand(gameProvider, new Card(Color.Red, Rank.Two));
            var cardsToClue = new[] {redTwoCardInHand};

            Guess guess = new Guess(gameProvider, redTwoCardInHand);
            guess.Visit(new ClueAboutRank(Rank.Two));

            bool result = guess.KnowAboutRankOrColor();

            Assert.IsTrue(result);
        }

        [Test]
        public void KnowAboutNominalOrColor_Default_ReturnsFalse()
        {
            IGameProvider gameProvider = GameProviderFabric.Create(Color.Red, Color.Yellow);

            Guess guess = new Guess(
                gameProvider,
                CreateCardInHand(gameProvider, new Card(Color.Red, Rank.Two)));

            bool result = guess.KnowAboutRankOrColor();

            Assert.IsFalse(result);
        }

        [Test]
        public void KnowAboutNominalOrColor_NotAnyColorsExceptRed_ReturnsTrue()
        {
            IGameProvider gameProvider = GameProviderFabric.Create(Color.Red, Color.Yellow);

            var redTwoCardInHand = CreateCardInHand(gameProvider, new Card(Color.Red, Rank.Two));
            var cardsToClue = new[] {redTwoCardInHand};

            Guess guess = new Guess(gameProvider, redTwoCardInHand);
            guess.Visit(new ClueAboutNotColor(Color.Yellow));

            bool result = guess.KnowAboutRankOrColor();

            Assert.IsTrue(result);
        }
    }
}