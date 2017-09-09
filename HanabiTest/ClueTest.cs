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
            Clue clue = new ClueAboutNominal(Nominal.Five);

            Clue revertedClue = clue.Revert();

            Clue expectedClue = new ClueAboutNotNominal(Nominal.Five);

            Assert.AreEqual(expectedClue, revertedClue);
        }

        [Test]
        public void Revert_IsNotValueFive_ReturnsIsValueFour()
        {
            Clue clue = new ClueAboutNotNominal(Nominal.Four);

            Clue revertedClue = clue.Revert();

            Clue expectedClue = new ClueAboutNominal(Nominal.Four);

            Assert.AreEqual(expectedClue, revertedClue);
        }

        [Test]
        public void Revert_IsNotColorRed_ReturnsIsColorRed()
        {
            Clue clue = new ClueAboutNotColor(Color.Red);

            Clue revertedClue = clue.Revert();

            Clue expectedClue = new ClueAboutColor(Color.Red);

            Assert.AreEqual(expectedClue, revertedClue);
        }

        [Test]
        public void Revert_IsColorRed_ReturnsIsNotColorRed()
        {
            Clue clue = new ClueAboutColor(Color.Red);

            Clue revertedClue = clue.Revert();

            Clue expectedClue = new ClueAboutNotColor(Color.Red);

            Assert.AreEqual(expectedClue, revertedClue);
        }
    }
}
