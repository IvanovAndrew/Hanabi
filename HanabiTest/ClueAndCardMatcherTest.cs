using Hanabi;
using NUnit.Framework;

namespace HanabiTest
{
    [TestFixture]
    public class ClueAndCardMatcherTest
    {
        [Test]
        public void Match_ClueAboutThreeAndBlueTwoCard_ReturnsFalse()
        {
            var blueTwoCard = new Card(Color.Blue, Rank.Two);
            var matcher = new ClueAndCardMatcher(blueTwoCard);
            var clue = new ClueAboutRank(Rank.Three);

            Assert.IsFalse(clue.Accept(matcher));
        }

        [Test]
        public void Match_ClueAboutThreeAndBlueThreeCard_ReturnsTrue()
        {
            var blueThreeCard = new Card(Color.Blue, Rank.Three);
            var matcher = new ClueAndCardMatcher(blueThreeCard);

            var clue = new ClueAboutRank(Rank.Three);

            Assert.IsTrue(clue.Accept(matcher));
        }

        [Test]
        public void Match_ClueAboutWhiteAndBlueThreeCard_ReturnsFalse()
        {
            var blueThreeCard = new Card(Color.Blue, Rank.Three);
            var matcher = new ClueAndCardMatcher(blueThreeCard);
            var clue = new ClueAboutColor(Color.White);

            Assert.IsFalse(clue.Accept(matcher));
        }

        [Test]
        public void Match_ClueAboutBlueAndBlueThreeCard_ReturnsTrue()
        {
            var blueThreeCard = new Card(Color.Blue, Rank.Three);
            var matcher = new ClueAndCardMatcher(blueThreeCard);
            var clue = new ClueAboutColor(Color.Blue);

            Assert.IsTrue(clue.Accept(matcher));
        }
    }
}
