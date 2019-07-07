using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Hanabi
{
    public class Clue
    {
        public IReadOnlyCollection<CardInHand> Cards { get; }

        public ClueType Type { get; }


        private Clue(ClueType type, IReadOnlyCollection<CardInHand> cards)
        {
            Cards = cards;
            Type = type;
        }


        public static Clue Create(ClueType clueType, IEnumerable<CardInHand> hand)
        {
            if (clueType == null) throw new ArgumentNullException(nameof(clueType));

            var result = new List<CardInHand>();
            foreach (var cardInHand in hand)
            {
                var matcher = new ClueAndCardMatcher(cardInHand.Card);

                if (clueType.Accept(matcher)) result.Add(cardInHand);
            }

            var readOnlyCollection = 
                new ReadOnlyCollectionBuilder<CardInHand>(result).ToReadOnlyCollection();

            return new Clue(clueType, readOnlyCollection);
        }


        public static Clue Revert(Clue clue, IEnumerable<CardInHand> hand)
        {
            return Create(clue.Type.Revert(), hand);
        }


        public bool IsSubtleClue(IEnumerable<Card> expectedCards)
        {
            if (Cards.Count > 1) return false;

            return Type.IsSubtleClue(expectedCards);
        }
    }
}