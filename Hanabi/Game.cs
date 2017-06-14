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

        public const int MinPlayerCount = 2;
        public const int MaxPlayerCount = 5;

        public int Score;

        public List<Player> Players;

        public Game(int playersCount, bool isSpecial = false)
        {
            if (playersCount < MinPlayerCount)
                throw new ArgumentOutOfRangeException("playersCount", "Too less players!");

            if (MaxPlayerCount < playersCount)
                throw new ArgumentOutOfRangeException("playersCount", "Too many players!");

            Players = new List<Player>();

            for (int i = 0; i < playersCount; i++)
            {
                Player player = new Player(this) {Name = i.ToString()};
                player.CardAdded += OnCardAdded;
                player.ClueGiven += OnClueGiven;
                player.Discarded += OnDiscarded;
                player.Blown += OnBlow;

                Players.Add(player);
            }

            Board = Board.Create();
            Score = 0;
        }

        void OnDiscarded(Object sender, EventArgs args)
        {
            Contract.Ensures(Contract.OldValue(Board.ClueCounter) <= Board.ClueCounter);

            Board.ClueCounter += 1;

            if (!Deck.IsEmpty())
            {
                AddCardToPlayer(sender as Player);
            }
        }

        void OnBlow(Object sender, EventArgs args)
        {
            Contract.Ensures(Contract.OldValue(Board.BlowCounter) < Board.BlowCounter);

            Board.BlowCounter -= 1;

            if (!Deck.IsEmpty())
            {
                AddCardToPlayer(sender as Player);
            }
        }

        void OnCardAdded(Object sender, EventArgs args)
        {
            Contract.Ensures(Score > Contract.OldValue(Score));

            Score++;
            if (!Deck.IsEmpty())
            {
                AddCardToPlayer(sender as Player);
            }
        }

        void OnClueGiven(Object sender, EventArgs args)
        {
            Contract.Ensures(Contract.OldValue(Board.ClueCounter) > Board.ClueCounter);

            Board.ClueCounter -= 1;
        }

        public void AddCardToPlayer(Player player)
        {
            Contract.Requires(player != null);
            Contract.Requires(!Deck.IsEmpty());

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