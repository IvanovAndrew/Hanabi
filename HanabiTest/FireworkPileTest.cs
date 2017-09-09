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
                Numbers = new List<Number> { Number.One, Number.Two}
            };
            var pile = new FireworkPile(provider);
            var greenOneCard = new Card(Color.Green, Number.One);

            var added = pile.AddCard(greenOneCard);
            Assert.IsTrue(added);
        }

        [Test]
        public void AddCard_AddWhiteTwoToEmptyFireworkPile_ReturnsFalse()
        {
            IGameProvider provider = new FakeGameProvider()
            {
                Colors = new List<Color> { Color.Green, Color.Red, Color.White },
                Numbers = new List<Number> { Number.One, Number.Two }
            };
            
            var pile = new FireworkPile(provider);
            var whiteTwoCard = new Card(Color.White, Number.Two);

            var added = pile.AddCard(whiteTwoCard);
            Assert.IsFalse(added);
        }

        [Test]
        public void AddCard_AddWhiteTwoCardToWhiteFireworkWithLastTwo_ReturnsFalse()
        {
            IGameProvider provider = new FakeGameProvider()
            {
                Colors = new List<Color> { Color.Green, Color.White },
                Numbers = new List<Number> { Number.One, Number.Two, Number.Three }
            };

            var firework = new FireworkPile(provider);

            var whiteOneCard = new Card(Color.White, Number.One);
            var whiteTwoCard = new Card(Color.White, Number.Two);

            firework.AddCard(whiteOneCard);
            firework.AddCard(whiteTwoCard);

            var otherWhiteTwoCard = new Card(Color.White, Number.Two);
            var added = firework.AddCard(otherWhiteTwoCard);

            Assert.IsFalse(added);
        }

        [Test]
        public void AddCard_AddBlueFourCardToBlueFireworkWithLastThree_ReturnsTrue()
        {
            IGameProvider provider = new FakeGameProvider()
            {
                Colors = new List<Color> { Color.Blue, Color.Red,} ,
                Numbers = new List<Number> { Number.One, Number.Two, Number.Three, Number.Four }
            };
            var firework = new FireworkPile(provider);

            var blueOneCard = new Card(Color.Blue, Number.One);
            var blueTwoCard = new Card(Color.Blue, Number.Two);
            var blueThreeCard = new Card(Color.Blue, Number.Three);

            firework.AddCard(blueOneCard);
            firework.AddCard(blueTwoCard);
            firework.AddCard(blueThreeCard);

            var blueFourCard = new Card(Color.Blue, Number.Four);
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
                Numbers = new List<Number> {Number.One, Number.Two, Number.Three, Number.Four, Number.Five},
            };

            var pile = new FireworkPile(provider);

            pile.AddCard(new Card(Color.Blue, Number.One));
            pile.AddCard(new Card(Color.Blue, Number.Two));
            pile.AddCard(new Card(Color.Blue, Number.Three));
            pile.AddCard(new Card(Color.Blue, Number.Four));
            pile.AddCard(new Card(Color.Blue, Number.Five));

            var nextCards = pile.GetExpectedCards();

            Assert.That(nextCards.All(card => card.Color != Color.Blue));
        }

        [Test]
        public void GetExpectedCards_EmptyFireworkPile_ReturnsListWithFiveElements()
        {
            IGameProvider provider = new FakeGameProvider()
            {
                Colors = new List<Color> {Color.Blue, Color.Green, Color.Red, Color.White, Color.Yellow},
                Numbers = new List<Number> {Number.One}
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
                Numbers = new List<Number> {Number.One, Number.Two, Number.Three, Number.Four, Number.Five}
            };
            var fireworkPile = new FireworkPile(provider);

            fireworkPile.AddCard(new Card(Color.Blue, Number.One));
            fireworkPile.AddCard(new Card(Color.Blue, Number.Two));
            fireworkPile.AddCard(new Card(Color.Blue, Number.Three));
            fireworkPile.AddCard(new Card(Color.Blue, Number.Four));

            var expectedCards = fireworkPile.GetExpectedCards();

            var blueFiveCard = new Card(Color.Blue, Number.Five);

            Assert.That(expectedCards.Any(card => card.Equals(blueFiveCard)));
        }

        [Test]
        public void GetExpectedCards_FireworkPileWithFullRedFirework_ReturnsListWithoutRedCards()
        {
            IGameProvider provider = new FakeGameProvider()
            {
                Colors = new List<Color>{Color.Red, Color.Blue},
                Numbers = new List<Number>{Number.One, Number.Two, Number.Three, Number.Four, Number.Five},
            };

            var fireworkPile = new FireworkPile(provider);

            fireworkPile.AddCard(new Card(Color.Red, Number.One));
            fireworkPile.AddCard(new Card(Color.Red, Number.Two));
            fireworkPile.AddCard(new Card(Color.Red, Number.Three));
            fireworkPile.AddCard(new Card(Color.Red, Number.Four));
            fireworkPile.AddCard(new Card(Color.Red, Number.Five));

            IReadOnlyList<Card> actual = fireworkPile.GetExpectedCards();

            Assert.IsTrue(actual.All(card => card.Color != Color.Red));
        }

        [Test]
        public void GetExpectedCards_FullFireworkPile_ReturnsEmptyList()
        {
            IGameProvider provider = new FakeGameProvider()
            {
                Colors = new List<Color> {Color.Blue, Color.Green},
                Numbers = new List<Number> { Number.One, Number.Two},
            };

            var fireworkPile = new FireworkPile(provider);

            var blueOneCard = new Card(Color.Blue, Number.One);
            fireworkPile.AddCard(blueOneCard);

            var blueTwoCard = new Card(Color.Blue, Number.Two);
            fireworkPile.AddCard(blueTwoCard);

            var greenOneCard = new Card(Color.Green, Number.One);
            fireworkPile.AddCard(greenOneCard);

            var greenTwoCard = new Card(Color.Green, Number.Two);
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
                Numbers = new List<Number> {Number.One, Number.Two},
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
                Numbers = new List<Number> {Number.One, Number.Two},
            };
            var pile = new FireworkPile(provider);

            var greenOneCard = new Card(Color.Green, Number.One);
            pile.AddCard(greenOneCard);
            
            var lastCards = pile.GetLastCards();

            var otherGreenOneCard = new Card(Color.Green, Number.One);
            
            Assert.Greater(lastCards.Count, 0);
            Assert.That(lastCards.Any(card => card.Equals(otherGreenOneCard)));
        }
        #endregion
    }
}
