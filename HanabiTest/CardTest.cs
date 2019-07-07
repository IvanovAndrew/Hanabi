using Hanabi;
using NUnit.Framework;

namespace HanabiTest
{
    [TestFixture]
    public class CardTest
    {
        [Test]
        public void Equals_YelloTwoAndYellowTwo_AreEqual()
        {
            var yellowTwo = new Card(Color.Yellow, Rank.Two);
            var anotherYellowTwo = new Card(Color.Yellow, Rank.Two);

            Assert.AreEqual(yellowTwo, anotherYellowTwo);
        }

        [Test]
        public void Equals_YelloTwoAndRedTwo_AreNotEqual()
        {
            var yellowTwo = new Card(Color.Yellow, Rank.Two);
            var redTwo = new Card(Color.Red, Rank.Two);

            Assert.AreNotEqual(yellowTwo, redTwo);
        }

        [Test]
        public void Equals_YelloTwoAndYellowFour_AreNotEqual()
        {
            var yellowTwo = new Card(Color.Yellow, Rank.Two);
            var yellowFour = new Card(Color.Yellow, Rank.Four);

            Assert.AreNotEqual(yellowTwo, yellowFour);
        }

        [Test]
        public void GetCardInFireworkBefore_YellowFour_ReturnsYellowThree()
        {
            var yellowFour = new Card(Color.Yellow, Rank.Four);

            var cardBeforeYellowFour = Card.GetPreviousCardInFirework(yellowFour);
            var yellowThreeCard = new Card(Color.Yellow, Rank.Three);

            Assert.AreEqual(yellowThreeCard, cardBeforeYellowFour);
        }

        [Test]
        public void GetCardInFireworkBefore_YellowOne_ReturnsNull()
        {
            var yellowOneCard = new Card(Color.Yellow, Rank.One);

            var cardBeforeYellowOne = Card.GetPreviousCardInFirework(yellowOneCard);

            Assert.IsNull(cardBeforeYellowOne);
        }
    }
}
