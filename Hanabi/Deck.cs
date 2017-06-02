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
                cards.Add(new BlueCard {Nominal = nominal});
                cards.Add(new GreenCard {Nominal = nominal});
                cards.Add(new RedCard {Nominal = nominal});
                cards.Add(new WhiteCard {Nominal = nominal});
                cards.Add(new YellowCard {Nominal = nominal});
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
            usualCards.Add(new MulticolorCard {Nominal = Number.One});
            usualCards.Add(new MulticolorCard {Nominal = Number.Two});
            usualCards.Add(new MulticolorCard { Nominal = Number.Three });
            usualCards.Add(new MulticolorCard { Nominal = Number.Four });
            usualCards.Add(new MulticolorCard { Nominal = Number.Five });

            Cards = new Stack<Card>(usualCards);
        }
    }
}
