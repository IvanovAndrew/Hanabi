using System.Collections.Generic;
using System.Linq;
using Hanabi;
using NUnit.Framework;

namespace HanabiTest
{
    public class DiscardStrategyStub : IDiscardStrategy
    {
        public IGameProvider GameProvider { get; set; }
        public Player Player { get; set; }
        public List<CardInHand> Hand { get; set; }
        public CardProbability DiscardProbabilities { get; set; }

        public DiscardStrategyStub()
        {
            Hand = new List<CardInHand>();
            DiscardProbabilities = new CardProbability();
        }

        public CardProbability EstimateDiscardProbability(IBoardContext boardContext)
        {
            return DiscardProbabilities;
        }

        public bool CheckIfRightClue(IBoardContext boardContext, IPlayerContext playerContext)
        {
            return true;
        }

        public void AddToHand(Card card)
        {
            var cardInHand = new CardInHand(Player, card);
            Player.AddCard(cardInHand);
            Hand.Add(cardInHand);
        }
    }

    [TestFixture]
    public class DiscardEstimatorTest
    {
        public DiscardStrategyStub CreateDiscardStrategy()
        {
            var gameProvider = GameProviderFabric.Create(Color.Blue, Color.Yellow);

            DiscardStrategyStub stub = new DiscardStrategyStub
            {
                Player = new Player(new Game(gameProvider, 4), ""),
                GameProvider = gameProvider,
            };

            return stub;
        }

        private void SetProbabilities(DiscardStrategyStub discardStrategy, Dictionary<Card, double> dict)
        {
            foreach (var entry in dict)
            {
                var cardInHand = discardStrategy.Hand.First(cih => cih.Card == entry.Key);
                discardStrategy.DiscardProbabilities[cardInHand] = entry.Value;
            }
        }

        /// <summary>
        /// Про все 3 карты ничего неизвестно.
        /// У всех 3 карт одинаковая вероятность сброса.
        /// Ожидается, что метод вернёт все 3 карты.
        /// </summary>
        [Test]
        public void DiscardEstimator_AllCardsWithSameProbability_ReturnsAllCards()
        {
            var discardStrategyStub = CreateDiscardStrategy();
            var blueOneCard = new Card(Color.Blue, Rank.One);
            var yellowTwoCard = new Card(Color.Yellow, Rank.Two);
            var yellowThreeCard = new Card(Color.Yellow, Rank.Three);

            discardStrategyStub.AddToHand(blueOneCard);
            discardStrategyStub.AddToHand(yellowTwoCard);
            discardStrategyStub.AddToHand(yellowThreeCard);

            var dict = new Dictionary<Card, double>
            {
                [blueOneCard] = 0.3,
                [yellowTwoCard] = 0.3,
                [yellowThreeCard] = 0.3
            };

            SetProbabilities(discardStrategyStub, dict);
            
            var boardContext = new BoardContextStub();
                //new BoardContext(new FireworkPile(discardStrategyStub.GameProvider), new Card[0], new Card[0]);
            var playerContext = PlayerContextFabric.CreateStub(discardStrategyStub.Player, discardStrategyStub.Hand);

            // act
            var estimator = new DiscardEstimator(discardStrategyStub);
            var cards = estimator.GetPossibleCards(boardContext, playerContext);

            // arrange
            Assert.AreEqual(3, cards.Count);
        }

        /// <summary>
        /// Вход.
        /// Про все 3 карты что-то известно: ранг или цвет.
        /// Вероятность сброса у каждой карты одинакова.
        /// Ожидается, что метод вернёт все 3 карты.
        /// </summary>
        [Test]
        public void DiscardEstimator_KnowSomethingAboutAllCardsAndTheSameProbabilities_ReturnsAllCards()
        {
            var discardStrategyStub = CreateDiscardStrategy();
            
            var blueOneCard = new Card(Color.Blue, Rank.One);
            var yellowTwoCard = new Card(Color.Yellow, Rank.Two);
            var yellowThreeCard = new Card(Color.Yellow, Rank.Three);

            discardStrategyStub.AddToHand(blueOneCard);
            discardStrategyStub.AddToHand(yellowTwoCard);
            discardStrategyStub.AddToHand(yellowThreeCard);

            var dict = new Dictionary<Card, double>
            {
                [blueOneCard] = 0.33,
                [yellowTwoCard] = 0.33,
                [yellowThreeCard] = 0.33
            };
            SetProbabilities(discardStrategyStub, dict);

            var boardContext = new BoardContextStub();//new BoardContext(new FireworkPile(discardStrategyStub.GameProvider), new Card[0], new Card[0]);
            var playerContext = PlayerContextFabric.CreateStub(discardStrategyStub.Player, discardStrategyStub.Hand);
            playerContext.KnowAboutRankOrColorPredicate = cardInHand => true;


            var estimator = new DiscardEstimator(discardStrategyStub);
            var cards = estimator.GetPossibleCards(boardContext, playerContext);

            Assert.AreEqual(3, cards.Count);
        }

        /// <summary>
        /// Вход.
        /// Про все 3 карты ничего неизвестно.
        /// Одна карта имеет большую вероятность сброса, чем остальные.
        /// Ожидается, что метод вернёт карту с наибольшей вероятностью.
        /// </summary>
        [Test]
        public void DiscardEstimator_BlueOneHasMaxProbability_ReturnsBlueOne()
        {
            var discardStrategyStub = CreateDiscardStrategy();

            var blueOneCard = new Card(Color.Blue, Rank.One);
            var yellowTwoCard = new Card(Color.Yellow, Rank.Two);
            var yellowThreeCard = new Card(Color.Yellow, Rank.Three);

            discardStrategyStub.AddToHand(blueOneCard);
            discardStrategyStub.AddToHand(yellowTwoCard);
            discardStrategyStub.AddToHand(yellowThreeCard);

            var dict = new Dictionary<Card, double>
            {
                [blueOneCard] = 0.5,
                [yellowTwoCard] = 0.3,
                [yellowThreeCard] = 0.25
            };

            SetProbabilities(discardStrategyStub, dict);
            
            var boardContext = new BoardContextStub();
                //new BoardContext(new FireworkPile(discardStrategyStub.GameProvider), new Card[0], new Card[0]);
            var playerContext = PlayerContextFabric.CreateStub(discardStrategyStub.Player, discardStrategyStub.Hand);

            // Act
            var estimator = new DiscardEstimator(discardStrategyStub);
            var cards = estimator.GetPossibleCards(boardContext, playerContext);

            // Assert
            Assert.AreEqual(1, cards.Count);
            Assert.AreEqual(blueOneCard, cards[0]);
        }

        /// <summary>
        /// Вход.
        /// Про синюю единицу известен ранг. У неё самая высокая вероятность сброса (не 1.0).
        /// Про другие карты ничего не известно. У них одинаковая вероятность сброса.
        /// Ожидается, что будут возвращены карты, про которых ничего неизвестно.
        /// </summary>
        [Test]
        public void DiscardEstimator_KnowAboutBlueOneOnly_ReturnsTwoYellowCards()
        {
            var discardStrategyStub = CreateDiscardStrategy();

            var blueOneCard = new Card(Color.Blue, Rank.One);
            var yellowTwoCard = new Card(Color.Yellow, Rank.Two);
            var yellowThreeCard = new Card(Color.Yellow, Rank.Three);

            discardStrategyStub.AddToHand(blueOneCard);
            discardStrategyStub.AddToHand(yellowTwoCard);
            discardStrategyStub.AddToHand(yellowThreeCard);

            var dict = new Dictionary<Card, double>
            {
                [blueOneCard] = 0.5,
                [yellowTwoCard] = 0.25,
                [yellowThreeCard] = 0.25
            };

            SetProbabilities(discardStrategyStub, dict);
            

            var boardContext = new BoardContextStub();
                //new BoardContext(new FireworkPile(discardStrategyStub.GameProvider), new Card[0], new Card[0]);
            var playerContext = PlayerContextFabric.CreateStub(discardStrategyStub.Player, discardStrategyStub.Hand);
            playerContext.KnowAboutRankOrColorPredicate = cardInHand => cardInHand.Card == blueOneCard;

            // Act
            var estimator = new DiscardEstimator(discardStrategyStub);
            var cards = estimator.GetPossibleCards(boardContext, playerContext);

            // Assert
            Assert.AreEqual(2, cards.Count);
            Assert.IsTrue(cards.Contains(yellowTwoCard));
            Assert.IsTrue(cards.Contains(yellowThreeCard));
        }

        /// <summary>
        /// Вход.
        /// Про синюю единицу известен ранг. У неё вероятность сброса 1.0.
        /// Про другие карты ничего не известно. У них одинаковая вероятность сброса.
        /// Ожидается, что будет возвращена синяя единица.
        /// </summary>
        [Test]
        public void DiscardEstimator_KnowAboutBlueOneOnlyAndProbability10_ReturnsBlueOne()
        {
            var discardStrategyStub = CreateDiscardStrategy();

            var blueOneCard = new Card(Color.Blue, Rank.One);
            var yellowTwoCard = new Card(Color.Yellow, Rank.Two);
            var yellowThreeCard = new Card(Color.Yellow, Rank.Three);

            discardStrategyStub.AddToHand(blueOneCard);
            discardStrategyStub.AddToHand(yellowTwoCard);
            discardStrategyStub.AddToHand(yellowThreeCard);

            var dict = new Dictionary<Card, double>
            {
                [blueOneCard] = 1.0,
                [yellowTwoCard] = 0.25,
                [yellowThreeCard] = 0.25
            };

            SetProbabilities(discardStrategyStub, dict);

            var boardContext = new BoardContextStub();
                //new BoardContext(new FireworkPile(discardStrategyStub.GameProvider), new Card[0], new Card[0]);
            var playerContext = PlayerContextFabric.CreateStub(discardStrategyStub.Player, discardStrategyStub.Hand);
            playerContext.KnowAboutRankOrColorPredicate = cardInHand => cardInHand.Card == blueOneCard;

            // Act
            var estimator = new DiscardEstimator(discardStrategyStub);
            var cards = estimator.GetPossibleCards(boardContext, playerContext);

            // Assert
            Assert.AreEqual(1, cards.Count);
            Assert.IsTrue(cards.Contains(blueOneCard));
        }
    }
}