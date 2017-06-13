using System;
using System.Collections.Generic;

namespace Hanabi
{
    public class Deck
    {
        protected Stack<Card> Cards;

        private static readonly Random Random = new Random();

        public static Deck Create(bool extended = false)
        {
            return extended ? new ExtendedDeck() : new Deck();
        }
        
        protected Deck()
        {
            Cards = new Stack<Card>(GetCards());
        }

        protected List<Card> GetCards()
        {
            List<Card> cards = new List<Card>();
        
            var nominals = new List<Number>
            {
                Number.One,
                Number.One,
                Number.One,
                Number.Two,
                Number.Two,
                Number.Three,
                Number.Three,
                Number.Four,
                Number.Four,
                Number.Five,
            };

            foreach (var nominal in nominals)
            {
                cards.Add(new BlueCard (nominal));
                cards.Add(new GreenCard (nominal));
                cards.Add(new RedCard (nominal));
                cards.Add(new WhiteCard (nominal));
                cards.Add(new YellowCard (nominal));
            }

            return cards;
        }

        public void Shuffle()
        {
            var cards = Cards.ToArray();
            for (int n = Cards.Count - 1; n > 0; --n)
            {
                int k = Random.Next(n + 1);
                Card temp = cards[n];
                cards[n] = cards[k];
                cards[k] = temp;
            }

            Cards = new Stack<Card>(cards);
        }

        public Card PopCard()
        {
            return IsEmpty()? null : Cards.Pop();
        }

        public bool IsEmpty()
        {
            return Cards.Count == 0;
        }
    }

    public class ExtendedDeck : Deck
    {
        internal ExtendedDeck()
        {
            var usualCards = GetCards();
            usualCards.Add(new MulticolorCard(Number.One));
            usualCards.Add(new MulticolorCard(Number.Two));
            usualCards.Add(new MulticolorCard(Number.Three));
            usualCards.Add(new MulticolorCard(Number.Four));
            usualCards.Add(new MulticolorCard(Number.Five));

            Cards = new Stack<Card>(usualCards);
        }
    }
}
