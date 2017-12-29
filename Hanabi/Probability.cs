using System;
using System.Diagnostics.Contracts;

namespace Hanabi
{
    public class Probability : IComparable
    {
        public static readonly Probability Minimum = new Probability(0.0);
        public static readonly Probability Maximum = new Probability(1.0);

        public double Value { get; }

        public Probability(double value)
        {
            Contract.Requires<ArgumentOutOfRangeException>(0 <= value);
            Contract.Requires<ArgumentOutOfRangeException>(value <= 1);

            Value = value;
        }

        private bool Equals(Probability another)
        {
            return this.Value == another.Value;
        }

        public override bool Equals(object obj)
        {
            if (obj is Probability p) return Equals(p);
            return false;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static bool operator >(Probability one, Probability two)
        {
            return one.Value > two.Value;
        }

        public static bool operator <(Probability one, Probability two)
        {
            return two > one;
        }

        public static bool operator >=(Probability one, Probability two)
        {
            return one > two || one == two;
        }

        public static bool operator <=(Probability one, Probability two)
        {
            return two >= one;
        }

        public static bool operator ==(Probability one, Probability two)
        {
            return Equals(one, two);
        }

        public static bool operator !=(Probability one, Probability two)
        {
            return !(one == two);
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public int CompareTo(object obj)
        {
            if (obj is Probability p)
            {
                if (this < p) return -1;
                if (this == p) return 0;
                if (this > p) return 1;
            }
            throw new ArgumentException();
        }
    }
}
