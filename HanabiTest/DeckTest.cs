using Hanabi;
using NUnit;
using NUnit.Framework;

namespace HanabiTest
{
    [TestFixture]
    public class DeckTest
    {
        [Test]
        public void Deck_New_NotEmpty()
        {
            var deck = Deck.Create();
            Assert.IsFalse(deck.IsEmpty());
        }

        [Test]
        public void Deck_Drop50Cards_EmptyDeck()
        {
            var deck = Deck.Create();
            
            for(int i = 0; i < 50; i++)
            {
                deck.PopCard();
            }
            
            Assert.IsTrue(deck.IsEmpty());
        }

        [Test]
        public void ExtendedDeck_Drop55Cards_EmptyDeck()
        {
            var deck = Deck.Create(true);

            for (int i = 0; i < 55; i++)
            {
                deck.PopCard();
            }

            Assert.IsTrue(deck.IsEmpty());
        }
    }
}
