using Hanabi;
using NUnit.Framework;

namespace HanabiTest
{
    [TestFixture]
    public class NumberTest
    {
        [Test]
        [TestCase(Number.One, Number.Two)]
        [TestCase(Number.Two, Number.Three)]
        [TestCase(Number.Three, Number.Four)]
        [TestCase(Number.Four, Number.Five)]
        public void GetNextNumber_Number_ReturnsNextNumber(Number number, Number expected)
        {
            Assert.AreEqual(expected, number.GetNextNumber());
        }

        [Test]
        public void GetNextNumber_NumberFive_ReturnsNull()
        {
            Assert.AreEqual(null, Number.Five.GetNextNumber());
        }

        [Test]
        [TestCase(Number.Two, Number.One)]
        [TestCase(Number.Three, Number.Two)]
        [TestCase(Number.Four, Number.Three)]
        [TestCase(Number.Five, Number.Four)]
        public void GetPreviousNumber_Number_ReturnsPreviousNumber(Number number, Number expected)
        {
            Assert.AreEqual(expected, number.GetPreviousNumber());
        }

        [Test]
        public void GetPreviousNumber_NumberOne_ReturnsNull()
        {
            Assert.AreEqual(null, Number.One.GetPreviousNumber());
        }
    }
}
