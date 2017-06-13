using System.Collections.Generic;
using System.Linq;
using Hanabi;
using NUnit.Framework;

namespace HanabiTest
{
    [TestFixture]
    public class PileTest
    {
        [Test]
        public void GetUniqueCards_EmptyDiscardPileAndEmptyFireworkPile_5UniqueCards()
        {
            IReadOnlyList<Card> actual = Pile.GetUniqueCards(new FireworkPile(), new DiscardPile());

            Assert.AreEqual(5, actual.Count);
        }

        [Test]
        public void GetUniqueCards_PlayRedOneAndDiscardRedOne_NotContainRedOne()
        {
            var fireworkPile = new FireworkPile();
            fireworkPile.AddCard(new RedCard(Number.One));
            
            var discardedPile = new DiscardPile();
            discardedPile.AddCard(new RedCard(Number.One));

            IReadOnlyList<Card> actual = Pile.GetUniqueCards(fireworkPile, discardedPile);

            Assert.IsFalse(actual.Contains(new RedCard(Number.One)));
        }

        [Test]
        public void GetThrownCards_EmptyPiles_EmptyList()
        {
            IReadOnlyList<Card> actual = Pile.GetThrownCards(new FireworkPile(), new DiscardPile());

            Assert.IsEmpty(actual);
        }

        [Test]
        public void GetThrownCards_FireworkPileWithBlueOneAndDiscardPileWithBlueOne_ListWith2Elements()
        {
            var fireworkPile = new FireworkPile();
            fireworkPile.AddCard(new BlueCard(Number.One));
            var discardPile = new DiscardPile();
            discardPile.AddCard(new BlueCard(Number.One));

            IReadOnlyList<Card> actual = Pile.GetThrownCards(fireworkPile, discardPile);

            Assert.AreEqual(2, actual.Count);
        }

        [Test]
        public void GetThrownCards_FireworkPileWithBlueOneAndDiscardPileWithBlueOne_ListWithBlueOnesOnly()
        {
            var fireworkPile = new FireworkPile();
            fireworkPile.AddCard(new BlueCard(Number.One));
            var discardPile = new DiscardPile();
            discardPile.AddCard(new BlueCard(Number.One));

            IReadOnlyList<Card> actual = Pile.GetThrownCards(fireworkPile, discardPile);

            Assert.IsTrue(actual.Count > 0 &&
                        actual.All(card => Equals(card, new BlueCard(Number.One))));
        }

        [Test]
        public void GetCardsWhateverToPlay_DiscardedAllWhiteOne_NotContainsWhiteCards()
        {
            var fireworkPile = new FireworkPile();
            var discardPile = new DiscardPile();
            discardPile.AddCard(new WhiteCard(Number.One));
            discardPile.AddCard(new WhiteCard(Number.One));
            discardPile.AddCard(new WhiteCard(Number.One));

            IReadOnlyList<Card> actual = Pile.GetCardsWhateverToPlay(fireworkPile, discardPile);

            Assert.IsTrue(actual.All(card => card.Color != Color.White));
        }

        [Test]
        public void GetCardsWhateverToPlay_DiscardedAllWhiteOne_ListWith20Elements()
        {
            var fireworkPile = new FireworkPile();
            var discardPile = new DiscardPile();
            discardPile.AddCard(new WhiteCard(Number.One));
            discardPile.AddCard(new WhiteCard(Number.One));
            discardPile.AddCard(new WhiteCard(Number.One));

            IReadOnlyList<Card> actual = Pile.GetCardsWhateverToPlay(fireworkPile, discardPile);

            Assert.AreEqual(20, actual.Count);
        }
    }
}

