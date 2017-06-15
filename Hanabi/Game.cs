using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Hanabi
{
    public class Game
    {
        public Board Board;

        public Deck Deck
        {
            get { return Board.Deck; }
        }

        public int Score;

        public List<Player> Players;

        public Game(GameMode mode)
        {
            Contract.Requires<ArgumentNullException>(mode != null);

            Players = new List<Player>();

            for (int i = 0; i < mode.PlayersCount; i++)
            {
                Player player = new Player(this) {Name = i.ToString()};
                player.CardAdded += OnCardAdded;
                player.ClueGiven += OnClueGiven;
                player.Discarded += OnDiscarded;
                player.Blown += OnBlow;

                Players.Add(player);
            }

            Board = Board.Create(mode.IsSpecialMode);
            Score = 0;
        }

        void OnDiscarded(Object sender, EventArgs args)
        {
            Contract.Requires<ArgumentException>(sender is Player);
            Contract.Ensures(Contract.OldValue(Board.ClueCounter) <= Board.ClueCounter);

            Board.ClueCounter += 1;

            if (!Deck.IsEmpty())
            {
                AddCardToPlayer((Player) sender);
            }
        }

        void OnBlow(Object sender, EventArgs args)
        {
            Contract.Requires<ArgumentException>(sender is Player);
            Contract.Ensures(Board.BlowCounter < Contract.OldValue(Board.BlowCounter));

            Board.BlowCounter -= 1;

            if (!Deck.IsEmpty())
            {
                AddCardToPlayer(sender as Player);
            }
            
        }

        void OnCardAdded(Object sender, EventArgs args)
        {
            Contract.Requires<ArgumentException>(sender is Player);
            Contract.Ensures(Contract.OldValue(Score) < Score);

            Score++;
            if (!Deck.IsEmpty())
            {
                AddCardToPlayer(sender as Player);
            }

            
        }

        void OnClueGiven(Object sender, EventArgs args)
        {
            Contract.Ensures(Board.ClueCounter < Contract.OldValue(Board.ClueCounter));
            
            Board.ClueCounter -= 1;
        }

        public void AddCardToPlayer(Player player)
        {
            Contract.Requires<ArgumentNullException>(player != null);

            Card newCard = Deck.PopCard();
            player.AddCard(newCard);
        }

        public int Play()
        {
            PrepareForGame();

            var playersEnumerator = NextTurn();
            Player player = null;

            while (Board.BlowCounter != 0 && !Deck.IsEmpty())
            {
                playersEnumerator.MoveNext();
                player = playersEnumerator.Current;

                player.Turn();
            }

            if (Deck.IsEmpty())
            {
                Player playerLastTurn = player;

                do
                {
                    playersEnumerator.MoveNext();
                    player = playersEnumerator.Current;

                    player.Turn();
                } while (player != playerLastTurn);
            }

            return GetScore();
        }

        private int GetScore()
        {
            return Board.BlowCounter == 0 ? 0 : Score;
        }

        private void PrepareForGame()
        {
            Deck.Shuffle();

            int playersCount = Players.Count;

            int cardsInHand = playersCount > 3 ? 4 : 5;

            for (int i = 0; i < cardsInHand; i++)
            {
                foreach (var player in Players)
                {
                    AddCardToPlayer(player);
                }
            }
        }

        public IReadOnlyList<Player> GetPlayers(Player player)
        {
            Contract.Ensures(Contract.Result<IReadOnlyList<Player>>().All(p => p != player));

            return Players.Except(new[] {player}).ToList();
        }

        public IEnumerator<Player> NextTurn()
        {
            while (true)
            {
                foreach (var player in Players)
                {
                    yield return player;
                }
            }
        }
    }
}