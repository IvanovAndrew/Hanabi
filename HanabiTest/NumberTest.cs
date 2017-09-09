using Hanabi;
using NUnit.Framework;

namespace HanabiTest
{
    [TestFixture]
    public class NumberTest
    {
        [Test]
        [TestCase(Nominal.One, Nominal.Two)]
        [TestCase(Nominal.Two, Nominal.Three)]
        [TestCase(Nominal.Three, Nominal.Four)]
        [TestCase(Nominal.Four, Nominal.Five)]
        public void GetNextNumber_Number_ReturnsNextNumber(Nominal nominal, Nominal expected)
        {
            Assert.AreEqual(expected, nominal.GetNextNumber());
        }

        [Test]
        public void GetNextNumber_NumberFive_ReturnsNull()
        {
            Assert.AreEqual(null, Nominal.Five.GetNextNumber());
        }

        [Test]
        [TestCase(Nominal.Two, Nominal.One)]
        [TestCase(Nominal.Three, Nominal.Two)]
        [TestCase(Nominal.Four, Nominal.Three)]
        [TestCase(Nominal.Five, Nominal.Four)]
        public void GetPreviousNumber_Number_ReturnsPreviousNumber(Nominal nominal, Nominal expected)
        {
            Assert.AreEqual(expected, nominal.GetPreviousNumber());
        }

        [Test]
        public void GetPreviousNumber_NumberOne_ReturnsNull()
        {
            Assert.AreEqual(null, Nominal.One.GetPreviousNumber());
        }
    }
}
