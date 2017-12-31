using Hanabi;
using NUnit.Framework;

namespace HanabiTest
{
    [TestFixture]
    class DiscardStrategyTest
    {
        private Guess CreateGuess(IGameProvider gameProvider, Card card)
        {
            var game = new Game(gameProvider, 2);
            var player = new Player(game);

            var cardInHand = new CardInHand(card, player);

            return new Guess(gameProvider, cardInHand);
        }
        
        //[Test]
        void EstimateDiscardProbability__()
        {
            // arrange
            var provider = GameProviderFabric.Create(Color.Blue, Color.Green);
            var guessAboutBlueThree = CreateGuess(provider, new Card(Color.Blue, Rank.Three));
            var guessAboutGreenTwo = CreateGuess(provider, new Card(Color.Green, Rank.Two));
            var discardStrategy = new DiscardStrategy(new []{guessAboutGreenTwo, guessAboutBlueThree});

            var boardContext = new BoardContextStub();

            //act
            discardStrategy.EstimateDiscardProbability(boardContext);
            Assert.Fail();
        }
    }
}
