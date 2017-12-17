using Hanabi;
using NUnit.Framework;

namespace HanabiTest
{
    [TestFixture]
    public class ClueAndCardMatcherTest
    {
        [Test]
        public void Match_ClueAboutThreeAndBlueTwoCard_ReturnsRevertedClue()
        {
            var clue = ClueAndCardMatcher.Match(new ClueAboutRank(Rank.Three), new Card(Color.Blue, Rank.Two));

            Assert.IsTrue(clue is ClueAboutNotRank);
        }

        [Test]
        public void Match_ClueAboutThreeAndBlueThreeCard_ReturnsClueAboutRank()
        {
            var clue = ClueAndCardMatcher.Match(new ClueAboutRank(Rank.Three), new Card(Color.Blue, Rank.Three));

            Assert.IsTrue(clue is ClueAboutRank);
        }

        [Test]
        public void Match_ClueAboutWhiteAndBlueThreeCard_ReturnsRevertedClue()
        {
            var clue = ClueAndCardMatcher.Match(new ClueAboutColor(Color.White), new Card(Color.Blue, Rank.Three));

            Assert.IsTrue(clue is ClueAboutNotColor);
        }

        [Test]
        public void Match_ClueAboutBlueAndBlueThreeCard_ReturnsClueAboutColor()
        {
            var clue = ClueAndCardMatcher.Match(new ClueAboutColor(Color.Blue), new Card(Color.Blue, Rank.Three));

            Assert.IsTrue(clue is ClueAboutColor);
        }
    }
}
