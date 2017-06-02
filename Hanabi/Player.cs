using System;
using System.Collections.Generic;
using System.Linq;

namespace Hanabi
{
    public class Player
    {
        public Dictionary<Card, Guess> Memory;

        public event EventHandler ClueGiven;

        public event EventHandler Discarded;

        public List<Card> Hand
        {
            get
            {
                return Memory.Keys.ToList();
            }
        }

        public Game Game;

        public Player(Game game)
        {
            Memory = new Dictionary<Card, Guess>();
            Game = game;
        }

        public void ReceiveFirstCards(List<Card> cards)
        {
            foreach (var card in cards)
            {
                Memory.Add(card, Guess.Create());
            }
        }

        public void ListenClue(List<Card> cards, Clue clue)
        {
            foreach(var card in cards)
            {
                Guess guess = Memory[card];
                clue.UpdateGuess(guess);
            }

            var otherCards = Hand.Except(cards);

            foreach(var card in otherCards)
            {
                Guess guess = Memory[card];
                Clue revertedClue = clue.Revert();
                clue.UpdateGuess(guess);
            }
        }

        public void Turn()
        {
            DiscardCard(Hand[0]);
            /// Can I play?
            ///     See Firework pile.
            ///     See my own guessed.
            ///     Can I play card?
            ///         Yes: Play (if next player has a clue)
            ///         Otherwise: No
            ///  Can I give a clue?
            ///  If Yes, see card next player and his memory. 
            ///  Can he play after my clue?

        }

        /// <summary>
        /// The player takes a card from their hand and places it face up in front of them.
        /// There are then 2 possibilies:
        /// (*) if the card can start, or can be added to a firework,
        ///     it is placed face-up on that firework's pile
        /// (*) if the card cannot be added to a firework, 
        ///     it is discarded, and the red counter is placed 
        ///     in the tin lid.
        ///
        /// In either case, the player then draws a new card, without looking at it, 
        /// and adds it to their hand.
        /// </summary>
        /// <param name="card"></param>
        private void PlayCard(Card card)
        {
            Memory.Remove(card);
        }

        /// <summary>
        /// Performing this action allows you to return a blue counter to the tin lid.
        /// The player discards a card from their hand, face-up, on to a discard pile (next to the tin).
        /// They then draw a new card, without looking at it, and add it to their hand.
        /// </summary>
        /// <param name="card"></param>
        private void DiscardCard(Card card)
        {
            Memory.Remove(card);

            DiscardPile discardPile = new DiscardPile();

            discardPile.AddCard(card);
            RaiseDiscardedEvent();
        }

        /// <summary>
        /// To perform this action, the player must remove a blue counter from the tin lid
        /// (and place it next to it, with the red counters). They can then give a clue to one of the other players 
        /// about the cards they hold.
        /// The player must indicate clearly - by pointing - which cards he is offering a clue about.
        /// A player must give a full clue: e.g. if a player has two yellow cards, the clue-giver must not indicate only one!
        /// </summary>
        /// <param name="cards"></param>
        /// <param name="clue"></param>
        private void GiveClue()
        {
            Player otherPlayer = null;

            List<Card> cards = null;
            Clue clue = null;

            otherPlayer.ListenClue(cards, clue);
            RaiseClueGivenEvent();
        }

        private void RaiseClueGivenEvent()
        {
            if (ClueGiven != null)
                ClueGiven(this, new EventArgs());
        }

        private void RaiseDiscardedEvent()
        {
            if (Discarded != null)
                Discarded(this, new EventArgs());
        }

        public void AddCard(Card card)
        {
            Memory.Add(card, Guess.Create());
        }
    }
}
