using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;


namespace Hanabi
{
    public class CardAddedEventArgs : EventArgs
    {
        public Card Card { get; set; }
    }

    public class Player
    {
        private readonly Knowledge _memory;
        private readonly IGameProvider _provider;

        public Knowledge Memory
        {
            get { return _memory; }
        }

        private readonly PilesAnalyzer _pilesAnalyzer;

        public event EventHandler ClueGiven;
        public event EventHandler Discarded;
        public event EventHandler<CardAddedEventArgs> CardAdded;
        public event EventHandler Blown;

        private IReadOnlyList<Guess> Guesses
        {
            get { return _memory.GetGuesses(); }
        }

        public string Name { get; set; }

        private readonly Game _game;

        public const double ProbabilityThreshold = 0.8;

        private DiscardPile DiscardPile
        {
            get
            {
                Contract.Ensures(Contract.Result<DiscardPile>() != null);
                return _game.Board.DiscardPile;
            }
        }

        private FireworkPile FireworkPile
        {
            get
            {
                Contract.Ensures(Contract.Result<FireworkPile>() != null);

                return _game.Board.FireworkPile;
            }
        }

        private int BlowCounter
        {
            get { return _game.Board.BlowCounter; }
        }

        private int ClueCounter
        {
            get { return _game.Board.ClueCounter; }
        }

        private IReadOnlyList<Player> Players
        {
            get
            {
                Contract.Ensures(Contract.Result<IReadOnlyList<Player>>() != null);
                return _game.GetPlayersExcept(this);
            }
        }

        private readonly List<CardInHand> _specialCards = new List<CardInHand>();

        public Player(Game game, IGameProvider provider)
        {
            Contract.Requires<ArgumentNullException>(game != null);
            Contract.Requires<ArgumentNullException>(provider != null);

            _memory = new Knowledge(provider, this);
            _pilesAnalyzer = new PilesAnalyzer(provider);
            _game = game;
            _provider = provider;
        }

        public IReadOnlyList<CardInHand> ShowCards(Player to)
        {
            Contract.Requires<ArgumentException>(to != this);

            return _memory.GetHand();
        }

        public void ListenClue(IReadOnlyCollection<CardInHand> cards, Clue clue)
        {
            Contract.Requires<ArgumentNullException>(cards != null);
            Contract.Requires(cards.Any());

            Contract.Requires<ArgumentNullException>(clue != null);

            // эвристика:
            // если на столе уже лежат 3-4 единицы/двойки/тройки/четвёрки 
            // и указали на одну единицу/двойку/тройку/четвёрку, то ей надо ходить. 
            // При этом если должны быть выложены карты всех цветов номиналом ниже.
            ClueAboutNominalVisitor visitor = new ClueAboutNominalVisitor();

            if (cards.Count == 1 && clue.Accept(visitor))
            {
                Number nominalAboutClue = visitor.Nominal.Value;

                if (nominalAboutClue != Number.Five)
                {
                    bool prevFullFilled;
                    Number? prevNumber = nominalAboutClue.GetPreviousNumber();
                    if (prevNumber == null)
                    {
                        prevFullFilled = true;
                    }
                    else
                    {
                        prevFullFilled = 
                            FireworkPile.Cards.Count(card => card.Nominal == prevNumber) == _provider.Colors.Count;
                    }
                    
                    int cardsCount =
                        FireworkPile.Cards.Count(card => card.Nominal == nominalAboutClue);

                    int diff = _provider.Colors.Count - cardsCount;

                    if (prevFullFilled && 
                        (diff == 1 || diff == 2))
                    {
                        Logger.Log.Info("It's a subtle clue");
                        _specialCards.Add(cards.First());
                    }
                }
            }

            _memory.Update(cards, clue);
        }

        /// Can I play?
        ///     See Firework pile.
        ///     See my own guessed.
        ///     Can I play card?
        ///         Yes: Play (if next player has a clue)
        ///         Otherwise: No
        ///  Can I give a clue?
        ///  If Yes, see the next player card and his memory. 
        ///  Can he play after my clue?
        public void Turn()
        {
            CardInHand cardToPlay = GetCardToPlay();
            if (cardToPlay != null)
            {
                PlayCard(cardToPlay);
                return;
            }

            bool clueGiven;
            if (ClueCounter > 0)
            {
                LogPlayersCards();
                clueGiven = OfferClue();
            }
            else
            {
                clueGiven = false;
            }

            if (!clueGiven)
            {
                CardInHand card = GetCardToDiscard();
                DiscardCard(card);
            }
        }

        /// <summary>
        /// The player takes a card from their hand and places it face up in front of them.
        /// There are then 2 possibilies:
        /// (*) if the cardInHand can start, or can be added to a firework,
        ///     it is placed face-up on that firework's pile
        /// (*) if the cardInHand cannot be added to a firework, 
        ///     it is discarded, and the red counter is placed 
        ///     in the tin lid.
        ///
        /// In either case, the player then draws a new card, without looking at it, 
        /// and adds it to their hand.
        /// </summary>
        /// <param name="cardInHand"></param>
        public void PlayCard(CardInHand cardInHand)
        {
            Logger.Log.InfoFormat("{0} is played", cardInHand);

            _memory.Remove(cardInHand);
            
            bool added = FireworkPile.AddCard(cardInHand.Card);

            if (added)
            {
                Logger.Log.InfoFormat("Firework pile:{0}", FireworkPile);
                
                RaiseCardAddedEvent(cardInHand.Card);
            }
            else
            {
                Logger.Log.InfoFormat("BLOW!");

                DiscardPile.AddCard(cardInHand.Card);
                RaiseBlownEvent();
            }
        }

        /// <summary>
        /// Performing this action allows you to return a blue counter to the tin lid.
        /// The player discards a card from their hand, face-up, on to a discard pile (next to the tin).
        /// They then draw a new card, without looking at it, and add it to their hand.
        /// </summary>
        /// <param name="card"></param>
        public void DiscardCard(CardInHand card)
        {
            Contract.Requires<ArgumentNullException>(card != null);

            Logger.Log.InfoFormat("{0} discarded", card);

            _memory.Remove(card);

            DiscardPile.AddCard(card.Card);
            RaiseDiscardedEvent();
        }

        /// <summary>
        /// To perform this action, the player must remove a blue counter from the tin lid
        /// (and place it next to it, with the red counters). They can then give a clue to one of the other players 
        /// about the cards they hold.
        /// The player must indicate clearly - by pointing - which cards he is offering a clue about.
        /// A player must give a full clue: e.g. if a player has two yellow cards, the clue-giver must not indicate only one!
        /// </summary>
        public bool OfferClue()
        {
            if (ClueCounter <= 0) throw new HanabiException("Zero blue counter");

            var candidate = FindClueCandidate();

            // ну совсем нечего игроку подсказывать...
            // тогда не будем ему ничего подсказывать
            if (candidate == null) return false;

            Player playerToClue = candidate.Player;
            CardInHand cardToClue = candidate.Card;
            
            // чтобы подсказки не повторялись, посмотрим, 
            // давали ли аналогичную подсказку раньше
            IReadOnlyList<Clue> previousClues =
                playerToClue._memory.GetPreviousCluesAboutCard(cardToClue);

            IReadOnlyCollection<CardInHand> cardsToClue;
            Clue clue = new IsNominal(cardToClue.Card.Nominal);
            if (previousClues.Contains(clue))
            {
                clue = new IsColor(cardToClue.Card.Color);
                cardsToClue = playerToClue.ShowCards(this)
                                        .Where(card => card.Card.Color == cardToClue.Card.Color)
                                        .ToList()
                                        .AsReadOnly();
            }
            else
            {
                cardsToClue = playerToClue.ShowCards(this)
                                        .Where(card => card.Card.Nominal == cardToClue.Card.Nominal)
                                        .ToList()
                                        .AsReadOnly();
            }

            playerToClue.ListenClue(cardsToClue, clue);

            Logger.Log.InfoFormat("Player {0} gives clue to player {1}: {2}",
                Name,
                playerToClue.Name,
                clue);
            RaiseClueGivenEvent();

            return true;
        }

        private ClueCandidate FindClueCandidate()
        {
            // cards that we can play at this turn
            var needCards = FireworkPile.GetExpectedCards();

            ClueCandidate candidate = FindCardsToPlayAtHands(needCards);

            if (candidate == null)
            {
                // если нет карт, которыми игрок может сходить прямо сейчас, 
                // то укажем на карты, которые ни в коем случае нельзя сбрасывать
                var uniqueCards = _pilesAnalyzer.GetUniqueCards(FireworkPile, DiscardPile);
                candidate = FindUniqueCardsAtHands(uniqueCards);
            }

            if (candidate == null)
            {
                // снова нет...
                // тогда ищем карты, которыми игрок может сходить в будущем

                var whateverPlayCards =
                    _pilesAnalyzer.GetCardsWhateverToPlay(FireworkPile, DiscardPile);

                candidate = FindWhateverPlayCardsAtHands(whateverPlayCards);
            }

            return candidate;
        }
        
        public CardInHand AddCardToHand(Card card)
        {
            Contract.Requires<ArgumentNullException>(card != null);
            Contract.Ensures(Contract.Result<CardInHand>() != null);

            return _memory.Add(card);
        }

        private CardInHand GetCardToPlay()
        {
            List<CardInHand> cardsToPlay = new List<CardInHand>();
            if (_specialCards.Count > 0)
            {
                Logger.Log.Info("Use a subtle clue");
                cardsToPlay.Add(_specialCards[0]);
                _specialCards.RemoveAt(0);
            }
            else
            {
                // cards we need to play
                var neededCards = FireworkPile.GetExpectedCards();

                // получить список карт, которые уже сброшены или находятся на руках у других игроков
                var excludedCards =
                    _pilesAnalyzer.GetThrownCards(FireworkPile, DiscardPile)
                        .Concat(GetOtherPlayersCards().Select(cardInHand => cardInHand.Card));
                
                // поискать у себя карты из списка нужных
                foreach (Guess guess in Guesses)
                {
                    var probability = guess.GetProbability(neededCards, excludedCards);
                    if (probability > ProbabilityThreshold)
                    {
                        cardsToPlay.Add(_memory.GetCardByGuess(guess));
                    }
                }
            }

            if (cardsToPlay.Count == 0)
                return null;

            cardsToPlay = cardsToPlay
                            .OrderBy(card => (int) card.Card.Nominal)
                            .ToList();

            return cardsToPlay.Last().Card.Nominal == Number.Five ? cardsToPlay.Last() : cardsToPlay.First();
        }

        //private CardInHand GetCardWithNominalOneToPlay()
        //{
        //    IClueVisitor clueFinder = new ClueAboutNominalFinder(Number.One);
        //    // Если мы знаем, что одна из карт -- единица, то ходим ей.
        //    foreach (var card in _memory.GetHand())
        //    {
        //        foreach (var clue in _memory.GetPreviousCluesAboutCard(card))
        //        {
        //            if (clue.Accept(clueFinder))
        //            {
        //                Logger.Log.InfoFormat("Player {0} finds cards with nominal One", Name);
        //                return card;
        //            }
        //        }
        //    }
        //    return null;
        //}

        private List<CardInHand> FindCardsAtPlayer(Player player, IEnumerable<Card> cardsToSearch)
        {
            // надо исключить карты, о которых игрок знает всё.
            return player.ShowCards(this)
                        .Where(cardInHand => cardsToSearch.Any(card => Equals(card, cardInHand.Card)))
                        .ToList();
        }

        // нужно выбросить ту карту, которая не нужна.
        //TODO пересмотреть реализацию метода
        //Выбросить нужно ту карту, которая не нужна => P(карта сыграна U уже не может быть сыграна) -> max
        //Нужно как-то учесть критичность карты, чтобы ненароком не выбросить нужное.
        // P(карта сыграна U уже не может быть сыграна) -> max И
        // P(карта не критичная) -> min
        private CardInHand GetCardToDiscard()
        {
            var uniqueCards = _pilesAnalyzer.GetUniqueCards(FireworkPile, DiscardPile);

            var thrownCards = _pilesAnalyzer.GetThrownCards(FireworkPile, DiscardPile);
            var otherPlayerCards = GetOtherPlayersCards();

            var excludedCards = thrownCards.Concat(otherPlayerCards.Select(cardInHand => cardInHand.Card));
            // для каждой карты оценим вероятность того, что она - карта - критичная
            // выбросим карту с наименьшей вероятностью

            return _memory.GetCardByGuess(
                            Guesses
                                .OrderBy(guess => guess.GetProbability(uniqueCards, excludedCards))
                                .First());
        }

        private IReadOnlyList<CardInHand> GetOtherPlayersCards()
        {
            List<CardInHand> result = new List<CardInHand>();

            return Players
                    .Aggregate(result, (current, player) => current.Concat(player.ShowCards(this)).ToList())
                    .Where(card => card != null)
                    .ToList()
                    .AsReadOnly();
        }

        public bool KnowAllAboutNominalAndColor(CardInHand card)
        {
            Contract.Requires<ArgumentNullException>(card != null);

            return _memory.GetGuessAboutCard(card).KnowAboutNominalAndColor();
        }

        private ClueCandidate FindCardsToPlayAtHands(IEnumerable<Card> cardsToSearch)
        {
            ClueCandidate candidate = null;
            foreach (Player player in Players)
            {
                var cards = FindCardsAtPlayer(player, cardsToSearch);

                // уберём карты, цвет и номинал которых игрок знает.
                // упорядочим их по номиналу
                cards = cards.FindAll(card => !player.KnowAllAboutNominalAndColor(card))
                            .OrderBy(card => (int) card.Card.Nominal)
                            .ToList();

                if (cards.Count > 0)
                {
                    // если есть 5, то пусть ходит 5
                    // иначе пусть ходит картой с наименьшим номиналом
                    candidate = new ClueCandidate
                    {
                        Player = player,
                        Card = cards.Last().Card.Nominal == Number.Five ? cards.Last() : cards.First(),
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

        private void RaiseCardAddedEvent(Card card)
        {
            if (CardAdded != null)
                CardAdded(this, new CardAddedEventArgs {Card = card});
        }

        private void RaiseBlownEvent()
        {
            if (Blown != null)
                Blown(this, new EventArgs());
        }

        private void LogPlayersCards()
        {
            string logEntry = string.Empty;
            foreach (var player in Players)
            {
                logEntry += String.Format("Player {0} hand: {1}", player.Name, Environment.NewLine);

                foreach (var card in player.ShowCards(this))
                {
                    string logString = String.Format("\t{0}", card);
                    var logClueVisitor = new LogClueVisitor();
                    foreach (Clue clue in player._memory.GetPreviousCluesAboutCard(card))
                    {
                        clue.Accept(logClueVisitor);
                    }

                    logString += !String.IsNullOrEmpty(logClueVisitor.Build())
                        ? "(" + logClueVisitor.Build() + ")"
                        : String.Empty;

                    logEntry += logString + Environment.NewLine;
                }
            }
            Logger.Log.Info(logEntry);
        }
    }
}
