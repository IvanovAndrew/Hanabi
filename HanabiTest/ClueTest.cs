using Hanabi;
using NUnit.Framework;

namespace HanabiTest
{
    [TestFixture]
    public class ClueTest
    {
        [Test]
        public void Revert_IsValueFive_ReturnsNotIsValueFive()
        {
            ClueType clue = new ClueAboutRank(Rank.Five);

            ClueType revertedClue = clue.Revert();

            ClueType expectedClue = new ClueAboutNotRank(Rank.Five);

            Assert.AreEqual(expectedClue, revertedClue);
        }

        [Test]
        public void Revert_IsNotValueFive_ReturnsIsValueFour()
        {
            ClueType clue = new ClueAboutNotRank(Rank.Four);

            ClueType revertedClue = clue.Revert();

            ClueType expectedClue = new ClueAboutRank(Rank.Four);

            Assert.AreEqual(expectedClue, revertedClue);
        }

        [Test]
        public void Revert_IsNotColorRed_ReturnsIsColorRed()
        {
            ClueType clue = new ClueAboutNotColor(Color.Red);

            ClueType revertedClue = clue.Revert();

            ClueType expectedClue = new ClueAboutColor(Color.Red);

            Assert.AreEqual(expectedClue, revertedClue);
        }

        [Test]
        public void Revert_IsColorRed_ReturnsIsNotColorRed()
        {
            ClueType clue = new ClueAboutColor(Color.Red);

            ClueType revertedClue = clue.Revert();

            ClueType expectedClue = new ClueAboutNotColor(Color.Red);

            Assert.AreEqual(expectedClue, revertedClue);
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
            bool result = clue.IsSubtleClue(firework.GetExpectedCards());

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
            bool result = clue.IsSubtleClue(firework.GetExpectedCards());

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
            bool result = clue.IsSubtleClue(firework.GetExpectedCards());

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
            bool result = clue.IsSubtleClue(firework.GetExpectedCards());

            // assert
            Assert.IsFalse(result);
        }
    }
}
