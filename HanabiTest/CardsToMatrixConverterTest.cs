using System.Collections.Generic;
using Hanabi;
using NUnit.Framework;

namespace HanabiTest
{
    [TestFixture]
    public class CardsToMatrixConverterTest
    {
        [Test]
        public void Encode_ZeroCards_ReturnsZeroMatrix()
        {
            var deckMatrixProvider = new FakeGameProvider
            {
                Colors = new List<Color> { Color.Blue, Color.Green, Color.Red },
                Ranks = new List<Rank> { Rank.One, Rank.Two, Rank.Three, }
            };

            Matrix emptyMatrix = deckMatrixProvider.CreateEmptyMatrix();

            CardsToMatrixConverter converter = new CardsToMatrixConverter(deckMatrixProvider);

            Matrix result = converter.Encode(new List<Card>());

            TestHelper.AreMatrixEqual(emptyMatrix, result, deckMatrixProvider);
        }

        [Test]
        public void Encode_WhiteOneCard_ReturnsMatrixWithOne()
        {
            // arrange
            var deckMatrixProvider = new FakeGameProvider
            {
                Colors = new List<Color> {Color.Yellow, Color.White},
                Ranks = new List<Rank> {Rank.One, Rank.Two, Rank.Three}
            };

            List<Card> list = new List<Card>{new Card(Rank.One, Color.White)};

            CardsToMatrixConverter converter = new CardsToMatrixConverter(deckMatrixProvider);
            
            // act
            Matrix result = converter.Encode(list);


            Matrix expected = deckMatrixProvider.CreateEmptyMatrix();
            expected[new Card(Color.White, Rank.One)] = 1;

            TestHelper.AreMatrixEqual(expected, result, deckMatrixProvider);
        }

        [Test]
        public void Encode_TwoRedFourCard_ReturnsMatrixWithZerosAndTwo()
        {
            // arrange
            var deckMatrixProvider = new FakeGameProvider()
            {
                Colors = new List<Color>() {Color.Yellow, Color.Red},
                Ranks = new List<Rank>() { Rank.One, Rank.Two, Rank.Three, Rank.Four},
            };

            List<Card> list = new List<Card>
            {
                new Card(Rank.Four, Color.Red),
                new Card(Rank.Four, Color.Red),
            };

            CardsToMatrixConverter converter = new CardsToMatrixConverter(deckMatrixProvider);
            
            // act
            Matrix result = converter.Encode(list);

            // assert
            Matrix expected = deckMatrixProvider.CreateEmptyMatrix();
            expected[new Card(Rank.Four, Color.Red)] = 2;

            TestHelper.AreMatrixEqual(expected, result, deckMatrixProvider);
        }

        [Test]
        public void Decode_ZeroMatrix_ReturnsEmptyList()
        {
            var deckMatrixProvider = new FakeGameProvider
            {
                Colors = new List<Color> {Color.Blue, Color.Red, Color.Green},
                Ranks = new List<Rank> {Rank.One, Rank.Two, Rank.Three}
            };

            Matrix zeroMatrix = deckMatrixProvider.CreateEmptyMatrix();

            CardsToMatrixConverter converter = new CardsToMatrixConverter(deckMatrixProvider);
            
            IReadOnlyList<Card> actual = converter.Decode(zeroMatrix);

            Assert.IsEmpty(actual);
        }

        [Test]
        public void Decode_MatrixWithOne1AndMany0_ReturnsListWithOneElement()
        {
            var deckMatrixProvider = new FakeGameProvider
            {
                Colors = new List<Color> {Color.Blue, Color.Green, Color.Red},
                Ranks = new List<Rank> {Rank.One, Rank.Two, Rank.Three},
            };

            Matrix input = deckMatrixProvider.CreateEmptyMatrix();
            input[new Card(Color.Green, Rank.Three)] = 1;

            CardsToMatrixConverter converter = new CardsToMatrixConverter(deckMatrixProvider);
            IReadOnlyList<Card> actual = converter.Decode(input);

            Assert.AreEqual(1, actual.Count);

            Card greenThree = new Card(Rank.Three, Color.Green);
            Assert.AreEqual(greenThree, actual[0]);
        }
    }
}
