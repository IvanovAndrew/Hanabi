using System;
using Hanabi;
using NUnit.Framework;

namespace HanabiTest
{
    [TestFixture]
    public class GameTest
    {
        [Test]
        [TestCase(0)]
        [TestCase(1)]
        public void NewGame_LessThen2Players_ArgumentOutOfRangeThrown(int playersCount)
        {
            Exception exception = Assert.Throws<ArgumentOutOfRangeException>(() => new Game(playersCount));

            StringAssert.Contains("Too less players", exception.Message);
        }

        [Test]
        [TestCase(6)]
        [TestCase(7)]
        public void NewGame_MoreThen5Players_ArgumentOutOfRangeThrown(int playersCount)
        {
            Exception exception = Assert.Throws<ArgumentOutOfRangeException>(() => new Game(playersCount));

            StringAssert.Contains("Too many players", exception.Message);
        }
    }
}
