using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Hanabi
{
    public class FireworkPile : Pile
    {
        public FireworkPile(IGameProvider provider)
            : base(provider)
        {
        }

        public override bool AddCard(Card card)
        {
            // ищем последнюю карту с данным цветом
            Card lastCardInFirework = GetLastCardInFirework(card.Color);

            bool added = false;
            if (Equals(lastCardInFirework, Card.GetCardInFireworkBefore(card)))
            {
                added = true;
                Matrix[card] += 1;
            }

            return added;
        }

        private Card GetLastCardInFirework(Color color)
        {
            foreach (var number in Provider.Numbers.Reverse())
            {
                if (Matrix[number, color] == 1)
                {
                    return new Card(number, color);
                }
            }
            return null;
        }

        private Card GetExpectedCardInFirework(Color color)
        {
            foreach (var number in Provider.Numbers)
            {
                if (Matrix[number, color] == 0)
                {
                    return new Card(number, color);
                }
            }
            return null;
        }

        public IReadOnlyList<Card> GetExpectedCards()
        {
            Contract.Ensures(Contract.Result<IReadOnlyList<Card>>() != null);
            Contract.Ensures(Contract.ForAll(Contract.Result<IReadOnlyList<Card>>(), card => card != null));
            
            List<Card> result = new List<Card>();

            foreach (var color in Provider.Colors)
            {
                Card expectedCard = GetExpectedCardInFirework(color);
                if (expectedCard != null)
                    result.Add(expectedCard);
            }

            return result;
        }

        public IReadOnlyList<Card> GetLastCards()
        {
            Contract.Ensures(Contract.Result<IReadOnlyList<Card>>() != null);
            
            List<Card> result = new List<Card>();

            foreach (var color in Provider.Colors)
            {
                Card lastCardInFirework = GetLastCardInFirework(color);
                if (lastCardInFirework != null)
                    result.Add(lastCardInFirework);
            }

            return result.AsReadOnly();
        }

        public override string ToString()
        {
            Func<string, Card, string> func = 
                (current, card) => String.Format("{0} {1} |", current, card.ToString());

            return GetLastCards().Aggregate("| ", func);
        }
    }
}
