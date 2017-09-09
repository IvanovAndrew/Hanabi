using System;
using System.Collections.Generic;
using Hanabi;
using NUnit.Framework;

namespace HanabiTest
{
    [TestFixture]
    public class DeckTest
    {
        [Test]
        public void IsEmpty_EmptyList_ReturnsTrue()
        {
            var deck = new Deck(new List<Card>());
            Assert.IsTrue(deck.IsEmpty());
        }

        [Test]
        public void PopCard_EmptyDeck_ThrowsInvalidOperationException()
        {
            var deck = new Deck(new Card[0]);
            InvalidOperationException exception = Assert.Catch<InvalidOperationException>(() => deck.PopCard());

            StringAssert.Contains("is empty", exception.Message);
        }
    }
}
