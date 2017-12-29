using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Hanabi
{
    [ContractClass(typeof(DiscardStrategyContract))]
    public interface IDiscardStrategy
    {
        IDictionary<CardInHand, Probability> EstimateDiscardProbability(IBoardContext boardContext);
        //bool CheckIfRightClue(IBoardContext boardContext, IPlayerContext playerContext);
    }

    [ContractClassFor(typeof(IDiscardStrategy))]
    abstract class DiscardStrategyContract : IDiscardStrategy
    {
        public IDictionary<CardInHand, Probability> EstimateDiscardProbability(IBoardContext boardContext)
        {
            Contract.Requires<ArgumentNullException>(boardContext != null);
            //Contract.Requires<ArgumentNullException>(playerContext != null);

            var result = Contract.Result<IDictionary<CardInHand, Probability>>();
            Contract.Ensures(result != null);
            //Contract.Ensures(
            //    Contract.ForAll(result, entry => playerContext.Hand.Contains(entry.Key))
            //    );
            
            // makes compiler too happy
            throw new NotSupportedException();
        }

        public bool CheckIfRightClue(IBoardContext boardContext, IPlayerContext playerContext)
        {
            Contract.Requires<ArgumentNullException>(boardContext != null);
            Contract.Requires<ArgumentNullException>(playerContext != null);

            Contract.Assume(playerContext.PossibleClue != null);

            // makes compiler too happy
            throw new NotSupportedException();
        }
    }
}
