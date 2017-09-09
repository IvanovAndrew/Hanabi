namespace Hanabi
{
    public enum Nominal
    {
        One = 0,
        Two = 1,
        Three = 2,
        Four = 3,
        Five = 4
    }

    public static class NumberExtension
    {
        public static Nominal? GetNextNumber(this Nominal nominal)
        {
            if (nominal == Nominal.Five) return null;

            return (Nominal)((int)nominal + 1);
        }

        public static Nominal? GetPreviousNumber(this Nominal nominal)
        {
            if (nominal == Nominal.One) return null;

            return (Nominal)((int)nominal - 1);
        }
    }
}
