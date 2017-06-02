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
        public void EmptyFireworkPile_GetScore_Zero()
        {
            var pile = new FireworkPile();

            Assert.AreEqual(0, pile.GetScore());
        }

        [Test]
        public void FireworkPileWithOneCard_GetScore_One()
        {
            var pile = new FireworkPile();
            pile.AddCard(new BlueCard { Nominal = Number.One });

            Assert.AreEqual(1, pile.GetScore());
        }

        [Test]
        public void EmptyFireworkPile_AddGreenOne_Added()
        {
            var pile = new FireworkPile();
            var greenOneCard = new GreenCard{Nominal = Number.One};
            
            var added = pile.AddCard(greenOneCard);
            Assert.IsTrue(added);
        }

        [Test]
        public void EmptyFireworkPile_AddWhiteTwo_NotAdded()
        {
            var pile = new FireworkPile();
            var whiteTwoCard = new WhiteCard { Nominal = Number.Two };

            var added = pile.AddCard(whiteTwoCard);
            Assert.IsFalse(added);
        }
    }
}
