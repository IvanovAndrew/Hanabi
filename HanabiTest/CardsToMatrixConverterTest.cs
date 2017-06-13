using System.Collections.Generic;
using Hanabi;
using NUnit.Framework;

namespace HanabiTest
{
    [TestFixture]
    public class CardsToMatrixConverterTest
    {
        [Test]
        public void Encode_ZeroCards_ZeroMatrix()
        {
            int[,] expected = new int[5, 5]
            {
                {0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0},
            };
            
            int[,] result = CardsToMatrixConverter.Encode(new List<Card>());

            TestHelper.AreMatrixEqual(expected, result);
        }

        [Test]
        public void Encode_WhiteOneCard_MatrixWithOne()
        {
            int[,] expected = new int[5, 5]
            {
                {0, 0, 0, 0, 1},
                {0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0},
            };

            List<Card> list = new List<Card>{Card.CreateCard(Number.One, Color.White)};

            int[,] result = CardsToMatrixConverter.Encode(list);

            TestHelper.AreMatrixEqual(expected, result);
        }

        [Test]
        public void Encode_TwoRedFourCard_MatrixWithZerosAndTwo()
        {
            int[,] expected = new int[5, 5]
            {
                {0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0},
                {0, 0, 2, 0, 0},
                {0, 0, 0, 0, 0},
            };

            List<Card> list = new List<Card>
            {
                Card.CreateCard(Number.Four, Color.Red),
                Card.CreateCard(Number.Four, Color.Red),
            };

            int[,] result = CardsToMatrixConverter.Encode(list);

            TestHelper.AreMatrixEqual(expected, result);
        }

        [Test]
        public void Decode_ZeroMatrix_EmptyList()
        {
            int[,] input = new int[5, 5]
            {
                {0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0},
            };

            List<Card> actual = CardsToMatrixConverter.Decode(input);

            Assert.IsEmpty(actual);
        }

        [Test]
        public void Decode_MatrixWithOne1AndMany0_ListWithOneElement()
        {
            int[,] input = new int[5, 5]
            {
                {0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0},
                {0, 1, 0, 0, 0},
                {0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0},
            };

            List<Card> actual = CardsToMatrixConverter.Decode(input);

            Assert.AreEqual(1, actual.Count);

            Card greenThree = Card.CreateCard(Number.Three, Color.Green);
            Assert.AreEqual(greenThree, actual[0]);
        }
    }
}
