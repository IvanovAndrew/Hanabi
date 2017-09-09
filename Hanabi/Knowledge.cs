using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Hanabi
{
    public class ThoughtsAboutCard
    {
        public CardInHand CardInHand { get; set; }
        public Guess Guess { get; set; }
        public List<Clue> Clues { get; set; }
    }

    public class Knowledge
    {
        private readonly List<ThoughtsAboutCard> _thoughts = new List<ThoughtsAboutCard>();
        private readonly IGameProvider _provider;
        private readonly Player _player;

        public Knowledge(IGameProvider provider, Player player)
        {
            _provider = provider;
            _player = player;
        }

        public void Remove(CardInHand card)
        {
            Contract.Requires<ArgumentNullException>(card != null);

            var thoughts = _thoughts.Find(thought => Equals(thought.CardInHand, card));
            _thoughts.Remove(thoughts);
        }

        public IReadOnlyList<Clue> GetPreviousCluesAboutCard(CardInHand card)
        {
            Contract.Requires<ArgumentNullException>(card != null);
            Contract.Ensures(Contract.Result<IReadOnlyList<Clue>>() != null);

            return _thoughts.Find(thought => Equals(thought.CardInHand, card)).Clues.AsReadOnly();
        }

        public IReadOnlyList<CardInHand> GetHand()
        {
            Contract.Ensures(Contract.Result<IReadOnlyList<CardInHand>>() != null);
            Contract.Ensures(Contract.Result<IReadOnlyList<CardInHand>>().Any());
            
            return _thoughts.Select(thought => thought.CardInHand).ToList().AsReadOnly();
        }

        public IReadOnlyList<Guess> GetGuesses()
        {
            Contract.Ensures(Contract.Result<IReadOnlyList<Guess>>() != null);
            Contract.Ensures(Contract.Result<IReadOnlyList<Guess>>().Any());

            return _thoughts.Select(thoughts => thoughts.Guess).ToList().AsReadOnly();
        }

        public CardInHand Add(Card card)
        {
            Contract.Requires<ArgumentNullException>(card != null);
            
            var newThought = new ThoughtsAboutCard
                        {
                            CardInHand = new CardInHand(card, _player), 
                            Guess = new Guess(_provider), 
                            Clues = new List<Clue>()
                        };
            _thoughts.Add(newThought);

            return newThought.CardInHand;
        }

        public CardInHand GetCardByGuess(Guess guess)
        {
            Contract.Requires<ArgumentNullException>(guess != null);
            Contract.Ensures(Contract.Result<CardInHand>() != null);

            return _thoughts.Find(thought => thought.Guess == guess).CardInHand;
        }

        private ThoughtsAboutCard GetThoughtsAboutCard(CardInHand card)
        {
            return _thoughts.Find(thought => Equals(thought.CardInHand, card));
        }

        public void Update(IEnumerable<CardInHand> cards, Clue clue)
        {
            Contract.Requires<ArgumentNullException>(cards != null);
            Contract.Requires(cards.Any());

            Contract.Requires<ArgumentNullException>(clue != null);

            foreach (CardInHand card in cards)
            {
                ApplyClue(card, clue);
            }

            Clue revertedClue = clue.Revert();
            var otherCards = _thoughts.Select(thought => thought.CardInHand)
                .Except(cards);

            foreach (CardInHand otherCard in otherCards)
            {
                ApplyClue(otherCard, revertedClue);
            }
        }

        private void ApplyClue(CardInHand card, Clue clue)
        {
            var thought = GetThoughtsAboutCard(card);
            thought.Clues.Add(clue);

            clue.Accept(thought.Guess);
        }

        public Guess GetGuessAboutCard(CardInHand card)
        {
            Contract.Requires<ArgumentNullException>(card != null);
            Contract.Ensures(Contract.Result<Guess>() != null);

            return _thoughts.Find(thought => Equals(thought.CardInHand, card)).Guess;
        }
    }
}
