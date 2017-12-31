using System;
using Hanabi;
using NUnit.Framework;

namespace HanabiTest
{
    [TestFixture]
    public class ClueDetailInfoTest
    {
        [Test]
        public void GetClueInfo_ClueAboutRankThree_ReturnsNotNullRank()
        {
            var clueDetailInfo = ClueDetailInfo.GetClueInfo(new ClueAboutRank(Rank.Three));
            Assert.AreEqual(Rank.Three, clueDetailInfo.Rank);
            Assert.IsNull(clueDetailInfo.Color);
        }

        [Test]
        public void GetClueInfo_ClueAboutWhiteColor_ReturnsNotNullColor()
        {
            var clueDetailInfo = ClueDetailInfo.GetClueInfo(new ClueAboutColor(Color.White));
            Assert.AreEqual(Color.White, clueDetailInfo.Color);
            Assert.IsNull(clueDetailInfo.Rank);
        }

        [Test]
        public void GetClueInfo_NotStraightClue_ThrowsInvalidOperationException()
        {
            var ex = Assert.Catch<InvalidOperationException>(
                () => ClueDetailInfo.GetClueInfo(new ClueAboutNotColor(Color.Blue)));

            StringAssert.Contains("type of clue", ex.Message);
        }

        
    }
}
