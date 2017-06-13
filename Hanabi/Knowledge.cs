using System.Collections.Generic;
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
            var thoughts = _thoughts.Find(thought => Equals(thought.Card, card));
            _thoughts.Remove(thoughts);
        }

        public Guess this[Card card]
        {
            get { return _thoughts.Find(thought => Equals(thought.Card, card)).Guess; }
        }

        public IReadOnlyList<Clue> GetPreviousCluesAboutCard(Card card)
        {
            return _thoughts.Find(thought => Equals(thought.Card, card)).Clues.AsReadOnly();
        }

        public IReadOnlyList<Card> GetHand()
        {
            return _thoughts.Select(thought => thought.Card).ToList().AsReadOnly();
        }

        public IReadOnlyList<Guess> GetGuesses()
        {
            return _thoughts.Select(thoughts => thoughts.Guess).ToList().AsReadOnly();
        }

        public void Add(Card card)
        {
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
            return _thoughts.Find(thought => thought.Guess == guess).Card;
        }

        private ThoughtsAboutCard GetThoughtsAboutCard(Card card)
        {
            return _thoughts.Find(thought => Equals(thought.Card, card));
        }

        public void Update(IEnumerable<Card> cards, Clue clue)
        {
            foreach (Card card in cards)
            {
                var thought = GetThoughtsAboutCard(card);
                thought.Clues.Add(clue);

                clue.UpdateGuess(thought.Guess);
            }
        }
    }
}
