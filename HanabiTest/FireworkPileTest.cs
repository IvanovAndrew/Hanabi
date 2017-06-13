using System;
using System.Collections.Generic;
using NUnit.Framework;
using Hanabi;

namespace HanabiTest
{
    [TestFixture]
    public class FireworkPileTest
    {
        [Test]
        public void AddCard_AddGreenOneToEmptyFireworkPile_Added()
        {
            var pile = new FireworkPile();
            var greenOneCard = new GreenCard(Number.One);
            
            var added = pile.AddCard(greenOneCard);
            Assert.IsTrue(added);
        }

        [Test]
        public void AddCard_AddWhiteTwoToEmptyFireworkPile_NotAdded()
        {
            var pile = new FireworkPile();
            var whiteTwoCard = new WhiteCard(Number.Two);

            var added = pile.AddCard(whiteTwoCard);
            Assert.IsFalse(added);
        }

        [Test]
        public void ToMatrix_EmptyFireworkPile_ZeroMatrix()
        {
            FireworkPile pile = new FireworkPile();
            int[,] actual = pile.ToMatrix();

            int[,] expected = new int[5, 5]
            {
                {0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0},
            };

            TestHelper.AreMatrixEqual(expected, actual);
        }

        [Test]
        public void ToMatrix_FireworkPileWithBlueOneCardOnly_MatrixWithOne()
        {
            FireworkPile pile = new FireworkPile();
            pile.AddCard(new BlueCard(Number.One));
            int[,] actual = pile.ToMatrix();

            int[,] expected = new int[5, 5]
            {
                {1, 0, 0, 0, 0},
                {0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0},
            };

            TestHelper.AreMatrixEqual(expected, actual);
        }

        [Test]
        public void GetExpectedCards_EmptyFireworkPile_ListWithFiveElements()
        {
            var fireworkPile = new FireworkPile();

            IReadOnlyList<Card> actual = fireworkPile.GetExpectedCards();

            Assert.AreEqual(5, actual.Count);
        }

        [Test]
        public void GetExpectedCards_FireworkPileWithFullRedFirework_ListWithFourElements()
        {
            var fireworkPile = new FireworkPile();
            fireworkPile.AddCard(new RedCard(Number.One));
            fireworkPile.AddCard(new RedCard(Number.Two));
            fireworkPile.AddCard(new RedCard(Number.Three));
            fireworkPile.AddCard(new RedCard(Number.Four));
            fireworkPile.AddCard(new RedCard(Number.Five));

            IReadOnlyList<Card> actual = fireworkPile.GetExpectedCards();

            Assert.AreEqual(4, actual.Count);
        }

        [Test]
        public void GetExpectedCards_FullFireworkPile_EmptyList()
        {
            var fireworkPile = new FireworkPile();
            fireworkPile.AddCard(new BlueCard(Number.One));
            fireworkPile.AddCard(new BlueCard(Number.Two));
            fireworkPile.AddCard(new BlueCard(Number.Three));
            fireworkPile.AddCard(new BlueCard(Number.Four));
            fireworkPile.AddCard(new BlueCard(Number.Five));

            fireworkPile.AddCard(new GreenCard(Number.One));
            fireworkPile.AddCard(new GreenCard(Number.Two));
            fireworkPile.AddCard(new GreenCard(Number.Three));
            fireworkPile.AddCard(new GreenCard(Number.Four));
            fireworkPile.AddCard(new GreenCard(Number.Five));

            fireworkPile.AddCard(new RedCard(Number.One));
            fireworkPile.AddCard(new RedCard(Number.Two));
            fireworkPile.AddCard(new RedCard(Number.Three));
            fireworkPile.AddCard(new RedCard(Number.Four));
            fireworkPile.AddCard(new RedCard(Number.Five));

            fireworkPile.AddCard(new YellowCard(Number.One));
            fireworkPile.AddCard(new YellowCard(Number.Two));
            fireworkPile.AddCard(new YellowCard(Number.Three));
            fireworkPile.AddCard(new YellowCard(Number.Four));
            fireworkPile.AddCard(new YellowCard(Number.Five));

            fireworkPile.AddCard(new WhiteCard(Number.One));
            fireworkPile.AddCard(new WhiteCard(Number.Two));
            fireworkPile.AddCard(new WhiteCard(Number.Three));
            fireworkPile.AddCard(new WhiteCard(Number.Four));
            fireworkPile.AddCard(new WhiteCard(Number.Five));

            IReadOnlyList<Card> actual = fireworkPile.GetExpectedCards();

            Assert.IsEmpty(actual);
        }
    }
}
