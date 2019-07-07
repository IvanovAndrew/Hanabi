using System;
using System.Collections.Generic;

namespace Hanabi
{
    public class Deck
    {
        private Stack<Card> _cards;

        private static readonly Random Random = new Random();

        public Deck(IEnumerable<Card> cards)
        {
            if (cards == null) throw new ArgumentNullException(nameof(cards));

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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">throw if deck is empty</exception>
        public Card PopCard()
        {
            if (IsEmpty()) throw new InvalidOperationException("Deck is empty");
            
            return _cards.Pop();
        }

        public bool IsEmpty()
        {
            return _cards.Count == 0;
        }

        public int Cards() => _cards.Count;
    }
}
