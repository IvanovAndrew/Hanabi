using System;
using System.Collections.Generic;
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
            if (card == null) throw new ArgumentNullException(nameof(card));

            // ищем последнюю карту с данным цветом
            Card lastCardInFirework = GetLastCardInFirework(card.Color);

            bool added = false;
            if (Equals(lastCardInFirework, Card.GetPreviousCardInFirework(card)))
            {
                Matrix[card] += 1;
                added = true;
            }

            return added;
        }

        private Card GetLastCardInFirework(Color color)
        {
            foreach (var number in Provider.Ranks.Reverse())
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
            foreach (var number in Provider.Ranks)
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
            return GetLastCards().Aggregate("|", (current, card) => $"{current} {card.ToString()} |");
        }

        public FireworkPile Clone()
        {
            var newFirework = new FireworkPile(this.Provider);

            foreach (var card in Cards)
            {
                if (!newFirework.AddCard(card)) throw new InvalidOperationException();
            }

            return newFirework;
        }
    }
}
