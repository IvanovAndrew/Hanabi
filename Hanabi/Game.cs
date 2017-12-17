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
            get
            {
                Contract.Ensures(Contract.Result<Deck>() != null);
                return Board.Deck;
            }
        }

        private readonly List<Player> _players;

        public IGameProvider GameProvider
        {
            get;
        }

        public Game(IGameProvider provider, int playersCount)
        {
            Contract.Requires<ArgumentOutOfRangeException>(MinPlayerCount <= playersCount, "Too less players!");
            Contract.Requires<ArgumentOutOfRangeException>(playersCount <= MaxPlayerCount, "Too many players!");
            Contract.Requires<ArgumentNullException>(provider != null);

            GameProvider = provider;
            _players = new List<Player>();

            for (int i = 0; i < playersCount; i++)
            {
                Player player = new Player(this, i.ToString());
                player.ClueGiven += OnClueGiven;

                _players.Add(player);
            }

            Board = Board.Create(provider);
        }

        public void AddCardToFirework(Player player, Card card)
        {
            Contract.Requires<ArgumentNullException>(player != null);
            Contract.Requires<ArgumentNullException>(card != null);

            Logger.Log.InfoFormat("{0} is played", card);

            bool added = Board.FireworkPile.AddCard(card);

            if (added)
            {
                Logger.Log.InfoFormat("Card added");

                if (card.Rank == Rank.Five)
                    Board.ClueCounter++;
            }
            else
            {
                Logger.Log.InfoFormat("BLOW!");
            
                Board.DiscardPile.AddCard(card);
                Board.BlowCounter--;
            }

            if (!Deck.IsEmpty())
                AddCardToPlayer(player);
        }

        public void AddCardToDiscardPile(Player player, Card card)
        {
            Contract.Requires<ArgumentNullException>(player != null);
            Contract.Requires<ArgumentNullException>(card != null);
            Contract.Ensures(Contract.OldValue(Board.ClueCounter) + 1 == Board.ClueCounter);

            Logger.Log.InfoFormat("{0} discarded", card);

            Board.DiscardPile.AddCard(card);
            
            Board.ClueCounter += 1;
            Logger.Log.Info("Clues: " + Board.ClueCounter);
            if (!Deck.IsEmpty())
            {
                AddCardToPlayer(player);
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
            Contract.Requires(!Deck.IsEmpty());

            Card newCard = Deck.PopCard();
            CardInHand cardInHand = new CardInHand(player, newCard);

            player.AddCard(cardInHand);
        }

        public int Play()
        {
            Contract.Ensures(Contract.Result<int>() >= 0);

            PrepareForGame();

            var playersEnumerator = NextTurn();
            Player player = null;

            while (!IsGameOver())
            {
                playersEnumerator.MoveNext();
                player = playersEnumerator.Current;

                Logger.Log.InfoFormat("Player {0} turns!", player.Name);
                Logger.Log.InfoFormat("Firework: {0}", Board.FireworkPile);
                Logger.Log.InfoFormat(String.Empty);

                player.Turn();

                Logger.Log.Info("");
            }

            if (Deck.IsEmpty() && Board.BlowCounter > 0)
            {
                Player playerLastTurn = player;

                do
                {
                    playersEnumerator.MoveNext();
                    player = playersEnumerator.Current;

                    Logger.Log.InfoFormat("Player {0} turns!", player.Name);
                    Logger.Log.InfoFormat("Firework: {0}", Board.FireworkPile);
                    Logger.Log.InfoFormat(String.Empty);
                    

                    player.Turn();
                } while (player != playerLastTurn && Board.BlowCounter > 0);
            }

            return Score;
        }

        public int Score
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 0);

                return Board.BlowCounter == 0 ? 0 : Board.FireworkPile.Cards.Count;
            }
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
            foreach (var otherPlayer in _players.Concat(_players))
            {
                if (!found)
                {
                    found = otherPlayer == player;
                }
                else
                {
                    if (otherPlayer == player) break;

                    result.Add(otherPlayer);
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
            return Score == GameProvider.GetMaximumScore() ||
                   Board.BlowCounter == 0 ||
                   Deck.IsEmpty();
        }
    }
}