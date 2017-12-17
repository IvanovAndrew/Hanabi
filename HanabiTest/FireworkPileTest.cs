using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Hanabi;

namespace HanabiTest
{
    [TestFixture]
    public class FireworkPileTest
    {
        #region AddCard test methods

        [Test]
        public void AddCard_AddGreenOneToEmptyFireworkPile_ReturnsTrue()
        {
            IGameProvider provider = new FakeGameProvider()
            {
                Colors = new List<Color>{Color.Green, Color.Red},
                Nominals = new List<Rank> { Rank.One, Rank.Two}
            };
            var pile = new FireworkPile(provider);
            var greenOneCard = new Card(Color.Green, Rank.One);

            var added = pile.AddCard(greenOneCard);
            Assert.IsTrue(added);
        }

        [Test]
        public void AddCard_AddWhiteTwoToEmptyFireworkPile_ReturnsFalse()
        {
            IGameProvider provider = new FakeGameProvider()
            {
                Colors = new List<Color> { Color.Green, Color.Red, Color.White },
                Nominals = new List<Rank> { Rank.One, Rank.Two }
            };
            
            var pile = new FireworkPile(provider);
            var whiteTwoCard = new Card(Color.White, Rank.Two);

            var added = pile.AddCard(whiteTwoCard);
            Assert.IsFalse(added);
        }

        [Test]
        public void AddCard_AddWhiteTwoCardToWhiteFireworkWithLastTwo_ReturnsFalse()
        {
            IGameProvider provider = new FakeGameProvider()
            {
                Colors = new List<Color> { Color.Green, Color.White },
                Nominals = new List<Rank> { Rank.One, Rank.Two, Rank.Three }
            };

            var firework = new FireworkPile(provider);

            var whiteOneCard = new Card(Color.White, Rank.One);
            var whiteTwoCard = new Card(Color.White, Rank.Two);

            firework.AddCard(whiteOneCard);
            firework.AddCard(whiteTwoCard);

            var otherWhiteTwoCard = new Card(Color.White, Rank.Two);
            var added = firework.AddCard(otherWhiteTwoCard);

            Assert.IsFalse(added);
        }

        [Test]
        public void AddCard_AddBlueFourCardToBlueFireworkWithLastThree_ReturnsTrue()
        {
            IGameProvider provider = new FakeGameProvider()
            {
                Colors = new List<Color> { Color.Blue, Color.Red,} ,
                Nominals = new List<Rank> { Rank.One, Rank.Two, Rank.Three, Rank.Four }
            };
            var firework = new FireworkPile(provider);

            var blueOneCard = new Card(Color.Blue, Rank.One);
            var blueTwoCard = new Card(Color.Blue, Rank.Two);
            var blueThreeCard = new Card(Color.Blue, Rank.Three);

            firework.AddCard(blueOneCard);
            firework.AddCard(blueTwoCard);
            firework.AddCard(blueThreeCard);

            var blueFourCard = new Card(Color.Blue, Rank.Four);
            var isAdded = firework.AddCard(blueFourCard);

            Assert.IsTrue(isAdded);
        }

        #endregion

        #region GetExpectedCards test methods

        [Test]
        public void GetExpectedCards_FullBlueFirework_ReturnsNullForBlueColor()
        {
            IGameProvider provider = new FakeGameProvider()
            {
                Colors = new List<Color> {Color.Blue, Color.White},
                Nominals = new List<Rank> {Rank.One, Rank.Two, Rank.Three, Rank.Four, Rank.Five},
            };

            var pile = new FireworkPile(provider);

            pile.AddCard(new Card(Color.Blue, Rank.One));
            pile.AddCard(new Card(Color.Blue, Rank.Two));
            pile.AddCard(new Card(Color.Blue, Rank.Three));
            pile.AddCard(new Card(Color.Blue, Rank.Four));
            pile.AddCard(new Card(Color.Blue, Rank.Five));

            var nextCards = pile.GetExpectedCards();

            Assert.That(nextCards.All(card => card.Color != Color.Blue));
        }

        [Test]
        public void GetExpectedCards_EmptyFireworkPile_ReturnsListWithFiveElements()
        {
            IGameProvider provider = new FakeGameProvider()
            {
                Colors = new List<Color> {Color.Blue, Color.Green, Color.Red, Color.White, Color.Yellow},
                Nominals = new List<Rank> {Rank.One}
            };


            var fireworkPile = new FireworkPile(provider);

            IReadOnlyList<Card> actual = fireworkPile.GetExpectedCards();

            Assert.AreEqual(5, actual.Count);
        }

        [Test]
        public void GetExpectedCards_BlueFireworkWithLastFour_ReturnsListWithBlueFive()
        {
            IGameProvider provider = new FakeGameProvider()
            {
                Colors = new List<Color> {Color.Blue},
                Nominals = new List<Rank> {Rank.One, Rank.Two, Rank.Three, Rank.Four, Rank.Five}
            };
            var fireworkPile = new FireworkPile(provider);

            fireworkPile.AddCard(new Card(Color.Blue, Rank.One));
            fireworkPile.AddCard(new Card(Color.Blue, Rank.Two));
            fireworkPile.AddCard(new Card(Color.Blue, Rank.Three));
            fireworkPile.AddCard(new Card(Color.Blue, Rank.Four));

            var expectedCards = fireworkPile.GetExpectedCards();

            var blueFiveCard = new Card(Color.Blue, Rank.Five);

            Assert.That(expectedCards.Any(card => card.Equals(blueFiveCard)));
        }

        [Test]
        public void GetExpectedCards_FireworkPileWithFullRedFirework_ReturnsListWithoutRedCards()
        {
            IGameProvider provider = new FakeGameProvider()
            {
                Colors = new List<Color>{Color.Red, Color.Blue},
                Nominals = new List<Rank>{Rank.One, Rank.Two, Rank.Three, Rank.Four, Rank.Five},
            };

            var fireworkPile = new FireworkPile(provider);

            fireworkPile.AddCard(new Card(Color.Red, Rank.One));
            fireworkPile.AddCard(new Card(Color.Red, Rank.Two));
            fireworkPile.AddCard(new Card(Color.Red, Rank.Three));
            fireworkPile.AddCard(new Card(Color.Red, Rank.Four));
            fireworkPile.AddCard(new Card(Color.Red, Rank.Five));

            IReadOnlyList<Card> actual = fireworkPile.GetExpectedCards();

            Assert.IsTrue(actual.All(card => card.Color != Color.Red));
        }

        [Test]
        public void GetExpectedCards_FullFireworkPile_ReturnsEmptyList()
        {
            IGameProvider provider = new FakeGameProvider()
            {
                Colors = new List<Color> {Color.Blue, Color.Green},
                Nominals = new List<Rank> { Rank.One, Rank.Two},
            };

            var fireworkPile = new FireworkPile(provider);

            var blueOneCard = new Card(Color.Blue, Rank.One);
            fireworkPile.AddCard(blueOneCard);

            var blueTwoCard = new Card(Color.Blue, Rank.Two);
            fireworkPile.AddCard(blueTwoCard);

            var greenOneCard = new Card(Color.Green, Rank.One);
            fireworkPile.AddCard(greenOneCard);

            var greenTwoCard = new Card(Color.Green, Rank.Two);
            fireworkPile.AddCard(greenTwoCard);

            IReadOnlyList<Card> actual = fireworkPile.GetExpectedCards();

            Assert.IsEmpty(actual);
        }

        #endregion

        #region GetLastCards test methods

        [Test]
        public void GetLastCards_Default_ReturnsEmptyList()
        {
            IGameProvider provider = new FakeGameProvider()
            {
                Colors = new List<Color> {Color.Blue, Color.Green, Color.Red, Color.White, Color.Yellow},
                Nominals = new List<Rank> {Rank.One, Rank.Two},
            };

            var pile = new FireworkPile(provider);

            var nextCards = pile.GetLastCards();

            Assert.IsEmpty(nextCards);
        }

        [Test]
        public void GetLastCards_FireworkWithGreenOneCard_ReturnsOneForGreenColor()
        {
            IGameProvider provider = new FakeGameProvider()
            {
                Colors = new List<Color> {Color.Green, Color.Blue},
                Nominals = new List<Rank> {Rank.One, Rank.Two},
            };
            var pile = new FireworkPile(provider);

            var greenOneCard = new Card(Color.Green, Rank.One);
            pile.AddCard(greenOneCard);
            
            var lastCards = pile.GetLastCards();

            var otherGreenOneCard = new Card(Color.Green, Rank.One);
            
            Assert.Greater(lastCards.Count, 0);
            Assert.That(lastCards.Any(card => card.Equals(otherGreenOneCard)));
        }
        #endregion
    }
}
