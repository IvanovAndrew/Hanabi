using System;
using System.Collections.Generic;
using System.Linq;

namespace Hanabi
{
    public class Knowledge
    {
        private readonly List<ThoughtsAboutCard> _thoughts = new List<ThoughtsAboutCard>();
        private readonly IGameProvider _provider;

        public Knowledge(IGameProvider provider)
        {
            _provider = provider;
        }

        public void Remove(CardInHand card)
        {
            if(card == null) throw new ArgumentNullException(nameof(card));

            var thoughts = _thoughts.Find(thought => Equals(thought.CardInHand, card));
            _thoughts.Remove(thoughts);
        }

        public IReadOnlyList<ClueType> GetCluesAboutCard(CardInHand card)
        {
            if (card == null) throw new ArgumentNullException(nameof(card));

            return _thoughts
                        .Find(thought => Equals(thought.CardInHand, card)).Clues
                        .Distinct()
                        .ToList().AsReadOnly();
        }

        public IReadOnlyList<CardInHand> GetHand()
        {
            return _thoughts.Select(thought => thought.CardInHand).ToList().AsReadOnly();
        }

        public IReadOnlyList<Guess> GetGuesses()
        {
            return _thoughts.Select(thoughts => thoughts.Guess).ToList().AsReadOnly();
        }

        public void Add(CardInHand cardInHand)
        {
            if (cardInHand == null) throw new ArgumentNullException(nameof(cardInHand));

            var newThought = new ThoughtsAboutCard
                        {
                            CardInHand = cardInHand,
                            Guess = new Guess(_provider, cardInHand), 
                            Clues = new List<ClueType>()
                        };
            _thoughts.Add(newThought);
        }

        public CardInHand GetCardByGuess(Guess guess)
        {
            if (guess == null) throw new ArgumentNullException(nameof(guess));

            return _thoughts.Find(thought => thought.Guess == guess).CardInHand;
        }

        private ThoughtsAboutCard GetThoughtsAboutCard(CardInHand cardInHand)
        {
            return _thoughts.Find(thought => Equals(thought.CardInHand, cardInHand));
        }

        public void Update(Clue clue)
        {
            if(clue == null) throw new ArgumentNullException(nameof(clue));

            foreach (CardInHand card in clue.Cards)
            {
                ApplyClue(card, clue.Type);
            }

            IEnumerable<CardInHand> otherCards = _thoughts
                .Select(thought => thought.CardInHand);

            Clue revertedClue = Clue.Revert(clue, otherCards);

            foreach (CardInHand otherCard in revertedClue.Cards)
            {
                ApplyClue(otherCard, revertedClue.Type);
            }
        }

        private void ApplyClue(CardInHand card, ClueType clue)
        {
            var thought = GetThoughtsAboutCard(card);
            thought.Clues.Add(clue);

            clue.Accept(thought.Guess);
        }

        public Guess GetGuessAboutCard(CardInHand card)
        {
            if (card == null) throw new ArgumentNullException(nameof(card));

            return _thoughts.Find(thought => Equals(thought.CardInHand, card)).Guess;
        }
    }
}
