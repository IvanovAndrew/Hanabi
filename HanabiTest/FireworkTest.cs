using Hanabi;
using NUnit.Framework;
using System;

namespace HanabiTest
{
    [TestFixture]
    public class FireworkTest
    {
        [Test]
        public void EmptyGreenFirework_AddGreenOneCard_CardAdded()
        {
            var firework = new GreenFirework();

            var greenOne = new GreenCard { Nominal = Number.One };

            var isAdded = firework.AddCard(greenOne);

            Assert.IsTrue(isAdded);
        }

        [Test]
        public void EmptyYellowFirework_AddYellowTwoCard_CardNotAdded()
        {
            var firework = new YellowFirework();

            var yellowTwo = new YellowCard { Nominal = Number.Two };

            var isAdded = firework.AddCard(yellowTwo);

            Assert.IsFalse(isAdded);
        }

        [Test]
        public void BlueFireworkWithLastThree_AddBlueFourCard_CardAdded()
        {
            var firework = new BlueFirework();

            var blueOneCard = new BlueCard { Nominal = Number.One };
            var blueTwoCard = new BlueCard { Nominal = Number.Two };
            var blueThreeCard = new BlueCard { Nominal = Number.Three };
            var blueFourCard = new BlueCard { Nominal = Number.Four };

            firework.AddCard(blueOneCard);
            firework.AddCard(blueTwoCard);
            firework.AddCard(blueThreeCard);

            var isAdded = firework.AddCard(blueFourCard);

            Assert.IsTrue(isAdded);
        }

        [Test]
        public void WhiteFireworkWithLastTwo_AddWhiteTwoCard_CardNotAdded()
        {
            var firework = new WhiteFirework();

            var whiteOneCard = new WhiteCard { Nominal = Number.One };
            var whiteTwoCard = new WhiteCard { Nominal = Number.Two };

            firework.AddCard(whiteOneCard);
            firework.AddCard(whiteTwoCard);

            var otherWhiteTwoCard = new WhiteCard { Nominal = Number.Two };
            var added = firework.AddCard(otherWhiteTwoCard);

            Assert.IsFalse(added);
        }

        [Test]
        public void RedFireworkWithLastOne_AddBlueTwoCard_ArgumentExceptionThrown()
        {
            var firework = new RedFirework();

            var redOneCard = new RedCard { Nominal = Number.One };
            firework.AddCard(redOneCard);

            var blueTwoCard = new BlueCard { Nominal = Number.Two };

            Assert.Catch<ArgumentException>(() => firework.AddCard(blueTwoCard));
        }
    }
}
