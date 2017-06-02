namespace Hanabi
{
    public class Guess
    {
        public int[,] Matrix;

        private Guess()
        {
            Matrix = 
                new int[5, 5]
                {
                    {3,3,3,3,3},
                    {2,2,2,2,2},
                    {2,2,2,2,2},
                    {2,2,2,2,2},
                    {1,1,1,1,1},
                };
        }

        public static Guess Create(bool isSpecialGame = false)
        {
            return new Guess();
        }

        public void NumberIs(Number value)
        {
            int row = (int)value;

            for(int n = 0; n < 5; n++)
            {
                if (n == row) continue;

                for(int c = 0; c < 5; c++)
                {
                    Matrix[n, c] = 0;
                }
            }
        }

        public void NumberIsNot(Number value)
        {
            int row = (int)value;

            for (int c = 0; c < 5; c++)
            {
                Matrix[row, c] = 0;
            }
        }

        public void ColorIs(Color color)
        {
            int column = (int)color;

            for (int c = 0; c < 5; c++)
            {
                if (c == column) continue;

                for (int row = 0; row < 5; row++)
                {
                    Matrix[row, c] = 0;
                }
            }
        }

        public void ColorIsNot(Color color)
        {
            int column = (int)color;

            for(int i = 0; i < 5; i++)
            {
                Matrix[i, column] = 0;
            }
        }
    }
}
