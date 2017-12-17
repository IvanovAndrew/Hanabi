using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Hanabi
{
    [ContractClass(typeof(EstimatorContract))]
    public interface IEstimator
    {
        IList<Card> GetPossibleCards(IBoardContext boardContext, IPlayerContext playerContext);
    }

    [ContractClassFor(typeof(IEstimator))]
    abstract class EstimatorContract : IEstimator
    {
        public IList<Card> GetPossibleCards(IBoardContext boardContext, IPlayerContext playerContext)
        {
            Contract.Requires<ArgumentNullException>(boardContext != null);
            Contract.Requires<ArgumentNullException>(playerContext != null);

            Contract.Ensures(Contract.Result<IList<Card>>() != null);

            throw new NotImplementedException();
        }
    }
}
