using System.Collections.Generic;

namespace Hanabi
{
    public interface IDiscardStrategy
    {
        IDictionary<CardInHand, Probability> EstimateDiscardProbability(IBoardContext boardContext);
    }
}
