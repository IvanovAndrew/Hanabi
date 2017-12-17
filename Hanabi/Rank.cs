namespace Hanabi
{
    public static class RankExtension
    {
        public static Rank? GetNextNumber(this Rank rank)
        {
            if (rank == Rank.Five) return null;

            return (Rank)((int)rank + 1);
        }

        public static Rank? GetPreviousNumber(this Rank rank)
        {
            if (rank == Rank.One) return null;

            return (Rank)((int)rank - 1);
        }
    }
}
