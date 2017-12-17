using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Hanabi
{
    [ContractClass(typeof(PlayCardStrategyContract))]
    public interface IPlayCardStrategy
    {
        CardProbability EstimateCardToPlayProbability(IBoardContext boardContext);
    }

    [ContractClassFor(typeof(IPlayCardStrategy))]
    abstract class PlayCardStrategyContract : IPlayCardStrategy
    {
        public CardProbability EstimateCardToPlayProbability(IBoardContext boardContext)
        {
            Contract.Requires<ArgumentNullException>(boardContext != null);
            Contract.Requires<ArgumentException>(boardContext.Firework != null);
            Contract.Requires<ArgumentException>(boardContext.ExcludedCards != null);

            var result = Contract.Result<IDictionary<CardInHand, double>>();
            Contract.Ensures(result != null);
            
            // все вероятности \in [0; 1]
            Contract.Ensures(
                Contract.ForAll(result, entry => 0 <= entry.Value && entry.Value <= 1)
                );
            

            throw new NotSupportedException();
        }
    }
}
