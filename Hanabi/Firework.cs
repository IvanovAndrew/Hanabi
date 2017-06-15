using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Hanabi
{
    public abstract class Firework
    {
        private readonly Stack<Card> _cards = new Stack<Card>();

        public Color Color;

        public bool AddCard(Card card)
        {
            Contract.Requires<ArgumentException>(card.Color == Color);

            if (_cards.Count == 0)
            {
                if (card.Nominal == Number.One)
                {
                    _cards.Push(card);
                    return true;
                }
                else
                {
                    return false;
                }
            }

            Card lastCard = _cards.Peek();

            if ((int)card.Nominal - (int)lastCard.Nominal == 1)
            {
                _cards.Push(card);
                return true;
            }

            return false;
        }

        public Card GetLastCard()
        {
            return _cards.Count == 0 ? null : _cards.Peek();
        }

        public Card GetNextCard()
        {
            Card lastCard = GetLastCard();
            if (lastCard == null)
                return CreateCard(Number.One);

            if (lastCard.Nominal == Number.Five)
                return null;

            int nextNumber = (int)lastCard.Nominal + 1;

            return CreateCard((Number) nextNumber);
        }

        protected abstract Card CreateCard(Number number);

    }

    public class BlueFirework : Firework
    {
        public BlueFirework()
        {
            Color = Color.Blue;
        }

        protected override Card CreateCard(Number number)
        {
            return new BlueCard(number);
        }
    }

    public class GreenFirework : Firework
    {
        public GreenFirework()
        {
            Color = Color.Green;
        }

        protected override Card CreateCard(Number number)
        {
            return new GreenCard(number);
        }
    }

    public class RedFirework : Firework
    {
        public RedFirework()
        {
            Color = Color.Red;
        }

        protected override Card CreateCard(Number number)
        {
            return new RedCard(number);
        }
    }

    public class WhiteFirework : Firework
    {
        public WhiteFirework()
        {
            Color = Color.White;
        }

        protected override Card CreateCard(Number number)
        {
            return new WhiteCard(number);
        }
    }

    public class YellowFirework : Firework
    {
        public YellowFirework()
        {
            Color = Color.Yellow;
        }

        protected override Card CreateCard(Number number)
        {
            return new YellowCard(number);
        }
    }
}