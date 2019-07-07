using Hanabi;
using NUnit.Framework;

namespace HanabiTest
{
    [TestFixture]
    class PlayCardStrategyTest
    {
        /// <summary>
        /// Игрок знает, что карта не не тройка, не четвёрка и не пятёрка.
        /// Также на руках других игроков и в сбросе все допустимые двойки. 
        /// Ожидается, что вероятность хода единицей максимальна.
        /// </summary>
        [Test]
        public void EstimateCardToPlayProbability_PossibleOneOnly_ReturnsMaximumProbability()
        {
            IGameProvider gameProvider = GameProviderFabric.Create(Color.Blue, Color.Green);

            Game game = new Game(gameProvider, 2);
            var player = new Player(game);

            var blueOneCard = new Card(Color.Blue, Rank.One);
            var blueOneInHand = new CardInHand(player, blueOneCard);

            var guess = new Guess(gameProvider, blueOneInHand);
            guess.Visit(new ClueAboutNotRank(Rank.Five));
            guess.Visit(new ClueAboutNotRank(Rank.Four));
            guess.Visit(new ClueAboutNotRank(Rank.Three));

            var playStrategy = new PlayCardStrategy(new []{guess});

            var boardContext = new BoardContextStub
            {
                ExcludedCards = new []
                {
                    new Card(Color.Blue, Rank.Two),
                    new Card(Color.Blue, Rank.Two),
                    new Card(Color.Green, Rank.Two),
                    new Card(Color.Green, Rank.Two), 
                },
                ExpectedCards = new[] {new Card(Color.Blue, Rank.One), new Card(Color.Green, Rank.One),},
            };

            var result = playStrategy.EstimateCardToPlayProbability(boardContext);
            Assert.AreEqual(Probability.Maximum, result[blueOneInHand]);
        }

        /// <summary>
        /// Игрок знает, что карта не не тройка, не четвёрка и не пятёрка. 
        /// Также на руках других игроков и в сбросе все допустимые двойки. 
        /// Ожидается, что вероятность хода единицей максимальна.
        /// </summary>
        [Test]
        public void EstimateCardToPlayProbability_PossibleOneOnly_ReturnsMinimumProbability()
        {
            IGameProvider gameProvider = GameProviderFabric.Create(Color.Blue, Color.Green);

            Game game = new Game(gameProvider, 2);
            var player = new Player(game);

            var blueOneCard = new Card(Color.Blue, Rank.One);
            var blueOneInHand = new CardInHand(player, blueOneCard);

            var guess = new Guess(gameProvider, blueOneInHand);
            guess.Visit(new ClueAboutNotRank(Rank.Five));
            guess.Visit(new ClueAboutNotRank(Rank.Four));
            guess.Visit(new ClueAboutNotRank(Rank.Three));
            guess.Visit(new ClueAboutNotRank(Rank.Two));

            var playStrategy = new PlayCardStrategy(new[] { guess });

            var boardContext = new BoardContextStub
            {
                ExcludedCards = new[]
                {
                    new Card(Color.Blue, Rank.One),
                    new Card(Color.Green, Rank.One),
                },
                ExpectedCards = new[] { new Card(Color.Blue, Rank.Two), new Card(Color.Green, Rank.Two), },
            };

            var result = playStrategy.EstimateCardToPlayProbability(boardContext);
            Assert.AreEqual(Probability.Minimum, result[blueOneInHand]);
        }
    }
}
