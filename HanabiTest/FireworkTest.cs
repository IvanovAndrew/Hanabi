using Hanabi;
using NUnit.Framework;
using System;

namespace HanabiTest
{
    [TestFixture]
    public class FireworkTest
    {
        [Test]
        public void AddCard_AddGreenOneCardToEmptyGreenFirework_Added()
        {
            var firework = new GreenFirework();

            var greenOne = new GreenCard(Number.One);

            var isAdded = firework.AddCard(greenOne);

            Assert.IsTrue(isAdded);
        }

        [Test]
        public void AddCard_AddYellowTwoCardToEmptyYellowFirework_NotAdded()
        {
            var firework = new YellowFirework();

            var yellowTwo = new YellowCard(Number.Two);

            var isAdded = firework.AddCard(yellowTwo);

            Assert.IsFalse(isAdded);
        }

        [Test]
        public void AddCard_AddBlueFourCardToBlueFireworkWithLastThree_Added()
        {
            var firework = new BlueFirework();

            var blueOneCard = new BlueCard (Number.One);
            var blueTwoCard = new BlueCard (Number.Two);
            var blueThreeCard = new BlueCard(Number.Three);
            var blueFourCard = new BlueCard (Number.Four);

            firework.AddCard(blueOneCard);
            firework.AddCard(blueTwoCard);
            firework.AddCard(blueThreeCard);

            var isAdded = firework.AddCard(blueFourCard);

            Assert.IsTrue(isAdded);
        }

        [Test]
        public void AddCard_AddWhiteTwoCardToWhiteFireworkWithLastTwo_NotAdded()
        {
            var firework = new WhiteFirework();

            var whiteOneCard = new WhiteCard(Number.One);
            var whiteTwoCard = new WhiteCard(Number.Two);

            firework.AddCard(whiteOneCard);
            firework.AddCard(whiteTwoCard);

            var otherWhiteTwoCard = new WhiteCard(Number.Two);
            var added = firework.AddCard(otherWhiteTwoCard);

            Assert.IsFalse(added);
        }

        [Test]
        public void AddCard_AddBlueTwoCardToRedFireworkWithLastOne_ArgumentExceptionThrown()
        {
            var firework = new RedFirework();

            var redOneCard = new RedCard(Number.One);
            firework.AddCard(redOneCard);

            var blueTwoCard = new BlueCard(Number.Two);

            Assert.Catch<ArgumentException>(() => firework.AddCard(blueTwoCard));
        }

        [Test]
        public void GetNextCard_EmptyBlueFirework_BlueOne()
        {
            var firework = new BlueFirework();

            var nextCard = firework.GetNextCard();

            Assert.AreEqual(Number.One, nextCard.Nominal);
        }

        [Test]
        public void GetNextCard_FullBlueFirework_Null()
        {
            var firework = new BlueFirework();
            firework.AddCard(new BlueCard (Number.One));
            firework.AddCard(new BlueCard (Number.Two));
            firework.AddCard(new BlueCard (Number.Three));
            firework.AddCard(new BlueCard (Number.Four));
            firework.AddCard(new BlueCard (Number.Five));

            var nextCard = firework.GetNextCard();

            Assert.IsNull(nextCard);
        }

        [Test]
        public void GetNextCard_BlueFireworkWithLastFour_BlueFive()
        {
            var firework = new BlueFirework();
            firework.AddCard(new BlueCard(Number.One));
            firework.AddCard(new BlueCard(Number.Two));
            firework.AddCard(new BlueCard(Number.Three));
            firework.AddCard(new BlueCard(Number.Four));

            var nextCard = firework.GetNextCard();

            Assert.AreEqual(new BlueCard(Number.Five), nextCard);
        }
    }
}
