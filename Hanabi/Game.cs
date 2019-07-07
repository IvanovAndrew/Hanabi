using Hanabi.Exceptions;
using System;
using System.Collections.Generic;
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
                return Board.Deck;
            }
        }

        public Player PlayerToTurnLast { get; private set; }

        private readonly List<Player> _players;

        public IGameProvider GameProvider
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="playersCount"></param>
        /// <exception cref="TooLessPlayersException"></exception>
        /// <exception cref="TooManyPlayersException"></exception>
        public Game(IGameProvider provider, int playersCount)
        {
            if (playersCount < MinPlayerCount) throw new TooLessPlayersException();
            if (MaxPlayerCount < playersCount) throw new TooManyPlayersException();

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        /// <param name="card"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void AddCardToFirework(Player player, Card card)
        {
            if (player == null) throw new ArgumentNullException(nameof(player));
            if (card == null) throw new ArgumentNullException(nameof(card));

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        /// <param name="card"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void AddCardToDiscardPile(Player player, Card card)
        {
            if (player == null) throw new ArgumentNullException(nameof(player));
            if (card == null) throw new ArgumentNullException(nameof(card));

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
            Board.ClueCounter -= 1;
            Logger.Log.Info("Clues: " + Board.ClueCounter);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">Throws if deck if empty</exception>
        private void AddCardToPlayer(Player player)
        {
            if (player == null) throw new ArgumentNullException(nameof(player));

            Card newCard = Deck.PopCard();
            CardInHand cardInHand = new CardInHand(player, newCard);

            player.AddCard(cardInHand);
        }

        public int Play()
        {
            PrepareForGame();

            var playersEnumerator = NextTurn();

            Player player;
            
            bool lastTurn = false;

            do
            {
                playersEnumerator.MoveNext();
                player = playersEnumerator.Current;

                Logger.Log.Info($"Deck contains {Board.Deck.Cards()} card(s)");
                Logger.Log.InfoFormat($"Player {player.Name} turns!");
                Logger.Log.InfoFormat($"Firework: {Board.FireworkPile}");
                Logger.Log.InfoFormat(String.Empty);

                player.Turn();

                Logger.Log.Info("");

                if (Deck.IsEmpty())
                {
                    if (PlayerToTurnLast == null)
                        PlayerToTurnLast = player;
                    else
                        lastTurn = true;
                }
            } while (!IsGameOver(lastTurn, player, PlayerToTurnLast));

            Logger.Log.InfoFormat($"Firework: {Board.FireworkPile}");
            return Score;
        }

        public int Score
        {
            get
            {
                return Board.BlowCounter == 0 ? 0 : Board.FireworkPile.Cards.Count;
            }
        }

        private void PrepareForGame()
        {
            Deck.Shuffle();

            int cardsInHand = GetCardInHandsCount(_players.Count);

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
        /// <exception cref="ArgumentNullException"></exception>
        public IReadOnlyList<Player> GetPlayersExcept(Player player)
        {
            if (player == null) throw new ArgumentNullException(nameof(player));

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

        private int GetCardInHandsCount(int playersCount)
        {
            // TODO clean up this
            return playersCount > 3 ? 4 : 5;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lastTurn"></param>
        /// <param name="activePlayer"></param>
        /// <param name="lastPlayerToTurn"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private bool IsGameOver(bool lastTurn, Player activePlayer, Player lastPlayerToTurn)
        {
            if (activePlayer == null) throw new ArgumentNullException(nameof(activePlayer));

            if (Score == GameProvider.GetMaximumScore()) return true;
            if (Board.BlowCounter == 0) return true;


            return lastTurn && activePlayer == lastPlayerToTurn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public IReadOnlyList<Player> GetPlayersToTurn(Player start)
        {
            if (start == null) throw new ArgumentNullException(nameof(start));

            var players = GetPlayersExcept(start);
            if (PlayerToTurnLast == null) return players;

            if (start == PlayerToTurnLast) return new List<Player>();

            return players
                        .TakeWhile(p => p != PlayerToTurnLast)
                        .Concat(new List<Player>{PlayerToTurnLast})
                        .ToList()
                        .AsReadOnly();
        }
    }
}