using System;
using Hanabi;
using NUnit.Framework;

namespace HanabiTest
{
    [TestFixture]
    public class GameModeTest
    {
        [Test]
        [TestCase(1)]
        public void GameMode_LessThen2Players_ArgumentOutOfRangeThrown(int playersCount)
        {
            var exception = Assert.Throws<ArgumentOutOfRangeException>(
                () => new GameMode(playersCount));

            StringAssert.Contains("Too less players", exception.Message);
        }

        [Test]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        public void GameMode_CorrectedNumberOfPlayers_NotThrowsException(int playersCount)
        {
            new GameMode(playersCount);
            Assert.Pass();
        }

        [Test]
        [TestCase(6)]
        public void GameMode_MoreThen5Players_ArgumentOutOfRangeThrown(int playersCount)
        {
            var exception = Assert.Throws<ArgumentOutOfRangeException>(
                () => new GameMode(playersCount));

            StringAssert.Contains("Too many players", exception.Message);
        }

        [Test]
        public void GameMode_ByDefault_Returns5Colors()
        {
            GameMode gameMode = new GameMode(2);

            Assert.AreEqual(5, gameMode.ColorsCount);
        }
    }
}
