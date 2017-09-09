using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Hanabi
{
    public class Game
    {
        public const int MinPlayerCount = 2;
        public const int MaxPlayerCount = 5;

        public Board Board;

        public Deck Deck
        {
            get { return Board.Deck; }
        }

        public int Score;

        private readonly List<Player> _players;
        private readonly IGameProvider _provider;

        public Game(IGameProvider provider, int playersCount)
        {
            Contract.Requires<ArgumentOutOfRangeException>(MinPlayerCount <= playersCount, "Too less players!");
            Contract.Requires<ArgumentOutOfRangeException>(playersCount <= MaxPlayerCount, "Too many players!");
            Contract.Requires<ArgumentNullException>(provider != null);

            _provider = provider;
            _players = new List<Player>();

            for (int i = 0; i < playersCount; i++)
            {
                Player player = new Player(this, provider) { Name = i.ToString() };
                player.CardAdded += OnCardAdded;
                player.ClueGiven += OnClueGiven;
                player.Discarded += OnDiscarded;
                player.Blown += OnBlow;

                _players.Add(player);
            }

            Board = Board.Create(provider);
            Score = 0;
        }

        void OnDiscarded(Object sender, EventArgs args)
        {
            Contract.Requires<ArgumentException>(sender is Player);
            Contract.Ensures(Contract.OldValue(Board.ClueCounter) <= Board.ClueCounter);

            Board.ClueCounter += 1;
            Logger.Log.Info("Clues: " + Board.ClueCounter);
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
            
            Logger.Log.Info("Blows remained: " + Board.BlowCounter);

            if (!Deck.IsEmpty())
            {
                AddCardToPlayer(sender as Player);
            }
            
        }

        void OnCardAdded(Object sender, CardAddedEventArgs args)
        {
            Contract.Requires<ArgumentException>(sender is Player);
            Contract.Ensures(Contract.OldValue(Score) < Score);

            Score++;

            if (args.Card.Nominal == Number.Five)
            {
                Board.ClueCounter++;
                Logger.Log.InfoFormat("Clue counter: {0}", Board.ClueCounter);
            }

            if (!Deck.IsEmpty())
            {
                AddCardToPlayer(sender as Player);
            }
        }

        void OnClueGiven(Object sender, EventArgs args)
        {
            Contract.Ensures(Board.ClueCounter < Contract.OldValue(Board.ClueCounter));
            
            Board.ClueCounter -= 1;
            Logger.Log.Info("Clues: " + Board.ClueCounter);
        }

        private void AddCardToPlayer(Player player)
        {
            Contract.Requires<ArgumentNullException>(player != null);

            Card newCard = Deck.PopCard();
            player.AddCardToHand(newCard);
        }

        public int Play()
        {
            PrepareForGame();

            var playersEnumerator = NextTurn();
            Player player = null;

            while (!IsGameOver())
            {
                playersEnumerator.MoveNext();
                player = playersEnumerator.Current;

                Logger.Log.InfoFormat("Player {0} turns!", player.Name);
                
                player.Turn();

                Logger.Log.Info("");
            }

            if (Deck.IsEmpty())
            {
                Player playerLastTurn = player;

                do
                {
                    playersEnumerator.MoveNext();
                    player = playersEnumerator.Current;

                    Logger.Log.InfoFormat("Player {0} turns!", player.Name);
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

            int playersCount = _players.Count;

            int cardsInHand = playersCount > 3 ? 4 : 5;

            for (int i = 0; i < cardsInHand; i++)
            {
                foreach (var player in _players)
                {
                    AddCardToPlayer(player);
                }
            }
        }

        /// <summary>
        /// 
        /// player1, player2, player3, player4
        /// GetPlayersExcept(player1) -> player2, player3, player4
        /// GetPlayersExcept(player2) -> player3, player4, player1
        /// GetPlayersExcept(player3) -> player4, player1, player2
        /// GetPlayersExcept(player4) -> player1, player2, player3
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public IReadOnlyList<Player> GetPlayersExcept(Player player)
        {
            Contract.Requires<ArgumentNullException>(player != null);
            Contract.Ensures(Contract.Result<IReadOnlyList<Player>>().All(p => p != player));

            List<Player> result = new List<Player>();
            bool found = false;
            foreach (var player1 in _players.Concat(_players))
            {
                if (!found)
                {
                    found = player1 == player;
                }
                else
                {
                    if (player1 == player) break;

                    result.Add(player1);
                }

            }

            return result.AsReadOnly();
        }

        private IEnumerator<Player> NextTurn()
        {
            while (true)
            {
                foreach (var player in _players)
                {
                    yield return player;
                }
            }
        }

        private bool IsGameOver()
        {
            return Score == _provider.GetMaximumScore() ||
                   Board.BlowCounter == 0 ||
                   Deck.IsEmpty();
        }
    }
}