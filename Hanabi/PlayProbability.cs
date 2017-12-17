using System.Collections.Concurrent;

namespace Hanabi
{
    public class CardProbability : ConcurrentDictionary<CardInHand, double>
    {
    }
}
