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
                Numbers = new List<Number> {Number.One, Number.Two, Number.Three, Number.Four},
            };
            var discardPile = new DiscardPile(provider);

            var blueThreeCard = new Card(Color.Blue, Number.Three);

            bool added = discardPile.AddCard(blueThreeCard);
            Assert.IsTrue(added);
        }
    }
}