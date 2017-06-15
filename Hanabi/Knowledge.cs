using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Hanabi
{
    public class ThoughtsAboutCard
    {
        public Card Card { get; set; }
        public Guess Guess { get; set; }
        public List<Clue> Clues { get; set; }
    }

    public class Knowledge
    {
        private readonly List<ThoughtsAboutCard> _thoughts = new List<ThoughtsAboutCard>();
        
        public void Remove(Card card)
        {
            Contract.Requires<ArgumentNullException>(card != null);

            var thoughts = _thoughts.Find(thought => Equals(thought.Card, card));
            _thoughts.Remove(thoughts);
        }

        public Guess this[Card card]
        {
            get
            {
                Contract.Requires<ArgumentNullException>(card != null);
                Contract.Ensures(Contract.Result<Guess>() != null);

                return _thoughts.Find(thought => Equals(thought.Card, card)).Guess;
            }
        }

        public IReadOnlyList<Clue> GetPreviousCluesAboutCard(Card card)
        {
            Contract.Requires<ArgumentNullException>(card != null);
            Contract.Ensures(Contract.Result<IReadOnlyList<Clue>>() != null);

            return _thoughts.Find(thought => Equals(thought.Card, card)).Clues.AsReadOnly();
        }

        public IReadOnlyList<Card> GetHand()
        {
            Contract.Ensures(Contract.Result<IReadOnlyList<Card>>() != null);
            Contract.Ensures(Contract.Result<IReadOnlyList<Card>>().Any());
            
            return _thoughts.Select(thought => thought.Card).ToList().AsReadOnly();
        }

        public IReadOnlyList<Guess> GetGuesses()
        {
            Contract.Ensures(Contract.Result<IReadOnlyList<Guess>>() != null);
            Contract.Ensures(Contract.Result<IReadOnlyList<Guess>>().Any());

            return _thoughts.Select(thoughts => thoughts.Guess).ToList().AsReadOnly();
        }

        public void Add(Card card)
        {
            Contract.Requires<ArgumentNullException>(card != null);
            
            var newThought = new ThoughtsAboutCard
                        {
                            Card = card, 
                            Guess = Guess.Create(), 
                            Clues = new List<Clue>()
                        };
            _thoughts.Add(newThought);
        }

        public Card GetCardByGuess(Guess guess)
        {
            Contract.Requires<ArgumentNullException>(guess != null);
            Contract.Ensures(Contract.Result<Card>() != null);

            return _thoughts.Find(thought => thought.Guess == guess).Card;
        }

        private ThoughtsAboutCard GetThoughtsAboutCard(Card card)
        {
            return _thoughts.Find(thought => Equals(thought.Card, card));
        }

        public void Update(IEnumerable<Card> cards, Clue clue)
        {
            Contract.Requires<ArgumentNullException>(cards != null);
            Contract.Requires(cards.Any());

            Contract.Requires<ArgumentNullException>(clue != null);

            foreach (Card card in cards)
            {
                var thought = GetThoughtsAboutCard(card);
                thought.Clues.Add(clue);

                clue.Accept(thought.Guess);
            }

            Clue revertedClue = clue.Revert();
            var otherCards = _thoughts.Select(thought => thought.Card)
                .Except(cards);

            foreach (var otherCard in otherCards)
            {
                var thought = GetThoughtsAboutCard(otherCard);
                revertedClue.Accept(thought.Guess);
            }
        }
    }
}
