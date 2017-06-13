using System;
using System.Collections.Generic;

namespace Hanabi
{
    public class FireworkPile : Pile
    {
        private readonly Firework _blueFirework;
        private readonly Firework _greenFirework;
        private readonly Firework _redFirework;
        private readonly Firework _yellowFirework;
        private readonly Firework _whiteFirework;

        public FireworkPile()
        {
            _blueFirework = new BlueFirework();
            _greenFirework = new GreenFirework();
            _redFirework = new RedFirework();
            _yellowFirework = new YellowFirework();
            _whiteFirework = new WhiteFirework();
        }

        public override bool AddCard(Card card)
        {
            bool added;

            switch(card.Color)
            {
                case Color.Blue:
                    added = _blueFirework.AddCard(card);
                    break;

                case Color.Green:
                    added = _greenFirework.AddCard(card);
                    break;

                case Color.Red:
                    added = _redFirework.AddCard(card);
                    break;

                case Color.White:
                    added = _whiteFirework.AddCard(card);
                    break;

                case Color.Yellow:
                    added = _yellowFirework.AddCard(card);
                    break;

                default: 
                    throw new ArgumentException("Unexpected card color!");
                    break;
            }

            if (added) _cards.Add(card);

            return added;
        }

        public IReadOnlyList<Card> GetExpectedCards()
        {
            return new List<Card>
                {
                    _blueFirework.GetNextCard(),
                    _greenFirework.GetNextCard(),
                    _redFirework.GetNextCard(),
                    _whiteFirework.GetNextCard(),
                    _yellowFirework.GetNextCard()
                }.FindAll(card => card != null);
        }

        public IReadOnlyList<Card> GetLastCards()
        {
            return new List<Card>
            {
                _blueFirework.GetLastCard(),
                _greenFirework.GetLastCard(),
                _redFirework.GetLastCard(),
                _whiteFirework.GetLastCard(),
                _yellowFirework.GetLastCard()
            }.FindAll(card => card != null);
        }
    }
}
