using System.Collections.Generic;
using Hanabi;
using NUnit.Framework;

namespace HanabiTest
{
    [TestFixture]
    public class DiscardPileTest
    {
        [Test]
        public void AddCard_Always_ReturnsTrue()
        {
            IGameProvider provider = new FakeGameProvider()
            {
                Colors = new List<Color>() {Color.Blue, Color.Green, Color.Red, Color.White},
                Ranks = new List<Rank> {Rank.One, Rank.Two, Rank.Three, Rank.Four},
            };
            var discardPile = new DiscardPile(provider);

            var blueThreeCard = new Card(Color.Blue, Rank.Three);

            bool added = discardPile.AddCard(blueThreeCard);
            Assert.IsTrue(added);
        }
    }
}