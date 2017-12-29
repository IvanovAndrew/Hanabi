using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Hanabi
{
    [ContractClass(typeof(GameProviderContract))]
    public interface IGameProvider
    {
        Matrix CreateEmptyMatrix();
        Matrix CreateFullDeckMatrix();

        IReadOnlyList<Color> Colors { get; }
        IReadOnlyList<Rank> Ranks { get; }

        int ColorToInt(Color color);
        [Pure]
        int GetMaximumScore();
    }

    [ContractClassFor(typeof(IGameProvider))]
    abstract class GameProviderContract : IGameProvider
    {
        public Matrix CreateEmptyMatrix()
        {
            Contract.Ensures(Contract.Result<Matrix>() != null);
            throw new NotSupportedException();
        }

        public Matrix CreateFullDeckMatrix()
        {
            Contract.Ensures(Contract.Result<Matrix>() != null);
            throw new NotSupportedException();
        }

        public IReadOnlyList<Color> Colors { get; }
        public IReadOnlyList<Rank> Ranks { get; }
        public int ColorToInt(Color color)
        {
            Contract.Ensures(Contract.Result<int>() >= 0);
            Contract.Ensures(Contract.Result<int>() < Colors.Count);
            throw new System.NotImplementedException();
        }

        [Pure]
        public int GetMaximumScore()
        {
            Contract.Ensures(Contract.Result<int>() >= 0);
            throw new System.NotImplementedException();
        }
    }
}
