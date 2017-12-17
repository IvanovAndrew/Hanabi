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
        public Clue PossibleClue { get; set; }

        public PlayerContext(Player player, IEnumerable<CardInHand> hand)
        {
            Contract.Requires<ArgumentNullException>(player != null);
            Contract.Requires<ArgumentNullException>(hand != null);
            Contract.Requires(hand.Any());

            Player = player;
            Hand = hand;
        }

        public IList<Clue> GetCluesAboutCard(CardInHand cardInHand)
        {
            var result = Player.GetCluesAboutCard(cardInHand).ToList();

            if (PossibleClue != null)
            {
                var clue = ClueAndCardMatcher.Match(cardInHand.Card, PossibleClue);
                result.Add(clue);
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

        public bool IsSubtleClue(CardInHand cardInHand, FireworkPile firework)
        {
            if (PossibleClue == null) return false;

            var clue = ClueAndCardMatcher.Match(cardInHand.Card, PossibleClue);

            return clue.IsStraightClue && ClueDetailInfo.IsSubtleClue(firework, PossibleClue);
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
