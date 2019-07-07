using System;
using System.Collections.Generic;
using System.Linq;

namespace Hanabi
{
    public class PlayerContext : IPlayerContext
    {
        public Player Player { get; }
        public IEnumerable<CardInHand> Hand { get; }
        public ClueType PossibleClue { get; set; }

        public PlayerContext(Player player, IEnumerable<CardInHand> hand)
        {
            Player = player ?? throw new ArgumentNullException(nameof(player));
            Hand = hand ?? throw new ArgumentNullException(nameof(hand));
        }

        public IList<ClueType> GetCluesAboutCard(CardInHand cardInHand)
        {
            var result = Player.GetCluesAboutCard(cardInHand).ToList();

            if (PossibleClue != null)
            {
                var clueAndCardMatcher = new ClueAndCardMatcher(cardInHand.Card);
                if (PossibleClue.Accept(clueAndCardMatcher))
                {
                    result.Add(PossibleClue);
                }
                else
                {
                    result.Add(PossibleClue.Revert());
                }
            }

            return result;
        }

        public bool KnowAboutRankOrColor(CardInHand cardInHand)
        {
            var guess = new Guess(Player.GameProvider, cardInHand);

            foreach (var clue in GetCluesAboutCard(cardInHand))
            {
                clue.Accept(guess);
            }

            return guess.KnowAboutRankOrColor();
        }

        public bool IsSubtleClue(CardInHand cardInHand, IEnumerable<Card> expectedCards)
        {
            if (PossibleClue == null) return false;

            return PossibleClue.IsSubtleClue(expectedCards);
        }

        public IPlayerContext Clone()
        {
            return new PlayerContext(Player, Hand);
        }
    }
}
