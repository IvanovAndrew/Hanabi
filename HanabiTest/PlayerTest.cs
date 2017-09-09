using System.Collections.Generic;
using Hanabi;
using NUnit.Framework;

namespace HanabiTest
{
    [TestFixture]
    public class PlayerTest
    {
        #region KnowAboutNominalAndColor method tests
        [Test]
        public void KnowAboutNominalAndColor_GivenCluesAboutNominalAndColor_ReturnsTrue()
        {
            var factory = new GameProvider();
            
            Game game = new Game(factory, 2);
            Player player = new Player(game, factory);

            CardInHand card = player.AddCardToHand(new Card(Number.One, Color.Blue));

            player.ListenClue(new []{card}, new IsNominal(Number.One));
            player.ListenClue(new []{card}, new IsColor(Color.Blue));

            Assert.IsTrue(player.KnowAllAboutNominalAndColor(card));
        }

        [Test]
        public void KnowAboutNominalAndColor_GivenClueAboutNominalOnly_ReturnsFalse()
        {
            var factory = new GameProvider();

            Game game = new Game(factory, 2);
            Player player = new Player(game, factory);

            CardInHand card = player.AddCardToHand(new Card(Color.Blue, Number.One));

            player.ListenClue(new[] { card }, new IsNominal(Number.One));

            Assert.IsFalse(player.KnowAllAboutNominalAndColor(card));
        }

        [Test]
        public void KnowAboutNominalAndColor_GivenClueAboutColorOnly_ReturnsFalse()
        {
            var factory = new GameProvider();
            Game game = new Game(factory, 2);
            Player player = new Player(game, factory);

            CardInHand card = player.AddCardToHand(new Card(Color.Blue, Number.One));

            player.ListenClue(new[] { card }, new IsColor(Color.Blue));

            Assert.IsFalse(player.KnowAllAboutNominalAndColor(card));
        }

        [Test]
        public void KnowAboutNominalAndColor_GivenCluesAboutAllOtherColorsAndNominal_ReturnsTrue()
        {
            var factory = new GameProvider();
            Game game = new Game(factory, 2);
            Player player = new Player(game, factory);

            CardInHand blueOneCard = player.AddCardToHand(new Card(Color.Blue, Number.One));
            CardInHand redTwoCard = player.AddCardToHand(new Card(Color.Red, Number.Two));
            CardInHand yellowThreeCard = player.AddCardToHand(new Card(Color.Yellow, Number.Three));
            CardInHand whiteFourCard = player.AddCardToHand(new Card(Color.White, Number.Four));
            CardInHand greenFiveCard = player.AddCardToHand(new Card(Color.Green, Number.Five));

            // clue about nominal blue one
            player.ListenClue(new[] { blueOneCard }, new IsNominal(Number.One));

            // clues about red, yellow, white and green colors
            player.ListenClue(new[] { redTwoCard  }, new IsColor(Color.Red));
            player.ListenClue(new[] { yellowThreeCard }, new IsColor(Color.Yellow));
            player.ListenClue(new[] { whiteFourCard }, new IsColor(Color.White));
            player.ListenClue(new[] { greenFiveCard }, new IsColor(Color.Green));

            Assert.IsTrue(player.KnowAllAboutNominalAndColor(blueOneCard));
        }

        #endregion

        // TODO create more accurate name for test
        [Test]
        public void ListenClue_HandWithTwoWhiteOneCardsAndClueAboutOne_AddsOneClueToEachCard()
        {
            IGameProvider provider = new GameProvider();
            Game game = new Game(provider, 2);

            var player = new Player(game, provider);

            CardInHand firstWhiteOneCard = player.AddCardToHand(new Card(Color.White, Number.One));
            CardInHand secondWhiteOneCard = player.AddCardToHand(new Card(Color.White, Number.One));

            Clue clue = new IsNominal(Number.One);

            player.ListenClue(new List<CardInHand>
            {
                firstWhiteOneCard,
                secondWhiteOneCard
            }, clue);

            foreach (var card in player.Memory.GetHand())
            {
                IReadOnlyList<Clue> previousClues = player.Memory.GetPreviousCluesAboutCard(card);

                Assert.AreEqual(1, previousClues.Count);

                Assert.IsTrue(previousClues[0] is IsNominal);
            }
        }

        // TODO create more accurate name for test
        [Test]
        public void ListenClue_HandWithRedFiveAndClueAboutFive_AddsClueAboutOtherCards()
        {
            IGameProvider provider = new GameProvider();
            Game game = new Game(provider, 2);

            var player = new Player(game, provider);

            CardInHand blueOneCard = player.AddCardToHand(new Card(Color.Blue, Number.One));
            CardInHand redFiveCard = player.AddCardToHand(new Card(Color.Red, Number.Five));

            Clue clue = new IsNominal(Number.Five);

            player.ListenClue(new List<CardInHand>
            {
                redFiveCard,
            }, clue);

            IReadOnlyList<Clue> previousClues = player.Memory.GetPreviousCluesAboutCard(blueOneCard);

            Assert.AreEqual(1, previousClues.Count);
            Assert.IsTrue(previousClues[0] is IsNotNominal);
        }

        public void GiveClue_Always_RaisesClueGivenEvent()
        {
            
        }
    }
}
