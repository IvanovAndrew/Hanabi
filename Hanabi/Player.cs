using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Hanabi
{
    public class Player
    {
        private readonly Knowledge _memory;

        public event EventHandler ClueGiven;
        public event EventHandler Discarded;
        public event EventHandler CardAdded;
        public event EventHandler Blown;

        public IReadOnlyList<Guess> Guesses
        {
            get { return _memory.GetGuesses(); }
        }

        public string Name { get; set; }

        public Game Game;

        public const double ProbabilityThreshold = 0.8;

        private DiscardPile DiscardPile
        {
            get
            {
                Contract.Ensures(Contract.Result<DiscardPile>() != null);
                return Game.Board.DiscardPile;
            }
        }

        private FireworkPile FireworkPile
        {
            get
            {
                Contract.Ensures(Contract.Result<FireworkPile>() != null);

                return Game.Board.FireworkPile;
            }
        }

        private int BlowCounter
        {
            get { return Game.Board.BlowCounter; }
        }

        private int ClueCounter
        {
            get { return Game.Board.ClueCounter; }
        }

        private IReadOnlyList<Player> Players
        {
            get
            {
                Contract.Ensures(Contract.Result<IReadOnlyList<Player>>() != null);
                return Game.GetPlayers(this);
            }
        }

        public Player(Game game)
        {
            Contract.Requires<ArgumentNullException>(game != null);

            _memory = new Knowledge();
            Game = game;
        }

        public void ReceiveFirstCards(IEnumerable<Card> cards)
        {
            Contract.Requires<ArgumentNullException>(cards != null);
            Contract.Requires(cards.Any());

            foreach (var card in cards)
            {
                _memory.Add(card);
            }
        }

        public IReadOnlyList<Card> ShowCards(Player to)
        {
            Contract.Requires<ArgumentException>(to != this);

            return _memory.GetHand();
        }

        public void ListenClue(IEnumerable<Card> cards, Clue clue)
        {
            Contract.Requires<ArgumentNullException>(cards != null);
            Contract.Requires(cards.Any());

            Contract.Requires<ArgumentNullException>(clue != null);

            _memory.Update(cards, clue);
        }

        /// Can I play?
        ///     See Firework pile.
        ///     See my own guessed.
        ///     Can I play card?
        ///         Yes: Play (if next player has a clue)
        ///         Otherwise: No
        ///  Can I give a clue?
        ///  If Yes, see card next player and his memory. 
        ///  Can he play after my clue?
        public void Turn()
        {
            Card cardToPlay = GetCardToPlay();
            if (cardToPlay != null)
            {
                PlayCard(cardToPlay);
                return;
            }

            bool clueGiven = ClueCounter > 0 && GiveClue();
            if (!clueGiven)
            {
                Card card = GetCardToDiscard();
                DiscardCard(card);
            }
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
            _memory.Remove(card);
            
            bool added = FireworkPile.AddCard(card);

            if (added)
            {
                RaiseCardAddedEvent();
            }
            else
            {
                DiscardPile.AddCard(card);
                RaiseBlownEvent();
            }
        }

        /// <summary>
        /// Performing this action allows you to return a blue counter to the tin lid.
        /// The player discards a card from their hand, face-up, on to a discard pile (next to the tin).
        /// They then draw a new card, without looking at it, and add it to their hand.
        /// </summary>
        /// <param name="card"></param>
        private void DiscardCard(Card card)
        {
            _memory.Remove(card);

            DiscardPile.AddCard(card);
            RaiseDiscardedEvent();
        }

        /// <summary>
        /// To perform this action, the player must remove a blue counter from the tin lid
        /// (and place it next to it, with the red counters). They can then give a clue to one of the other players 
        /// about the cards they hold.
        /// The player must indicate clearly - by pointing - which cards he is offering a clue about.
        /// A player must give a full clue: e.g. if a player has two yellow cards, the clue-giver must not indicate only one!
        /// </summary>
        private bool GiveClue()
        {
            var candidate = FindClueCandidate();

            // ну совсем нечего игроку подсказывать...
            // тогда не будем ему ничего подсказывать
            if (candidate == null) return false;

            Player playerToClue = candidate.Player;
            Card cardToClue = candidate.Card;
            
            // чтобы подсказки не повторялись, посмотрим, 
            // давали ли аналогичную подсказку раньше
            IReadOnlyList<Clue> previousClues =
                playerToClue._memory.GetPreviousCluesAboutCard(cardToClue);

            IEnumerable<Card> cardsToClue;
            Clue clue = new IsValue(cardToClue.Nominal);
            if (previousClues.Contains(clue))
            {
                clue = new IsColor(cardToClue.Color);
                cardsToClue = playerToClue.ShowCards(this)
                                        .Where(card => card.Color == cardToClue.Color);
            }
            else
            {
                cardsToClue = playerToClue.ShowCards(this)
                                        .Where(card => card.Nominal == cardToClue.Nominal);
            }

            playerToClue.ListenClue(cardsToClue, clue);
            RaiseClueGivenEvent();

            return true;
        }

        private ClueCandidate FindClueCandidate()
        {
            // понять, какие карты нужны
            var needCards = FireworkPile.GetExpectedCards();

            ClueCandidate candidate = FindCardsToPlayAtHands(needCards);

            if (candidate == null)
            {
                // если нет карт, которыми игрок может походить прямо сейчас, то укажем на карты,
                // которые ни в коем случае нельзя сбрасывать
                var uniqueCards = Pile.GetUniqueCards(FireworkPile, DiscardPile);
                candidate = FindUniqueCardsAtHands(uniqueCards);
            }

            if (candidate == null)
            {
                // снова нет...
                // тогда ищем карты, которыми игрок может сходить в будущем

                var whateverPlayCards = 
                    Pile.GetCardsWhateverToPlay(FireworkPile, DiscardPile);

                candidate = FindWhateverPlayCardsAtHands(whateverPlayCards);
            }

            return candidate;
        }
        

        public void AddCard(Card card)
        {
            Contract.Requires<ArgumentNullException>(card != null);

            _memory.Add(card);
        }

        private Card GetCardToPlay()
        {
            // получить список нужных карт
            var neededCards = FireworkPile.GetExpectedCards();

            // получить список карт, которые уже сброшены или находятся на руках у других игроков
            var excludedCards =
                Pile.GetThrownCards(FireworkPile, DiscardPile)
                    .Concat(GetOtherPlayersCards());

            List<Card> cardsToPlay = new List<Card>();
            // поискать у себя карты из списка нужных
            foreach (Guess guess in Guesses)
            {
                var probability = guess.GetProbability(neededCards, excludedCards);
                if (probability > ProbabilityThreshold)
                {
                    cardsToPlay.Add(_memory.GetCardByGuess(guess));
                }
            }

            if (cardsToPlay.Count == 0)
                return null;

            cardsToPlay = cardsToPlay
                            .OrderBy(card => (int) card.Nominal)
                            .ToList();

            return cardsToPlay.Last().Nominal == Number.Five ? cardsToPlay.Last() : cardsToPlay.First();
        }

        private List<Card> FindCardsAtPlayer(Player player, IEnumerable<Card> cardsToSearch)
        {
            // надо исключить карты, о которых игрок знает всё.
            return player.ShowCards(this)
                        .Intersect(cardsToSearch)
                        .OrderBy(card => card.Nominal)
                        .ToList();
        }

        // нужно выбросить ту карту, которая не нужна.
        private Card GetCardToDiscard()
        {
            var uniqueCards = Pile.GetUniqueCards(FireworkPile, DiscardPile);

            var thrownCards = Pile.GetThrownCards(FireworkPile, DiscardPile);
            var otherPlayerCards = GetOtherPlayersCards();

            var excludedCards = thrownCards.Concat(otherPlayerCards);
            // для каждой карты оценим вероятность того, что она - карта - критичная
            // выбросим карту с наименьшей вероятностью

            return _memory.GetCardByGuess(
                            Guesses
                                .OrderBy(guess => guess.GetProbability(uniqueCards, excludedCards))
                                .First());
        }

        private IReadOnlyList<Card> GetOtherPlayersCards()
        {
            List<Card> result = new List<Card>();

            return Players
                    .Aggregate(result, (current, player) => current.Concat(player.ShowCards(this)).ToList())
                    .Where(card => card != null)
                    .ToList()
                    .AsReadOnly();
        }

        public bool KnowAllAboutNominalAndColor(Card card)
        {
            Contract.Requires<ArgumentNullException>(card != null);

            return _memory[card].KnowAboutNominalAndColor();
        }

        private ClueCandidate FindCardsToPlayAtHands(IEnumerable<Card> cardsToSearch)
        {
            ClueCandidate candidate = null;
            foreach (Player player in Players)
            {
                var cards = FindCardsAtPlayer(player, cardsToSearch);

                // уберём карты, цвет и номинал которых игрок знает.
                cards = cards.FindAll(card => !player.KnowAllAboutNominalAndColor(card));

                if (cards.Count > 0)
                {
                    // если есть 5, то пусть ходит 5
                    // иначе пусть ходит картой с наименьшим номиналом
                    candidate = new ClueCandidate
                    {
                        Player = player,
                        Card = cards.Last().Nominal == Number.Five ? cards.Last() : cards.First(),
                    };
                    break;
                }
            }
            return candidate;
        }

        private ClueCandidate FindUniqueCardsAtHands(IEnumerable<Card> uniqueCards)
        {
            ClueCandidate candidate = null;

            foreach (var player in Players)
            {
                var cardToClue = FindCardsAtPlayer(player, uniqueCards)
                    .FindAll(card => !player.KnowAllAboutNominalAndColor(card))
                    .FirstOrDefault();

                if (cardToClue != null)
                {
                    candidate = new ClueCandidate
                    {
                        Card = cardToClue,
                        Player = player,
                    };
                    break;
                }
            }

            return candidate;
        }

        private ClueCandidate FindWhateverPlayCardsAtHands(IEnumerable<Card> cardsToSearch)
        {
            ClueCandidate candidate = null;

            foreach (var player in Players)
            {
                var cardToClue = FindCardsAtPlayer(player, cardsToSearch)
                    .FindAll(card => !player.KnowAllAboutNominalAndColor(card))
                    .FirstOrDefault();

                if (cardToClue != null)
                {
                    candidate = new ClueCandidate
                    {
                        Card = cardToClue,
                        Player = player,
                    };
                    break;
                }
            }

            return candidate;
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

        private void RaiseCardAddedEvent()
        {
            if (CardAdded != null)
                CardAdded(this, new EventArgs());
        }

        private void RaiseBlownEvent()
        {
            if (Blown != null)
                Blown(this, new EventArgs());
        }
    }
}
