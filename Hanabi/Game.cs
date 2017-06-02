using System;
using System.Collections.Generic;

namespace Hanabi
{
    public class Game
    {
        public const int MaxClueCounter = 8;
        public const int MaxBlowCounter = 3;

        public const int MinPlayerCount = 2;
        public const int MaxPlayerCount = 5;

        public Deck Deck;

        public FireworkPile FireworkPile;
        public DiscardPile DiscardPile;

        public List<Player> Players;

        private int _clueCounter;
        public int ClueCounter
        {
            get
            {
                return _clueCounter;
            }
            set
            {
                if (0 <= value && value <= MaxClueCounter)
                    _clueCounter = value;
            }
        }

        private int _blowCounter;
        public int BlowCounter
        {
            get
            {
                return _blowCounter;
            }
            set
            {
                if (0 <= value && value <= MaxBlowCounter)
                    _blowCounter = value;
            }
        }

        public Game(int playersCount, bool isSpecial = false)
        {
            if (playersCount < MinPlayerCount)
                throw new ArgumentOutOfRangeException("playersCount", "Too less players!");

            if (MaxPlayerCount < playersCount)
                throw new ArgumentOutOfRangeException("playersCount", "Too many players!");

            Players = new List<Player>();

            for (int i = 0; i < playersCount; i++)
            {
                Player player = new Player(this);
                player.ClueGiven += OnClueGiven;
                player.Discarded += OnDiscarded;
                Players.Add(player);
            }

            ClueCounter = MaxClueCounter;
            BlowCounter = MaxBlowCounter;

            FireworkPile = new FireworkPile();
            FireworkPile.Blow += OnBlow;
            DiscardPile = new DiscardPile();

            Deck = Deck.Create();
        }

        void OnDiscarded(Object sender, EventArgs args)
        {
            ClueCounter += 1;
            AddCardToPlayer(sender as Player);
        }

        void OnBlow(Object sender, EventArgs args)
        {
            BlowCounter -= 1;
        }

        void OnClueGiven(Object sender, EventArgs args)
        {
            ClueCounter -= 1;
            AddCardToPlayer(sender as Player);
        }

        public void AddCardToPlayer(Player player)
        {
            Card newCard = Deck.PopCard();
            player.AddCard(newCard);
        }

        public int Play()
        {
            PrepareForGame();

            var playersEnumerator = NextTurn();
            Player player = null;

            while (BlowCounter != 0 || Deck.IsEmpty())
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

            return BlowCounter == 0 ? 0 : FireworkPile.GetScore();
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