using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Hanabi
{
    [ContractClass(typeof(ClueStrategyContract))]
    interface IClueStrategy
    {
        HardSolution FindClueCandidate(IReadOnlyList<Player> players);
    }

    [ContractClassFor(typeof(IClueStrategy))]
    abstract class ClueStrategyContract : IClueStrategy
    {
        public HardSolution FindClueCandidate(IReadOnlyList<Player> players)
        {
            Contract.Requires<ArgumentNullException>(players != null);
            Contract.Requires<ArgumentException>(players.Count > 0);

            var contractResult = Contract.Result<HardSolution>();

            Contract.Ensures(contractResult != null);

            Contract.Ensures(
                contractResult.Situation != ClueSituation.ClueExists ||
                contractResult.Situation == ClueSituation.ClueExists &&
                contractResult.PlayerToClue != null &&
                contractResult.Clue != null);

            throw new NotSupportedException();
        }
    }
}
