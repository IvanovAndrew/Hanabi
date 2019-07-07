using System.Linq;
using Hanabi;
using NUnit.Framework;

namespace HanabiTest
{
    [TestFixture]
    public class ClueToPlayFinderTest
    {
        private Player GetPlayer()
        {
            var gameProvider = new GameProvider();
            var game = new Game(gameProvider, 2);
            return new Player(game);
        }

        [Test]
        public void Find_NoCardsToPlay_ReturnsNull()
        {
            var boardContext = new BoardContextStub
            {
                ExpectedCards = new[] {new Card(Color.Blue, Rank.One),}
            };

            var player = GetPlayer();

            var playerContext = new PlayerContextStub
            {
                Player = player,
                Hand = new [] {new CardInHand(new Card(Color.Yellow, Rank.Two), player)},
            };

            // action
            var clueToPlayFinder = new ClueToPlayFinder(boardContext, playerContext);
            var result = clueToPlayFinder.Find();

            // assert
            Assert.IsNull(result);
        }

        [Test]
        public void Find_CanPlayFiveAndOneRankedCards_ReturnFiveRankedCard()
        {
            var boardContext = new BoardContextStub
            {
                ExpectedCards = new[]
                {
                    new Card(Color.Blue, Rank.One),
                    new Card(Color.Green, Rank.Five),
                }
            };

            var player = GetPlayer();

            var playerContext = new PlayerContextStub
            {
                Player = player,
                Hand = new[]
                {
                    new CardInHand(new Card(Color.Yellow, Rank.Two), player),
                    new CardInHand(new Card(Color.Blue, Rank.One), player), 
                    new CardInHand(new Card(Color.Green, Rank.Five), player), 
                },
            };

            // action
            var clueToPlayFinder = new ClueToPlayFinder(boardContext, playerContext);
            var result = clueToPlayFinder.Find();

            // assert
            Assert.IsTrue((result.Action.Outcome & OutcomeFlags.Play) > 0);
            Assert.IsTrue(result.Action.Cards.Any());
            Assert.IsTrue(result.Action.Cards.Any(c => c.Equals(new Card(Color.Green, Rank.Five))));
        }
    }
}
