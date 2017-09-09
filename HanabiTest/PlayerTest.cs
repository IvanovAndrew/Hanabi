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

            CardInHand card = player.AddCardToHand(new Card(Nominal.One, Color.Blue));

            player.ListenClue(new []{card}, new ClueAboutNominal(Nominal.One));
            player.ListenClue(new []{card}, new ClueAboutColor(Color.Blue));

            Assert.IsTrue(player.KnowAllAboutNominalAndColor(card));
        }

        [Test]
        public void KnowAboutNominalAndColor_GivenClueAboutNominalOnly_ReturnsFalse()
        {
            var factory = new GameProvider();

            Game game = new Game(factory, 2);
            Player player = new Player(game, factory);

            CardInHand card = player.AddCardToHand(new Card(Color.Blue, Nominal.One));

            player.ListenClue(new[] { card }, new ClueAboutNominal(Nominal.One));

            Assert.IsFalse(player.KnowAllAboutNominalAndColor(card));
        }

        [Test]
        public void KnowAboutNominalAndColor_GivenClueAboutColorOnly_ReturnsFalse()
        {
            var factory = new GameProvider();
            Game game = new Game(factory, 2);
            Player player = new Player(game, factory);

            CardInHand card = player.AddCardToHand(new Card(Color.Blue, Nominal.One));

            player.ListenClue(new[] { card }, new ClueAboutColor(Color.Blue));

            Assert.IsFalse(player.KnowAllAboutNominalAndColor(card));
        }

        [Test]
        public void KnowAboutNominalAndColor_GivenCluesAboutAllOtherColorsAndNominal_ReturnsTrue()
        {
            var factory = new GameProvider();
            Game game = new Game(factory, 2);
            Player player = new Player(game, factory);

            CardInHand blueOneCard = player.AddCardToHand(new Card(Color.Blue, Nominal.One));
            CardInHand redTwoCard = player.AddCardToHand(new Card(Color.Red, Nominal.Two));
            CardInHand yellowThreeCard = player.AddCardToHand(new Card(Color.Yellow, Nominal.Three));
            CardInHand whiteFourCard = player.AddCardToHand(new Card(Color.White, Nominal.Four));
            CardInHand greenFiveCard = player.AddCardToHand(new Card(Color.Green, Nominal.Five));

            // clue about nominal blue one
            player.ListenClue(new[] { blueOneCard }, new ClueAboutNominal(Nominal.One));

            // clues about red, yellow, white and green colors
            player.ListenClue(new[] { redTwoCard  }, new ClueAboutColor(Color.Red));
            player.ListenClue(new[] { yellowThreeCard }, new ClueAboutColor(Color.Yellow));
            player.ListenClue(new[] { whiteFourCard }, new ClueAboutColor(Color.White));
            player.ListenClue(new[] { greenFiveCard }, new ClueAboutColor(Color.Green));

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

            CardInHand firstWhiteOneCard = player.AddCardToHand(new Card(Color.White, Nominal.One));
            CardInHand secondWhiteOneCard = player.AddCardToHand(new Card(Color.White, Nominal.One));

            Clue clue = new ClueAboutNominal(Nominal.One);

            player.ListenClue(new List<CardInHand>
            {
                firstWhiteOneCard,
                secondWhiteOneCard
            }, clue);

            foreach (var card in player.Memory.GetHand())
            {
                IReadOnlyList<Clue> previousClues = player.Memory.GetPreviousCluesAboutCard(card);

                Assert.AreEqual(1, previousClues.Count);

                Assert.IsTrue(previousClues[0] is ClueAboutNominal);
            }
        }

        // TODO create more accurate name for test
        [Test]
        public void ListenClue_HandWithRedFiveAndClueAboutFive_AddsClueAboutOtherCards()
        {
            IGameProvider provider = new GameProvider();
            Game game = new Game(provider, 2);

            var player = new Player(game, provider);

            CardInHand blueOneCard = player.AddCardToHand(new Card(Color.Blue, Nominal.One));
            CardInHand redFiveCard = player.AddCardToHand(new Card(Color.Red, Nominal.Five));

            Clue clue = new ClueAboutNominal(Nominal.Five);

            player.ListenClue(new List<CardInHand>
            {
                redFiveCard,
            }, clue);

            IReadOnlyList<Clue> previousClues = player.Memory.GetPreviousCluesAboutCard(blueOneCard);

            Assert.AreEqual(1, previousClues.Count);
            Assert.IsTrue(previousClues[0] is ClueAboutNotNominal);
        }

        public void GiveClue_Always_RaisesClueGivenEvent()
        {
            
        }
    }
}
