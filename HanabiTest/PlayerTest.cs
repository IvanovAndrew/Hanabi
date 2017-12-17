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
            var gameProvider = new GameProvider();
            
            Game game = new Game(gameProvider, 2);
            Player player = new Player(game, "");

            CardInHand cardInHand = new CardInHand(player, new Card(Rank.One, Color.Blue));
            player.AddCard(cardInHand);

            player.ListenClue(new []{cardInHand}, new ClueAboutRank(Rank.One));
            player.ListenClue(new []{cardInHand}, new ClueAboutColor(Color.Blue));

            Assert.IsTrue(player.KnowAboutNominalAndColor(cardInHand));
        }

        [Test]
        public void KnowAboutNominalAndColor_GivenClueAboutNominalOnly_ReturnsFalse()
        {
            var gameProvider = new GameProvider();

            Game game = new Game(gameProvider, 2);
            Player player = new Player(game, "");

            CardInHand card = new CardInHand(player, new Card(Color.Blue, Rank.One));
            player.AddCard(card);

            // act
            player.ListenClue(new[] { card }, new ClueAboutRank(Rank.One));

            Assert.IsFalse(player.KnowAboutNominalAndColor(card));
        }

        [Test]
        public void KnowAboutNominalAndColor_GivenClueAboutColorOnly_ReturnsFalse()
        {
            var gameProvider = new GameProvider();
            Game game = new Game(gameProvider, 2);
            Player player = new Player(game, "");

            CardInHand card = new CardInHand(player, new Card(Color.Blue, Rank.One));
            player.AddCard(card);

            player.ListenClue(new[] { card }, new ClueAboutColor(Color.Blue));

            Assert.IsFalse(player.KnowAboutNominalAndColor(card));
        }

        [Test]
        public void KnowAboutNominalAndColor_GivenCluesAboutAllOtherColorsAndNominal_ReturnsTrue()
        {
            var gameProvider = new GameProvider();
            Game game = new Game(gameProvider, 2);
            Player player = new Player(game, "");

            CardInHand blueOneCard = new CardInHand(player, new Card(Color.Blue, Rank.One));
            player.AddCard(blueOneCard);

            CardInHand redTwoCard = new CardInHand(player, new Card(Color.Red, Rank.Two));
            player.AddCard(redTwoCard);

            CardInHand yellowThreeCard = new CardInHand(player, new Card(Color.Yellow, Rank.Three));
            player.AddCard(yellowThreeCard);

            CardInHand whiteFourCard = new CardInHand(player, new Card(Color.White, Rank.Four));
            player.AddCard(whiteFourCard);

            CardInHand greenFiveCard = new CardInHand(player, new Card(Color.Green, Rank.Five));
            player.AddCard(greenFiveCard);

            // clue about rank blue one
            player.ListenClue(new[] { blueOneCard }, new ClueAboutRank(Rank.One));

            // clues about red, yellow, white and green colors
            player.ListenClue(new[] { redTwoCard  }, new ClueAboutColor(Color.Red));
            player.ListenClue(new[] { yellowThreeCard }, new ClueAboutColor(Color.Yellow));
            player.ListenClue(new[] { whiteFourCard }, new ClueAboutColor(Color.White));
            player.ListenClue(new[] { greenFiveCard }, new ClueAboutColor(Color.Green));

            Assert.IsTrue(player.KnowAboutNominalAndColor(blueOneCard));
        }

        #endregion

        // TODO create more accurate name for test
        //[Test]
        public void ListenClue_HandWithTwoWhiteOneCardsAndClueAboutOne_AddsOneClueToEachCard()
        {
            IGameProvider provider = new GameProvider();
            Game game = new Game(provider, 2);

            var player = new Player(game, "");

            CardInHand firstWhiteOneCard = new CardInHand(player, new Card(Color.White, Rank.One));
            player.AddCard(firstWhiteOneCard);

            CardInHand secondWhiteOneCard = new CardInHand(player, new Card(Color.White, Rank.One));
            player.AddCard(firstWhiteOneCard);

            Clue clue = new ClueAboutRank(Rank.One);

            player.ListenClue(new List<CardInHand>
            {
                firstWhiteOneCard,
                secondWhiteOneCard
            }, clue);

            //foreach (var card in player.Memory.GetHand())
            //{
            //    IReadOnlyList<Clue> clues = player.Memory.GetCluesAboutCard(card);

            //    Assert.AreEqual(1, clues.Count);

            //    Assert.IsTrue(clues[0] is ClueAboutRank);
            //}
        }

        // TODO create more accurate name for test
        //[Test]
        public void ListenClue_HandWithRedFiveAndClueAboutFive_AddsClueAboutOtherCards()
        {
            IGameProvider provider = new GameProvider();
            Game game = new Game(provider, 2);

            var player = new Player(game, "");

            CardInHand blueOneCard = new CardInHand(player, new Card(Color.Blue, Rank.One));
            player.AddCard(blueOneCard);

            CardInHand redFiveCard = new CardInHand(player, new Card(Color.Red, Rank.Five));
            player.AddCard(redFiveCard);

            Clue clue = new ClueAboutRank(Rank.Five);

            player.ListenClue(new List<CardInHand>
            {
                redFiveCard,
            }, clue);

            //IReadOnlyList<Clue> clues = player.Memory.GetCluesAboutCard(blueOneCard);

            //Assert.AreEqual(1, clues.Count);
            //Assert.IsTrue(clues[0] is ClueAboutNotRank);
        }

        public void GiveClue_Always_RaisesClueGivenEvent()
        {
            
        }
    }
}
