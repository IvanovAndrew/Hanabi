using Hanabi;
using NUnit.Framework;

namespace HanabiTest
{
    [TestFixture]
    public class PlayerTest
    {
        [Test]
        public void KnowAboutNominalAndColor_GivenCluesAboutNominalAndColor_ReturnsTrue()
        {
            Game game = new Game(new GameMode(2));
            Player player = new Player(game);

            player.AddCard(new BlueCard(Number.One));

            player.ListenClue(new []{new BlueCard(Number.One)}, new IsValue(Number.One));
            player.ListenClue(new []{new BlueCard(Number.One)}, new IsColor(Color.Blue));

            Assert.IsTrue(player.KnowAllAboutNominalAndColor(new BlueCard(Number.One)));
        }

        [Test]
        public void KnowAboutNominalAndColor_GivenClueAboutNominalOnly_ReturnsFalse()
        {
            Game game = new Game(new GameMode(2));
            Player player = new Player(game);

            player.AddCard(new BlueCard(Number.One));

            player.ListenClue(new[] { new BlueCard(Number.One) }, new IsValue(Number.One));

            Assert.IsFalse(player.KnowAllAboutNominalAndColor(new BlueCard(Number.One)));
        }

        [Test]
        public void KnowAboutNominalAndColor_GivenClueAboutColorOnly_ReturnsFalse()
        {
            Game game = new Game(new GameMode(2));
            Player player = new Player(game);

            player.AddCard(new BlueCard(Number.One));

            player.ListenClue(new[] { new BlueCard(Number.One) }, new IsColor(Color.Blue));

            Assert.IsFalse(player.KnowAllAboutNominalAndColor(new BlueCard(Number.One)));
        }

        [Test]
        public void KnowAboutNominalAndColor_GivenCluesAboutAllOtherColorsAndNominal_ReturnsTrue()
        {
            Game game = new Game(new GameMode(2));
            Player player = new Player(game);

            player.AddCard(new BlueCard(Number.One));
            player.AddCard(new RedCard(Number.Two));
            player.AddCard(new YellowCard(Number.Three));
            player.AddCard(new WhiteCard(Number.Four));
            player.AddCard(new GreenCard(Number.Five));

            // clue about nominal blue one
            player.ListenClue(new[] { new BlueCard(Number.One) }, new IsValue(Number.One));

            // clues about red, yellow, white and green colors
            player.ListenClue(new[] { new RedCard(Number.Two),  }, new IsColor(Color.Red));
            player.ListenClue(new[] { new YellowCard(Number.Three), }, new IsColor(Color.Yellow));
            player.ListenClue(new[] { new WhiteCard(Number.Four), }, new IsColor(Color.White));
            player.ListenClue(new[] { new GreenCard(Number.Five), }, new IsColor(Color.Green));

            Assert.IsTrue(player.KnowAllAboutNominalAndColor(new BlueCard(Number.One)));
        }
    }
}
