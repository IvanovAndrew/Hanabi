using System.Collections.Generic;
using System.Linq;
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
            Player player = new Player(game);

            CardInHand cardInHand = new CardInHand(player, new Card(Rank.One, Color.Blue));
            player.AddCard(cardInHand);

            var cardsToClue = new[] {cardInHand};
            var clueOne = Clue.Create(new ClueAboutRank(Rank.One), cardsToClue);
            var clueTwo = Clue.Create(new ClueAboutColor(Color.Blue), cardsToClue);

            player.ListenClue(clueOne);
            player.ListenClue(clueTwo);

            Assert.IsTrue(player.KnowAboutNominalAndColor(cardInHand));
        }

        [Test]
        public void KnowAboutNominalAndColor_GivenClueAboutNominalOnly_ReturnsFalse()
        {
            var gameProvider = new GameProvider();

            Game game = new Game(gameProvider, 2);
            Player player = new Player(game);

            CardInHand card = new CardInHand(player, new Card(Color.Blue, Rank.One));
            player.AddCard(card);

            var clue = Clue.Create(new ClueAboutRank(Rank.One), new[] {card});

            // act
            player.ListenClue(clue);

            Assert.IsFalse(player.KnowAboutNominalAndColor(card));
        }

        [Test]
        public void KnowAboutNominalAndColor_GivenClueAboutColorOnly_ReturnsFalse()
        {
            var gameProvider = new GameProvider();
            Game game = new Game(gameProvider, 2);
            Player player = new Player(game);

            CardInHand card = new CardInHand(player, new Card(Color.Blue, Rank.One));
            player.AddCard(card);

            // act
            var clue = Clue.Create(new ClueAboutColor(Color.Blue), new[] {card});
            player.ListenClue(clue);

            // assert
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
            var clue = Clue.Create(new ClueAboutRank(Rank.One), new[] { blueOneCard });
            player.ListenClue(clue);

            // clues about red, yellow, white and green colors
            player.ListenClue(Clue.Create(new ClueAboutColor(Color.Red),    new[] { redTwoCard }));
            player.ListenClue(Clue.Create(new ClueAboutColor(Color.Yellow), new[] { yellowThreeCard }));
            player.ListenClue(Clue.Create(new ClueAboutColor(Color.White),  new[] { whiteFourCard }));
            player.ListenClue(Clue.Create(new ClueAboutColor(Color.Green),  new[] { greenFiveCard }));

            Assert.IsTrue(player.KnowAboutNominalAndColor(blueOneCard));
        }

        #endregion

        // TODO create more accurate name for test
        [Test]
        public void ListenClue_HandWithTwoWhiteOneCardsAndClueAboutOne_AddsOneClueToEachCard()
        {
            IGameProvider provider = new GameProvider();
            Game game = new Game(provider, 2);

            var player = new Player(game);

            CardInHand firstWhiteOneCard = new CardInHand(player, new Card(Color.White, Rank.One));
            player.AddCard(firstWhiteOneCard);

            CardInHand secondWhiteOneCard = new CardInHand(player, new Card(Color.White, Rank.One));
            player.AddCard(secondWhiteOneCard);

            ClueType clue = new ClueAboutRank(Rank.One);

            //act
            player.ListenClue(
                Clue.Create(clue, new List<CardInHand> {firstWhiteOneCard, secondWhiteOneCard}));

            // assert
            Assert.AreEqual(1, player.GetCluesAboutCard(firstWhiteOneCard).Count);
            Assert.AreEqual(1, player.GetCluesAboutCard(secondWhiteOneCard).Count);
        }

        // TODO create more accurate name for test
        [Test]
        public void ListenClue_HandWithRedFiveAndClueAboutFive_AddsClueAboutOtherCards()
        {
            IGameProvider provider = new GameProvider();
            Game game = new Game(provider, 2);

            var player = new Player(game);

            CardInHand blueOneCard = new CardInHand(player, new Card(Color.Blue, Rank.One));
            player.AddCard(blueOneCard);

            CardInHand redFiveCard = new CardInHand(player, new Card(Color.Red, Rank.Five));
            player.AddCard(redFiveCard);

            // act
            player.ListenClue(Clue.Create(new ClueAboutRank(Rank.Five), new [] { redFiveCard }));

            IReadOnlyList<ClueType> clues = player.GetCluesAboutCard(blueOneCard);

            Assert.AreEqual(1, clues.Count);
            Assert.IsTrue(clues[0] is ClueAboutNotRank);
        }

        public void GiveClue_Always_RaisesClueGivenEvent()
        {
            
        }
    }
}
