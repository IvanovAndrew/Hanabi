using System;
using System.Collections.Generic;

namespace Hanabi
{
    public interface IPlayCardStrategy
    {
        IDictionary<CardInHand, Probability> EstimateCardToPlayProbability(IBoardContext boardContext);
    }
}
