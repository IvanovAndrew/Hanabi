using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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
            Contract.Requires<ArgumentNullException>(player != null);
            Contract.Requires<ArgumentNullException>(hand != null);
            Contract.Requires(hand.Any());

            Player = player;
            Hand = hand;
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

        public PlayerContext Clone()
        {
            return new PlayerContext(Player, Hand);
        }

        [ContractInvariantMethod]
        private void Invariant()
        {
            Contract.Invariant(Player != null);
            Contract.Invariant(Hand != null);
            Contract.Invariant(Hand.Any());
        }
    }
}
