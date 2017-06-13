using NUnit.Framework;

namespace HanabiTest
{
    public static class TestHelper
    {
        public static void AreMatrixEqual(int[,] expected, int[,] actual)
        {
            Assert.AreEqual(expected.GetLength(0), actual.GetLength(0));
            Assert.AreEqual(expected.GetLength(1), actual.GetLength(1));

            for (int i = 0; i < expected.GetLength(0); i++)
            {
                for (int j = 0; j < expected.GetLength(1); j++)
                {
                    Assert.AreEqual(expected[i, j], actual[i, j]);
                }
            }

            Assert.Pass();
        }
    }
}
