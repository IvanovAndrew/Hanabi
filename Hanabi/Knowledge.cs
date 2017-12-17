﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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
            Contract.Requires<ArgumentNullException>(card != null);

            var thoughts = _thoughts.Find(thought => Equals(thought.CardInHand, card));
            _thoughts.Remove(thoughts);
        }

        public IReadOnlyList<Clue> GetCluesAboutCard(CardInHand card)
        {
            Contract.Requires<ArgumentNullException>(card != null);
            Contract.Ensures(Contract.Result<IReadOnlyList<Clue>>() != null);

            return _thoughts
                        .Find(thought => Equals(thought.CardInHand, card)).Clues
                        .Distinct()
                        .ToList().AsReadOnly();
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

        public void Add(CardInHand cardInHand)
        {
            Contract.Requires<ArgumentNullException>(cardInHand != null);
            Contract.Ensures(Contract.OldValue(_thoughts.Count) + 1 == _thoughts.Count);

            var newThought = new ThoughtsAboutCard
                        {
                            CardInHand = cardInHand,
                            Guess = new Guess(_provider, cardInHand), 
                            Clues = new List<Clue>()
                        };
            _thoughts.Add(newThought);
        }

        public CardInHand GetCardByGuess(Guess guess)
        {
            Contract.Requires<ArgumentNullException>(guess != null);
            Contract.Ensures(Contract.Result<CardInHand>() != null);

            return _thoughts.Find(thought => thought.Guess == guess).CardInHand;
        }

        private ThoughtsAboutCard GetThoughtsAboutCard(CardInHand cardInHand)
        {
            return _thoughts.Find(thought => Equals(thought.CardInHand, cardInHand));
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
            IEnumerable<CardInHand> otherCards = _thoughts
                                        .Select(thought => thought.CardInHand)
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
