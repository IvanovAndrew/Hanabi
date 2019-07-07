using System;
using System.Collections.Generic;

namespace Hanabi
{
    interface IClueStrategy
    {
        IClueSituationStrategy FindClueCandidate(IReadOnlyList<Player> players);
    }
}
