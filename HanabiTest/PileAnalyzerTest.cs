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
            IGameProvider gameProvider = GameProviderFabric.Create(Color.Blue);

            FireworkPile fireworkPile = new FireworkPile(gameProvider);
            DiscardPile discardPile = new DiscardPile(gameProvider);

            Card blueTwoCard = new Card(Color.Blue, Rank.Two);
            discardPile.AddCard(blueTwoCard);

            var pileAnalyzer = new PilesAnalyzer(gameProvider);

            IReadOnlyList<Card> actual = pileAnalyzer.GetUniqueCards(fireworkPile, discardPile);

            Assert.IsTrue(actual.Contains(new Card(Color.Blue, Rank.Two)));
        }

        [Test]
        public void GetUniqueCards_DiscardAllYellowOneCards_DoesNotContainYellowCards()
        {
            IGameProvider provider = GameProviderFabric.Create(Color.Yellow);

            var fireworkPile = new FireworkPile(provider);
            var discardPile = new DiscardPile(provider);
            discardPile.AddCard(new Card(Color.Yellow, Rank.One));
            discardPile.AddCard(new Card(Color.Yellow, Rank.One));
            discardPile.AddCard(new Card(Color.Yellow, Rank.One));

            GameProvider gameProvider = new GameProvider();
            var pileAnalyzer = new PilesAnalyzer(gameProvider);

            IReadOnlyList<Card> actual = pileAnalyzer.GetUniqueCards(fireworkPile, discardPile);

            Assert.IsFalse(actual.Any(card => card.Color == Color.Yellow));
        }

        [Test]
        public void GetUniqueCards_PlayRedOneAndDiscardRedOne_DoesNotContainRedOne()
        {
            IGameProvider provider = GameProviderFabric.Create(Color.Red);

            var fireworkPile = new FireworkPile(provider);

            fireworkPile.AddCard(new Card(Color.Red, Rank.One));

            var discardedPile = new DiscardPile(provider);
            discardedPile.AddCard(new Card(Color.Red, Rank.One));

            var pileAnalyzer = new PilesAnalyzer(provider);

            IReadOnlyList<Card> actual = pileAnalyzer.GetUniqueCards(fireworkPile, discardedPile);

            Assert.IsFalse(actual.Contains(new Card(Color.Red, Rank.One)));
        }

        #endregion
        
        #region GetThrownCards method tests

        [Test]
        public void GetThrownCards_EmptyPiles_ReturnsEmptyList()
        {
            IGameProvider gameProvider = GameProviderFabric.Create(Color.Multicolor);

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
            FakeGameProvider provider = GameProviderFabric.Create(Color.Blue);

            // play blue one card
            var fireworkPile = new FireworkPile(provider);
            fireworkPile.AddCard(new Card(Color.Blue, Rank.One));

            // discard blue one card
            var discardPile = new DiscardPile(provider);
            discardPile.AddCard(new Card(Color.Blue, Rank.One));

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

            FakeGameProvider gameProvider =
                GameProviderFabric.Create(new List<Color> {Color.Blue, Color.Red}.AsReadOnly());

            // play blue one card
            var fireworkPile = new FireworkPile(gameProvider);
            fireworkPile.AddCard(new Card(Color.Blue, Rank.One));

            // discard blue one card
            var discardPile = new DiscardPile(gameProvider);
            discardPile.AddCard(new Card(Color.Blue, Rank.One));

            var pileAnalyzer = new PilesAnalyzer(gameProvider);

            //// act
            IReadOnlyList<Card> actual = pileAnalyzer.GetThrownCards(fireworkPile, discardPile);

            Assert.IsTrue(actual.Count > 0 &&
                          actual.All(card => Equals(card, new Card(Color.Blue, Rank.One))));
        }

        #endregion

        #region GetCardsWhateverToPlay method tests

        [Test]
        public void GetCardsWhateverToPlay_EmptyPiles_ReturnsAllUniqueCards()
        {
            IGameProvider gameProvider = 
                GameProviderFabric.Create(new List<Color> {Color.White, Color.Red});

            var fireworkPile = new FireworkPile(gameProvider);
            var discardPile = new DiscardPile(gameProvider);

            var pileAnalyzer = new PilesAnalyzer(gameProvider);

            // act
            IReadOnlyList<Card> actual = pileAnalyzer.GetCardsWhateverToPlay(fireworkPile, discardPile);
            
            // assert
            var actualMatrix = new CardsToMatrixConverter(gameProvider).Encode(actual);

            var expectedMatrix = gameProvider.CreateEmptyMatrix();
            expectedMatrix[Rank.One, Color.White] = 1;
            expectedMatrix[Rank.One, Color.Red] = 1;
            expectedMatrix[Rank.Two, Color.White] = 1;
            expectedMatrix[Rank.Two, Color.Red] = 1;
            expectedMatrix[Rank.Three, Color.White] = 1;
            expectedMatrix[Rank.Three, Color.Red] = 1;
            expectedMatrix[Rank.Four, Color.White] = 1;
            expectedMatrix[Rank.Four, Color.Red] = 1;
            expectedMatrix[Rank.Five, Color.White] = 1;
            expectedMatrix[Rank.Five, Color.Red] = 1;

            TestHelper.AreMatrixEqual(expectedMatrix, actualMatrix, gameProvider);
        }

        [Test]
        public void GetCardsWhateverToPlay_DiscardedAllWhiteOne_DoesNotContainWhiteCards()
        {
            FakeGameProvider provider = GameProviderFabric.Create(new List<Color> {Color.White, Color.Red});

            var fireworkPile = new FireworkPile(provider);
            var discardPile = new DiscardPile(provider);
            discardPile.AddCard(new Card(Color.White, Rank.One));
            discardPile.AddCard(new Card(Color.White, Rank.One));
            discardPile.AddCard(new Card(Color.White, Rank.One));

            GameProvider gameProvider = new GameProvider();
            var pileAnalyzer = new PilesAnalyzer(gameProvider);

            IReadOnlyList<Card> actual = pileAnalyzer.GetCardsWhateverToPlay(fireworkPile, discardPile);

            Assert.IsTrue(actual.All(card => card.Color != Color.White));
        }

        #endregion
    }
}

