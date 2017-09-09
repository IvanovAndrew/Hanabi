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
            Clue clue = new IsNominal(Number.Five);

            Clue revertedClue = clue.Revert();

            Clue expectedClue = new IsNotNominal(Number.Five);

            Assert.AreEqual(expectedClue, revertedClue);
        }

        [Test]
        public void Revert_IsNotValueFive_ReturnsIsValueFour()
        {
            Clue clue = new IsNotNominal(Number.Four);

            Clue revertedClue = clue.Revert();

            Clue expectedClue = new IsNominal(Number.Four);

            Assert.AreEqual(expectedClue, revertedClue);
        }

        [Test]
        public void Revert_IsNotColorRed_ReturnsIsColorRed()
        {
            Clue clue = new IsNotColor(Color.Red);

            Clue revertedClue = clue.Revert();

            Clue expectedClue = new IsColor(Color.Red);

            Assert.AreEqual(expectedClue, revertedClue);
        }

        [Test]
        public void Revert_IsColorRed_ReturnsIsNotColorRed()
        {
            Clue clue = new IsColor(Color.Red);

            Clue revertedClue = clue.Revert();

            Clue expectedClue = new IsNotColor(Color.Red);

            Assert.AreEqual(expectedClue, revertedClue);
        }
    }
}
