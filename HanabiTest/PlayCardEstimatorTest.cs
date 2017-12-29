using System.Collections.Generic;
using System.Linq;
using Hanabi;
using NUnit.Framework;

namespace HanabiTest
{
    public class PlayCardStrategyStub : IPlayCardStrategy
    {
        public Player Player { get; set; }
        public List<CardInHand> Hand = new List<CardInHand>();
        public IDictionary<CardInHand, Probability> ProbabilitiesToReturn = new Dictionary<CardInHand, Probability>();

        public IDictionary<CardInHand, Probability> EstimateCardToPlayProbability(IBoardContext boardContext)
        {
            return ProbabilitiesToReturn;
        }

        public void AddCardInHand(Card card)
        {
            var cardInHand = new CardInHand(card, Player);

            Player.AddCard(cardInHand);
            Hand.Add(cardInHand);
        }
    }

    

    [TestFixture]
    public class PlayCardEstimatorTest
    {
        private PlayCardStrategyStub InitPlayStrategy(IGameProvider gameProvider)
        {
            var playCardStrategyMock = new PlayCardStrategyStub();
            var game = new Game(gameProvider, 4);
            var player = new Player(game, "");
            playCardStrategyMock.Player = player;

            return playCardStrategyMock;
        }

        private void SetProbabilities(PlayCardStrategyStub strategy, Dictionary<Card, double> dict)
        {
            foreach (var entry in dict)
            {
                strategy.ProbabilitiesToReturn.Add(
                    strategy.Hand.First(cih => cih.Card == entry.Key), 
                    new Probability(entry.Value));
            }
        }
        
        /// <summary>
        /// Нет карт с тонкой подсказкой.
        /// У всех карт вероятность быть сыгранной ниже порога.
        /// Ожидается, что будет возвращён пустой список
        /// </summary>
        [Test]
        public void PlayCardEstimator_NoSubtleCluesAndAllProbabilitiesLessThanThreshold_ReturnsEmpty()
        {
            var blueOneCard = new Card(Color.Blue, Rank.One);
            var whiteTwoCard = new Card(Color.White, Rank.Two);
            var blueThreeCard = new Card(Color.Blue, Rank.Three);
            var whiteFourCard = new Card(Color.White, Rank.Four);

            var gameProvider = GameProviderFabric.Create(Color.White, Color.Blue);
            var playStrategyStub = InitPlayStrategy(gameProvider);
            playStrategyStub.AddCardInHand(blueOneCard);
            playStrategyStub.AddCardInHand(whiteTwoCard);
            playStrategyStub.AddCardInHand(blueThreeCard);
            playStrategyStub.AddCardInHand(whiteFourCard);

            var dict = new Dictionary<Card, double>
            {
                {blueOneCard, 0.2},
                {whiteTwoCard, 0.3},
                {blueThreeCard, 0.4},
                {whiteFourCard, 0.5}
            };
            SetProbabilities(playStrategyStub, dict);

            var boardContext = new BoardContextStub();
                //new BoardContext(new FireworkPile(gameProvider), new Card[0], new Card[0]);
            var playerContext = PlayerContextFabric.CreateStub(playStrategyStub.Player, playStrategyStub.Hand);

            var playCardEstimator = new PlayCardEstimator(playStrategyStub);
            var possibleCards = playCardEstimator.GetPossibleCards(boardContext, playerContext);

            Assert.IsTrue(!possibleCards.Any());
        }

        /// <summary>
        /// По двум картам есть тонкие подсказки. У них разная вероятность.
        /// По остальным картам нет тонких подсказок. У них вероятность выше.
        /// Ожидается, что будут возвращены карты с тонкими подсказками.
        /// </summary>
        [Test]
        public void PlayCardEstimator_TwoSubtleCluesWithDifferentProbabilies_ReturnsTwoCards()
        {
            var blueOneCard = new Card(Color.Blue, Rank.One);
            var whiteTwoCard = new Card(Color.White, Rank.Two);
            var blueThreeCard = new Card(Color.Blue, Rank.Three);
            var whiteFourCard = new Card(Color.White, Rank.Four);

            var gameProvider = GameProviderFabric.Create(Color.White, Color.Blue);
            var playStrategyStub = InitPlayStrategy(gameProvider);
            playStrategyStub.AddCardInHand(blueOneCard);
            playStrategyStub.AddCardInHand(whiteTwoCard);
            playStrategyStub.AddCardInHand(blueThreeCard);
            playStrategyStub.AddCardInHand(whiteFourCard);

            var dict = new Dictionary<Card, double>
            {
                {blueOneCard, 0.2},
                {whiteTwoCard, 0.3},
                {blueThreeCard, 0.4},
                {whiteFourCard, 0.5}
            };

            SetProbabilities(playStrategyStub, dict);

            var playerContext = PlayerContextFabric.CreateStub(playStrategyStub.Player, playStrategyStub.Hand);
            playerContext.IsSubtleCluePredicate =
                cardInHand => new [] {blueOneCard, whiteTwoCard}.Contains(cardInHand.Card);

            var boardContext = new BoardContextStub();
            
                //new BoardContext(new FireworkPile(gameProvider), new Card[0], new Card[0]);

            var playCardEstimator = new PlayCardEstimator(playStrategyStub);
            var possibleCards = playCardEstimator.GetPossibleCards(boardContext, playerContext);

            Assert.AreEqual(2, possibleCards.Count);
            Assert.IsTrue(possibleCards.Contains(blueOneCard));
            Assert.IsTrue(possibleCards.Contains(whiteTwoCard));
        }
    }
}
