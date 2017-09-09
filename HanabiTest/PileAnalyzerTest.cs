using System.Collections.Generic;
using System.Linq;
using Hanabi;
using NUnit.Framework;

namespace HanabiTest
{
    [TestFixture]
    public class PileAnalyzerTest
    {
        #region GetUniqueCards

        [Test]
        public void GetUniqueCards_DiscardBlueTwoCard_ContainsBlueTwoCard()
        {
            FakeGameProvider gameProvider = new FakeGameProvider
            {
                Colors = new List<Color> {Color.Blue},
                Numbers = new List<Number> {Number.One, Number.Two, Number.Three}
            };

            gameProvider.FullDeckMatrix = gameProvider.CreateEmptyMatrix();
            gameProvider.FullDeckMatrix[Number.One, Color.Blue] = 3;
            gameProvider.FullDeckMatrix[Number.Two, Color.Blue] = 2;
            gameProvider.FullDeckMatrix[Number.Three, Color.Blue] = 2;


            FireworkPile fireworkPile = new FireworkPile(gameProvider);
            DiscardPile discardPile = new DiscardPile(gameProvider);

            Card blueTwoCard = new Card(Color.Blue, Number.Two);
            discardPile.AddCard(blueTwoCard);

            var pileAnalyzer = new PilesAnalyzer(gameProvider);

            IReadOnlyList<Card> actual = pileAnalyzer.GetUniqueCards(fireworkPile, discardPile);

            Assert.IsTrue(actual.Contains(new Card(Color.Blue, Number.Two)));
        }

        [Test]
        public void GetUniqueCards_DiscardAllYellowOneCards_DoesNotContainYellowCards()
        {
            FakeGameProvider provider = new FakeGameProvider();
            provider.Colors = new List<Color> {Color.Yellow};
            provider.Numbers = new List<Number> {Number.One, Number.Two, Number.Three, Number.Four, Number.Five};

            provider.FullDeckMatrix = provider.CreateEmptyMatrix();
            provider.FullDeckMatrix[Number.One, Color.Yellow] = 3;
            provider.FullDeckMatrix[Number.Two, Color.Yellow] = 2;
            provider.FullDeckMatrix[Number.Three, Color.Yellow] = 2;
            provider.FullDeckMatrix[Number.Four, Color.Yellow] = 2;
            provider.FullDeckMatrix[Number.Five, Color.Yellow] = 1;

            var fireworkPile = new FireworkPile(provider);
            var discardPile = new DiscardPile(provider);
            discardPile.AddCard(new Card(Color.Yellow, Number.One));
            discardPile.AddCard(new Card(Color.Yellow, Number.One));
            discardPile.AddCard(new Card(Color.Yellow, Number.One));

            GameProvider gameProvider = new GameProvider();
            var pileAnalyzer = new PilesAnalyzer(gameProvider);

            IReadOnlyList<Card> actual = pileAnalyzer.GetUniqueCards(fireworkPile, discardPile);

            Assert.IsFalse(actual.Any(card => card.Color == Color.Yellow));
        }

        [Test]
        public void GetUniqueCards_PlayRedOneAndDiscardRedOne_DoesNotContainRedOne()
        {
            FakeGameProvider provider = new FakeGameProvider
            {
                Colors = new List<Color> {Color.Red},
                Numbers = new List<Number> {Number.One, Number.Two}
            };

            provider.FullDeckMatrix = provider.CreateEmptyMatrix();
            provider.FullDeckMatrix[Number.One, Color.Red] = 3;
            provider.FullDeckMatrix[Number.Two, Color.Red] = 2;

            var fireworkPile = new FireworkPile(provider);

            fireworkPile.AddCard(new Card(Color.Red, Number.One));

            var discardedPile = new DiscardPile(provider);
            discardedPile.AddCard(new Card(Color.Red, Number.One));

            var pileAnalyzer = new PilesAnalyzer(provider);

            IReadOnlyList<Card> actual = pileAnalyzer.GetUniqueCards(fireworkPile, discardedPile);

            Assert.IsFalse(actual.Contains(new Card(Color.Red, Number.One)));
        }

        #endregion
        
        #region GetThrownCards method tests

        [Test]
        public void GetThrownCards_EmptyPiles_ReturnsEmptyList()
        {
            IGameProvider gameProvider = new FakeGameProvider()
            {
                Colors = new List<Color> {Color.Blue},
                Numbers = new List<Number> {Number.One},
            };

            var pileAnalyzer = new PilesAnalyzer(gameProvider);

            var fireworkPile = new FireworkPile(gameProvider);
            var discardPile = new DiscardPile(gameProvider);

            IReadOnlyList<Card> actual = pileAnalyzer.GetThrownCards(fireworkPile, discardPile);

            Assert.IsEmpty(actual);
        }

        [Test]
        public void GetThrownCards_FireworkPileWithBlueOneAndDiscardPileWithBlueOne_ReturnsListWith2Elements()
        {
            //// arrange

            // create fake deck that contains blue one cards only
            FakeGameProvider provider = new FakeGameProvider();
            provider.Colors = new List<Color> { Color.Blue };
            provider.Numbers = new List<Number> { Number.One};

            // play blue one card
            var fireworkPile = new FireworkPile(provider);
            fireworkPile.AddCard(new Card(Color.Blue, Number.One));

            // discard blue one card
            var discardPile = new DiscardPile(provider);
            discardPile.AddCard(new Card(Color.Blue, Number.One));

            GameProvider gameProvider = new GameProvider();
            var pileAnalyzer = new PilesAnalyzer(gameProvider);

            //// act
            IReadOnlyList<Card> actual = pileAnalyzer.GetThrownCards(fireworkPile, discardPile);

            Assert.AreEqual(2, actual.Count);
        }

        [Test]
        public void GetThrownCards_FireworkPileWithBlueOneAndDiscardPileWithBlueOne_ReturnsListWithBlueOnesOnly()
        {
            //// arrange

            FakeGameProvider provider = new FakeGameProvider();
            provider.Colors = new List<Color> { Color.Blue, Color.Red };
            provider.Numbers = new List<Number> { Number.One, Number.Two };

            // play blue one card
            var fireworkPile = new FireworkPile(provider);
            fireworkPile.AddCard(new Card(Color.Blue, Number.One));

            // discard blue one card
            var discardPile = new DiscardPile(provider);
            discardPile.AddCard(new Card(Color.Blue, Number.One));

            GameProvider gameProvider = new GameProvider();
            var pileAnalyzer = new PilesAnalyzer(gameProvider);

            //// act
            IReadOnlyList<Card> actual = pileAnalyzer.GetThrownCards(fireworkPile, discardPile);

            Assert.IsTrue(actual.Count > 0 &&
                          actual.All(card => Equals(card, new Card(Color.Blue, Number.One))));
        }

        #endregion

        #region GetCardsWhateverToPlay method tests

        [Test]
        public void GetCardsWhateverToPlay_EmptyPiles_ReturnsAllUniqueCards()
        {
            FakeGameProvider provider = new FakeGameProvider
            {
                Colors = new List<Color> { Color.White, Color.Red },
                Numbers = new List<Number> { Number.One, Number.Two }
            };

            provider.FullDeckMatrix = provider.CreateEmptyMatrix();
            provider.FullDeckMatrix[Number.One, Color.White] = 3;
            provider.FullDeckMatrix[Number.One, Color.Red] = 3;
            provider.FullDeckMatrix[Number.Two, Color.White] = 2;
            provider.FullDeckMatrix[Number.Two, Color.Red] = 2;

            var fireworkPile = new FireworkPile(provider);
            var discardPile = new DiscardPile(provider);

            var pileAnalyzer = new PilesAnalyzer(provider);

            // act
            IReadOnlyList<Card> actual = pileAnalyzer.GetCardsWhateverToPlay(fireworkPile, discardPile);
            
            // assert
            var actualMatrix = new CardsToMatrixConverter(provider).Encode(actual);

            var expectedMatrix = provider.CreateEmptyMatrix();
            expectedMatrix[Number.One, Color.White] = 1;
            expectedMatrix[Number.One, Color.Red] = 1;
            expectedMatrix[Number.Two, Color.White] = 1;
            expectedMatrix[Number.Two, Color.Red] = 1;

            TestHelper.AreMatrixEqual(expectedMatrix, actualMatrix, provider);
        }

        [Test]
        public void GetCardsWhateverToPlay_DiscardedAllWhiteOne_DoesNotContainWhiteCards()
        {
            FakeGameProvider provider = new FakeGameProvider
            {
                Colors = new List<Color> { Color.White, Color.Red },
                Numbers = new List<Number> { Number.One, Number.Two, Number.Three }
            };

            provider.FullDeckMatrix = provider.CreateEmptyMatrix();
            provider.FullDeckMatrix[Number.One, Color.White] = 3;
            provider.FullDeckMatrix[Number.One, Color.Red] = 3;
            provider.FullDeckMatrix[Number.Two, Color.White] = 2;
            provider.FullDeckMatrix[Number.Two, Color.Red] = 3;
            provider.FullDeckMatrix[Number.Three, Color.White] = 2;
            provider.FullDeckMatrix[Number.Three, Color.Red] = 2;
            
            var fireworkPile = new FireworkPile(provider);
            var discardPile = new DiscardPile(provider);
            discardPile.AddCard(new Card(Color.White, Number.One));
            discardPile.AddCard(new Card(Color.White, Number.One));
            discardPile.AddCard(new Card(Color.White, Number.One));

            GameProvider gameProvider = new GameProvider();
            var pileAnalyzer = new PilesAnalyzer(gameProvider);

            IReadOnlyList<Card> actual = pileAnalyzer.GetCardsWhateverToPlay(fireworkPile, discardPile);

            Assert.IsTrue(actual.All(card => card.Color != Color.White));
        }

        #endregion
    }
}

