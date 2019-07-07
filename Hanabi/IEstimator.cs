using System;
using System.Collections.Generic;

namespace Hanabi
{
    public interface IEstimator
    {
        IList<Card> GetPossibleCards(IBoardContext boardContext, IPlayerContext playerContext);
    }
}
