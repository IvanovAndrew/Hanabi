using System;
using System.Collections.Generic;
using System.Linq;

namespace Hanabi
{
    public class FireworkPile
    {
        public Firework BlueFirework;
        public Firework GreenFirework;
        public Firework RedFirework;
        public Firework YellowFirework;
        public Firework WhiteFirework;

        public event EventHandler Blow;

        public FireworkPile()
        {
            BlueFirework = new BlueFirework();
            GreenFirework = new GreenFirework();
            RedFirework = new RedFirework();
            YellowFirework = new YellowFirework();
            WhiteFirework = new WhiteFirework();
        }

        public bool AddCard(Card card)
        {
            bool added;

            switch(card.Color)
            {
                case Color.Blue:
                    added = BlueFirework.AddCard(card);
                    break;

                case Color.Green:
                    added = GreenFirework.AddCard(card);
                    break;

                case Color.Red:
                    added = RedFirework.AddCard(card);
                    break;

                case Color.White:
                    added = WhiteFirework.AddCard(card);
                    break;

                case Color.Yellow:
                    added = YellowFirework.AddCard(card);
                    break;

                default: 
                    throw new ArgumentException("Unexpected card color!");
                    break;
            }

            if (!added) RaiseEvent();

            return added;
        }

        public int GetScore()
        {
            return new List<Firework> { BlueFirework, GreenFirework, RedFirework, WhiteFirework, YellowFirework }
                    .Select(firework => firework.GetLastCard())
                    .Sum(card => card == null? 0 : (int) card.Nominal + 1);
        }

        private void RaiseEvent()
        {
            if (Blow != null)
                Blow(this, new EventArgs());
        }

        //public bool AddCard(Card card)
        //{
        //    return AddCardToFirework(card);
        //}

        //private bool AddCardToFirework(YellowCard card)
        //{
        //    return YellowFirework.AddCard(card);
        //}

        //private bool AddCardToFirework(BlueCard card)
        //{
        //    return BlueFirework.AddCard(card);
        //}

        //private bool AddCardToFirework(GreenCard card)
        //{
        //    return GreenFirework.AddCard(card);
        //}

        //private bool AddCardToFirework(RedCard card)
        //{
        //    return RedFirework.AddCard(card);
        //}

        //private bool AddCardToFirework(WhiteCard card)
        //{
        //    return WhiteFirework.AddCard(card);
        //}

    }
}
