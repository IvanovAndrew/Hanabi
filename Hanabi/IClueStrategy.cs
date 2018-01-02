using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Hanabi
{
    [ContractClass(typeof(ClueStrategyContract))]
    interface IClueStrategy
    {
        IClueSituationStrategy FindClueCandidate(IReadOnlyList<Player> players);
    }

    [ContractClassFor(typeof(IClueStrategy))]
    abstract class ClueStrategyContract : IClueStrategy
    {
        public IClueSituationStrategy FindClueCandidate(IReadOnlyList<Player> players)
        {
            Contract.Requires<ArgumentNullException>(players != null);
            Contract.Requires<ArgumentException>(players.Count > 0);

            Contract.Ensures(Contract.Result<IClueSituationStrategy>() != null);

            throw new NotSupportedException();
        }
    }
}
