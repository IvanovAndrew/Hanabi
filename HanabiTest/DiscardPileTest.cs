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
                Nominals = new List<Nominal> {Nominal.One, Nominal.Two, Nominal.Three, Nominal.Four},
            };
            var discardPile = new DiscardPile(provider);

            var blueThreeCard = new Card(Color.Blue, Nominal.Three);

            bool added = discardPile.AddCard(blueThreeCard);
            Assert.IsTrue(added);
        }
    }
}