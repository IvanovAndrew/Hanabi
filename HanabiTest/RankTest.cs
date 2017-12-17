using Hanabi;
using NUnit.Framework;

namespace HanabiTest
{
    [TestFixture]
    public class RankTest
    {
        [Test]
        [TestCase(Rank.One, Rank.Two)]
        [TestCase(Rank.Two, Rank.Three)]
        [TestCase(Rank.Three, Rank.Four)]
        [TestCase(Rank.Four, Rank.Five)]
        public void GetNextNumber_Number_ReturnsNextNumber(Rank rank, Rank expected)
        {
            Assert.AreEqual(expected, rank.GetNextNumber());
        }

        [Test]
        public void GetNextNumber_NumberFive_ReturnsNull()
        {
            Assert.AreEqual(null, Rank.Five.GetNextNumber());
        }

        [Test]
        [TestCase(Rank.Two, Rank.One)]
        [TestCase(Rank.Three, Rank.Two)]
        [TestCase(Rank.Four, Rank.Three)]
        [TestCase(Rank.Five, Rank.Four)]
        public void GetPreviousNumber_Number_ReturnsPreviousNumber(Rank rank, Rank expected)
        {
            Assert.AreEqual(expected, rank.GetPreviousNumber());
        }

        [Test]
        public void GetPreviousNumber_NumberOne_ReturnsNull()
        {
            Assert.AreEqual(null, Rank.One.GetPreviousNumber());
        }
    }
}
