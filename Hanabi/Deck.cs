using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Hanabi
{
    public class Deck
    {
        private Stack<Card> _cards;

        private static readonly Random Random = new Random();

        public Deck(IEnumerable<Card> cards)
        {
            Contract.Requires<ArgumentNullException>(cards != null);

            _cards = new Stack<Card>(cards);
        }

        public void Shuffle()
        {
            var cards = _cards.ToArray();
            for (int n = _cards.Count - 1; n > 0; --n)
            {
                int k = Random.Next(n + 1);
                Card temp = cards[n];
                cards[n] = cards[k];
                cards[k] = temp;
            }

            _cards = new Stack<Card>(cards);
        }

        public Card PopCard()
        {
            Contract.Ensures(Contract.Result<Card>() != null);
            
            if (IsEmpty()) throw new InvalidOperationException("Deck is empty");

            return _cards.Pop();
        }

        public bool IsEmpty()
        {
            return _cards.Count == 0;
        }
    }
}
