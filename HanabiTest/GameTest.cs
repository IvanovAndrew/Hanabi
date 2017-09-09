using System;
using System.Collections.Generic;
using Hanabi;
using NUnit.Framework;

namespace HanabiTest
{
    [TestFixture]
    public class GameTest
    {
        [Test]
        [TestCase(1)]
        public void Game_LessThen2Players_ThrowsArgumentOutOfRangeException(int playersCount)
        {
            IGameProvider provider = new FakeGameProvider();

            var exception = Assert.Throws<ArgumentOutOfRangeException>(
                () => new Game(provider, playersCount));

            StringAssert.Contains("Too less players", exception.Message);
        }

        [Test]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        public void Game_CorrectNumberOfPlayers_NotThrowsException(int playersCount)
        {
            FakeGameProvider provider = new FakeGameProvider();
            provider.Colors = new List<Color> {Color.Blue};
            provider.Nominals = new List<Nominal> { Nominal.One };
            provider.FullDeckMatrix = provider.CreateEmptyMatrix();
            new Game(provider, playersCount);
            Assert.Pass();
        }

        [Test]
        [TestCase(6)]
        public void Game_MoreThen5Players_ThrowsArgumentOutOfRangeException(int playersCount)
        {
            IGameProvider provider = new FakeGameProvider();

            var exception = Assert.Throws<ArgumentOutOfRangeException>(
                () => new Game(provider, playersCount));

            StringAssert.Contains("Too many players", exception.Message);
        }

        [Test]
        public void Game_Default_ZeroScore()
        {
            FakeGameProvider provider = new FakeGameProvider
            {
                Colors = new List<Color> {Color.Blue},
                Nominals = new List<Nominal> {Nominal.One}
            };
            provider.FullDeckMatrix = provider.CreateEmptyMatrix();

            Game game = new Game(provider, 2);

            Assert.AreEqual(0, game.Score);
        }
    }
}
