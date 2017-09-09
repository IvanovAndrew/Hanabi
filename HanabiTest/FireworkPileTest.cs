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
                Nominals = new List<Nominal> { Nominal.One, Nominal.Two}
            };
            var pile = new FireworkPile(provider);
            var greenOneCard = new Card(Color.Green, Nominal.One);

            var added = pile.AddCard(greenOneCard);
            Assert.IsTrue(added);
        }

        [Test]
        public void AddCard_AddWhiteTwoToEmptyFireworkPile_ReturnsFalse()
        {
            IGameProvider provider = new FakeGameProvider()
            {
                Colors = new List<Color> { Color.Green, Color.Red, Color.White },
                Nominals = new List<Nominal> { Nominal.One, Nominal.Two }
            };
            
            var pile = new FireworkPile(provider);
            var whiteTwoCard = new Card(Color.White, Nominal.Two);

            var added = pile.AddCard(whiteTwoCard);
            Assert.IsFalse(added);
        }

        [Test]
        public void AddCard_AddWhiteTwoCardToWhiteFireworkWithLastTwo_ReturnsFalse()
        {
            IGameProvider provider = new FakeGameProvider()
            {
                Colors = new List<Color> { Color.Green, Color.White },
                Nominals = new List<Nominal> { Nominal.One, Nominal.Two, Nominal.Three }
            };

            var firework = new FireworkPile(provider);

            var whiteOneCard = new Card(Color.White, Nominal.One);
            var whiteTwoCard = new Card(Color.White, Nominal.Two);

            firework.AddCard(whiteOneCard);
            firework.AddCard(whiteTwoCard);

            var otherWhiteTwoCard = new Card(Color.White, Nominal.Two);
            var added = firework.AddCard(otherWhiteTwoCard);

            Assert.IsFalse(added);
        }

        [Test]
        public void AddCard_AddBlueFourCardToBlueFireworkWithLastThree_ReturnsTrue()
        {
            IGameProvider provider = new FakeGameProvider()
            {
                Colors = new List<Color> { Color.Blue, Color.Red,} ,
                Nominals = new List<Nominal> { Nominal.One, Nominal.Two, Nominal.Three, Nominal.Four }
            };
            var firework = new FireworkPile(provider);

            var blueOneCard = new Card(Color.Blue, Nominal.One);
            var blueTwoCard = new Card(Color.Blue, Nominal.Two);
            var blueThreeCard = new Card(Color.Blue, Nominal.Three);

            firework.AddCard(blueOneCard);
            firework.AddCard(blueTwoCard);
            firework.AddCard(blueThreeCard);

            var blueFourCard = new Card(Color.Blue, Nominal.Four);
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
                Nominals = new List<Nominal> {Nominal.One, Nominal.Two, Nominal.Three, Nominal.Four, Nominal.Five},
            };

            var pile = new FireworkPile(provider);

            pile.AddCard(new Card(Color.Blue, Nominal.One));
            pile.AddCard(new Card(Color.Blue, Nominal.Two));
            pile.AddCard(new Card(Color.Blue, Nominal.Three));
            pile.AddCard(new Card(Color.Blue, Nominal.Four));
            pile.AddCard(new Card(Color.Blue, Nominal.Five));

            var nextCards = pile.GetExpectedCards();

            Assert.That(nextCards.All(card => card.Color != Color.Blue));
        }

        [Test]
        public void GetExpectedCards_EmptyFireworkPile_ReturnsListWithFiveElements()
        {
            IGameProvider provider = new FakeGameProvider()
            {
                Colors = new List<Color> {Color.Blue, Color.Green, Color.Red, Color.White, Color.Yellow},
                Nominals = new List<Nominal> {Nominal.One}
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
                Nominals = new List<Nominal> {Nominal.One, Nominal.Two, Nominal.Three, Nominal.Four, Nominal.Five}
            };
            var fireworkPile = new FireworkPile(provider);

            fireworkPile.AddCard(new Card(Color.Blue, Nominal.One));
            fireworkPile.AddCard(new Card(Color.Blue, Nominal.Two));
            fireworkPile.AddCard(new Card(Color.Blue, Nominal.Three));
            fireworkPile.AddCard(new Card(Color.Blue, Nominal.Four));

            var expectedCards = fireworkPile.GetExpectedCards();

            var blueFiveCard = new Card(Color.Blue, Nominal.Five);

            Assert.That(expectedCards.Any(card => card.Equals(blueFiveCard)));
        }

        [Test]
        public void GetExpectedCards_FireworkPileWithFullRedFirework_ReturnsListWithoutRedCards()
        {
            IGameProvider provider = new FakeGameProvider()
            {
                Colors = new List<Color>{Color.Red, Color.Blue},
                Nominals = new List<Nominal>{Nominal.One, Nominal.Two, Nominal.Three, Nominal.Four, Nominal.Five},
            };

            var fireworkPile = new FireworkPile(provider);

            fireworkPile.AddCard(new Card(Color.Red, Nominal.One));
            fireworkPile.AddCard(new Card(Color.Red, Nominal.Two));
            fireworkPile.AddCard(new Card(Color.Red, Nominal.Three));
            fireworkPile.AddCard(new Card(Color.Red, Nominal.Four));
            fireworkPile.AddCard(new Card(Color.Red, Nominal.Five));

            IReadOnlyList<Card> actual = fireworkPile.GetExpectedCards();

            Assert.IsTrue(actual.All(card => card.Color != Color.Red));
        }

        [Test]
        public void GetExpectedCards_FullFireworkPile_ReturnsEmptyList()
        {
            IGameProvider provider = new FakeGameProvider()
            {
                Colors = new List<Color> {Color.Blue, Color.Green},
                Nominals = new List<Nominal> { Nominal.One, Nominal.Two},
            };

            var fireworkPile = new FireworkPile(provider);

            var blueOneCard = new Card(Color.Blue, Nominal.One);
            fireworkPile.AddCard(blueOneCard);

            var blueTwoCard = new Card(Color.Blue, Nominal.Two);
            fireworkPile.AddCard(blueTwoCard);

            var greenOneCard = new Card(Color.Green, Nominal.One);
            fireworkPile.AddCard(greenOneCard);

            var greenTwoCard = new Card(Color.Green, Nominal.Two);
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
                Nominals = new List<Nominal> {Nominal.One, Nominal.Two},
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
                Nominals = new List<Nominal> {Nominal.One, Nominal.Two},
            };
            var pile = new FireworkPile(provider);

            var greenOneCard = new Card(Color.Green, Nominal.One);
            pile.AddCard(greenOneCard);
            
            var lastCards = pile.GetLastCards();

            var otherGreenOneCard = new Card(Color.Green, Nominal.One);
            
            Assert.Greater(lastCards.Count, 0);
            Assert.That(lastCards.Any(card => card.Equals(otherGreenOneCard)));
        }
        #endregion
    }
}
