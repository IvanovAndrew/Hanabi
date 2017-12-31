using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Hanabi
{
    [ContractClass(typeof(PlayCardStrategyContract))]
    public interface IPlayCardStrategy
    {
        IDictionary<CardInHand, Probability> EstimateCardToPlayProbability(IBoardContext boardContext);
    }

    [ContractClassFor(typeof(IPlayCardStrategy))]
    abstract class PlayCardStrategyContract : IPlayCardStrategy
    {
        public IDictionary<CardInHand, Probability> EstimateCardToPlayProbability(IBoardContext boardContext)
        {
            Contract.Requires<ArgumentNullException>(boardContext != null);

            var result = Contract.Result<IDictionary<CardInHand, Probability>>();
            Contract.Ensures(result != null);
            
            throw new NotSupportedException();
        }
    }
}
