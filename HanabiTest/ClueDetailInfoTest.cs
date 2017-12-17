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

        [Test]
        public void IsSubtleClue_FireworkWithAllOnesExceptRedAndCluesAboutOne_ReturnsTrue()
        {
            var firework = new FireworkPile(new GameProvider());
            
            firework.AddCard(new Card(Color.Blue, Rank.One));
            firework.AddCard(new Card(Color.Green, Rank.One));
            firework.AddCard(new Card(Color.White, Rank.One));
            firework.AddCard(new Card(Color.Yellow, Rank.One));
            
            var clue = new ClueAboutRank(Rank.One);

            // act
            bool result = ClueDetailInfo.IsSubtleClue(firework, clue);

            // assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsSubtleClue_FireworkWithAllOnesExceptRedAndYellowAndCluesAboutOne_ReturnsTrue()
        {
            var firework = new FireworkPile(new GameProvider());

            firework.AddCard(new Card(Color.Blue, Rank.One));
            firework.AddCard(new Card(Color.Green, Rank.One));
            firework.AddCard(new Card(Color.White, Rank.One));

            var clue = new ClueAboutRank(Rank.One);

            // act
            bool result = ClueDetailInfo.IsSubtleClue(firework, clue);

            // assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsSubtleClue_FireworkWithAllOnesAndCluesAboutOne_ReturnsFalse()
        {
            var firework = new FireworkPile(new GameProvider());

            firework.AddCard(new Card(Color.Blue, Rank.One));
            firework.AddCard(new Card(Color.Green, Rank.One));
            firework.AddCard(new Card(Color.Red, Rank.One));
            firework.AddCard(new Card(Color.White, Rank.One));
            firework.AddCard(new Card(Color.Yellow, Rank.One));

            var clue = new ClueAboutRank(Rank.One);

            // act
            bool result = ClueDetailInfo.IsSubtleClue(firework, clue);

            // assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsSubtleClue_FireworkWithBlueAndGreenOneAndCluesAboutOne_ReturnsFalse()
        {
            var firework = new FireworkPile(new GameProvider());

            firework.AddCard(new Card(Color.Blue, Rank.One));
            firework.AddCard(new Card(Color.Green, Rank.One));

            var clue = new ClueAboutRank(Rank.One);

            // act
            bool result = ClueDetailInfo.IsSubtleClue(firework, clue);

            // assert
            Assert.IsFalse(result);
        }
    }
}
