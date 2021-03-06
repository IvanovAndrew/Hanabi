﻿using Hanabi.Exceptions;
using System;
using System.Collections.Generic;
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
        private readonly PilesAnalyzer _pilesAnalyzer;

        public event EventHandler ClueGiven;

        private IReadOnlyList<Guess> Guesses => _memory.GetGuesses();

        public string Name { get; }

        private readonly Game _game;


        private DiscardPile DiscardPile => _game.Board.DiscardPile;

        private FireworkPile FireworkPile => _game.Board.FireworkPile;

        private int BlowCounter => _game.Board.BlowCounter;

        public int ClueCounter => _game.Board.ClueCounter;

        public Probability PlayProbabilityThreshold
        {
            get
            {
                if (_game.Deck.IsEmpty() && _game.Board.BlowCounter > 1)
                    return new Probability(0.5);

                return new Probability(0.8);
            }
        }

        public Probability DiscardProbabilityThreshold => new Probability(0.05);

        public IGameProvider GameProvider => _game.GameProvider;
        public int CardsCount => _memory.GetHand().Count;

        private readonly List<CardInHand> _specialCards = new List<CardInHand>();

        public Player(Game game) : this(game, "")
        {
            
        }

        public Player(Game game, string name)
        {
            if (game == null) throw new ArgumentNullException(nameof(game));

            _memory = new Knowledge(game.GameProvider);
            _pilesAnalyzer = new PilesAnalyzer(game.GameProvider);
            _game = game;
            Name = name;
        }

        public IReadOnlyList<CardInHand> ShowCards(Player to)
        {
            if (to == null) throw new ArgumentNullException(nameof(to));
            if (to == this) throw new IncorrectCardDemonstratingException("You can't see your own cards!");

            return _memory.GetHand();
        }

        public void ListenClue(Clue clue)
        {
            if (clue == null) throw new ArgumentNullException(nameof(clue));
            if (!clue.Type.IsStraightClue) throw new IncorrectClueException("You can give clues only about Rank IS or Nominal IS type");

            // эвристика
            if (clue.IsSubtleClue(FireworkPile.GetExpectedCards()))
            {
                Logger.Log.Info("It's a subtle clue");
                _specialCards.Add(clue.Cards.First());
            }

            _memory.Update(clue);
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

            bool clueGiven = false;
            if (ClueCounter > 0)
            {
                LogPlayersCards();
                clueGiven = OfferClue();
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
        /// (*) if the card can start, or can be added to a firework,
        ///     it is placed face-up on that firework's pile
        /// (*) if the card cannot be added to a firework, 
        ///     it is discarded, and the red counter is placed 
        ///     in the tin lid.
        ///
        /// In either case, the player then draws a new card, without looking at it, 
        /// and adds it to their hand.
        /// </summary>
        /// <param name="cardInHand"></param>
        public void PlayCard(CardInHand cardInHand)
        {
            if (cardInHand == null) throw new ArgumentNullException(nameof(cardInHand));
            if (cardInHand.Player != this) throw new IncorrectPlayActionException("You can play only own cards");

            _memory.Remove(cardInHand);
            _game.AddCardToFirework(this, cardInHand.Card);
        }

        /// <summary>
        /// Performing this action allows you to return a blue counter to the tin lid.
        /// The player discards a card from their hand, face-up, on to a discard pile (next to the tin).
        /// They then draw a new card, without looking at it, and add it to their hand.
        /// </summary>
        /// <param name="card"></param>
        public void DiscardCard(CardInHand card)
        {
            if (card == null) throw new ArgumentNullException(nameof(card));
            if (card.Player != this) throw new IncorrectDiscardActionException();

            _memory.Remove(card);
            _game.AddCardToDiscardPile(this, card.Card);
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
            if (ClueCounter == 0) return false;

            if (_game.Deck.IsEmpty() && !_game.GetPlayersToTurn(this).Any()) return false;

            var otherPlayerCards = GetOtherPlayersCards().Select(cih => cih.Card).Concat(GetKnownCards());
            IClueStrategy strategy;
            IBoardContext boardContext = new BoardContext.Builder()
                                            .WithBoard(_game.Board)
                                            .WithPilesAnalyzer(_pilesAnalyzer)
                                            .WithOtherPlayersCards(otherPlayerCards)
                                            .Build();
            if (ClueCounter == 1)
            {
                strategy = new LastTinClueStrategy(this, boardContext, GameProvider, otherPlayerCards);
            }
            else
            {
                strategy = new ManyTinClueStrategy(this, boardContext);
            }

            var solution = strategy.FindClueCandidate(_game.GetPlayersToTurn(this));
            var clueCandidate = solution.GetClueCandidate();

            if (clueCandidate == null)
            {
                Logger.Log.Info(String.Format($"Player {Name} has to discard, because there isn't a clue"));
                return false;
            }

            GiveClue(clueCandidate.Candidate, clueCandidate.Clue);
            return true;
        }

        private void GiveClue(Player playerToClue, Clue clue)
        {
            if (playerToClue == null) throw new ArgumentNullException(nameof(playerToClue));
            if (clue == null) throw new ArgumentNullException(nameof(clue));
            if (!clue.Type.IsStraightClue) throw new IncorrectClueException("You must say rank or color!");

            playerToClue.ListenClue(clue);

            Logger.Log.Info(string.Format(
                $"Player {Name} gives clue to player {playerToClue.Name}: {clue.Type}"));

            RaiseClueGivenEvent();
        }

        public void AddCard(CardInHand cardInHand)
        {
            if (cardInHand == null) throw new ArgumentNullException(nameof(cardInHand));
            // TODO create more concise message
            if (cardInHand.Player != this) throw new ArgumentException("cardInHand.Player  != this");

            _memory.Add(cardInHand);
        }

        private CardInHand GetCardToPlay()
        {
            // cards we need to play
            var neededCards = FireworkPile.GetExpectedCards();

            List<CardInHand> cardsToPlay = new List<CardInHand>();
            if (_specialCards.Count > 0)
            {
                Logger.Log.Info("Use a subtle clue");

                var cardToPlay = _specialCards.First();

                _specialCards.RemoveAt(0);
                if (neededCards.Any(c => cardToPlay.Card == c))
                {
                    return cardToPlay;
                }
            }
            
            // получить список карт, которые уже сброшены или находятся на руках у других игроков
            var excludedCards =
                _pilesAnalyzer.GetThrownCards(FireworkPile, DiscardPile)
                    .Concat(GetOtherPlayersCards().Select(cardInHand => cardInHand.Card));

            // поискать у себя карты из списка нужных
            foreach (Guess guess in Guesses)
            {
                var probability = guess.GetProbability(neededCards, excludedCards);
                if (probability > PlayProbabilityThreshold)
                {
                    cardsToPlay.Add(_memory.GetCardByGuess(guess));
                }
            }

            // эвристика:
            // если игрок знает, что у него есть единица, то пусть играет ей.
            // при условии, что ещё нужны единицы
            if (
                FireworkPile.GetExpectedCards().Any(card => card.Rank == Rank.One) &&
                cardsToPlay.Count == 0 && 
                BlowCounter > 1)
            {
                CardInHand card = GetCardWithRankOneToPlay();
                if (card != null)
                {
                    Logger.Log.Info("I know that I have One. I'll try to play it.");
                    cardsToPlay.Add(card);
                }
            }

            if (cardsToPlay.Count == 0)
                return null;

            cardsToPlay = cardsToPlay
                .OrderBy(card => (int) card.Card.Rank)
                .ToList();

            return cardsToPlay.Last().Card.Rank == Rank.Five ? cardsToPlay.Last() : cardsToPlay.First();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private CardInHand GetCardWithRankOneToPlay()
        {
            // Если мы знаем, что одна из карт -- единица неизвестного цвета, то ходим ей.
            foreach (var card in _memory.GetHand())
            {
                foreach (var clue in _memory.GetCluesAboutCard(card))
                {
                    if (IsClueAboutOneRank(clue) &&
                        !KnowAboutNominalAndColor(card))
                    {
                        Logger.Log.InfoFormat("Player {0} finds cards with rank One", Name);
                        return card;
                    }
                }
            }
            return null;

            bool IsClueAboutOneRank(ClueType clue)
            {
                return new ClueAboutRank(Rank.One).Equals(clue);
            }
        }

        // Выбросить нужно ту карту, которая не нужна => P(карта сыграна U уже не может быть сыграна) -> max
        // Нужно как-то учесть критичность карты, чтобы ненароком не выбросить нужное.
        // P(карта сыграна U уже не может быть сыграна) -> max И
        // P(карта не критичная) -> min
        private CardInHand GetCardToDiscard()
        {
            var uniqueCards = _pilesAnalyzer.GetUniqueCards(FireworkPile, DiscardPile);

            var thrownCards = _pilesAnalyzer.GetThrownCards(FireworkPile, DiscardPile);
            var otherPlayerCards = GetOtherPlayersCards().Select(cih => cih.Card).ToList();

            var excludedCards = thrownCards.Concat(otherPlayerCards);

            //var boardContext = BoardContext.Create(_game.Board, _pilesAnalyzer, otherPlayerCards);
            //var playerContext = new PlayerContext(this, );

            //var discardStrategy = new DiscardStrategy();
            //discardStrategy.EstimateDiscardProbability(boardContext, playerContext);

            var cardsWhateverToPlay = 
                _pilesAnalyzer.GetCardsWhateverToPlay(FireworkPile, DiscardPile);

            // разделим карты на две категории:
            // (*) о которых что-то известно (номинал или цвет),
            // (*) о которых ничего конкретного неизвестно.

            // если из тех карт, о которых что-то известно, есть те, которые можно выкинуть,
            // то выкидываем. 
            var guessesAboutKnownCards = 
                    _memory
                        .GetGuesses()
                        .Where(guess => guess.KnowAboutRankOrColor());

            bool ShouldDiscard(Guess guess) => 
                guess.GetProbability(cardsWhateverToPlay, excludedCards) < DiscardProbabilityThreshold;

            Guess guessToDiscard =
                guessesAboutKnownCards.FirstOrDefault(ShouldDiscard);

            if (guessToDiscard != null)
                return _memory.GetCardByGuess(guessToDiscard);

            // что ж, тогда копаемся со второй группой.
            // для каждой карты оценим вероятность того, что она - карта - критичная
            // выбросим карту с наименьшей вероятностью
            guessToDiscard =
                Guesses
                    .Except(guessesAboutKnownCards)
                    .OrderBy(guess => guess.GetProbability(uniqueCards, excludedCards))
                    .FirstOrDefault();

            if (guessToDiscard == null)
            {
                guessToDiscard = guessesAboutKnownCards
                    .OrderBy(guess => guess.GetProbability(uniqueCards, excludedCards))
                    .First();
            }

            return _memory.GetCardByGuess(guessToDiscard);
        }

        private IReadOnlyList<CardInHand> GetOtherPlayersCards()
        {
            var result = new List<CardInHand>();

            return _game.GetPlayersExcept(this)
                .Aggregate(result, (cards, player) => cards.Concat(player.ShowCards(this)).ToList())
                .Where(card => card != null)
                .ToList()
                .AsReadOnly();
        }

        public bool KnowAboutNominalAndColor(CardInHand cardInHand)
        {
            if (cardInHand == null) throw new ArgumentNullException(nameof(cardInHand));
            // TODO create more concise message
            if (cardInHand.Player != this) throw new ArgumentException("cardInHand.Player  != this");

            return _memory.GetGuessAboutCard(cardInHand).KnowAboutNominalAndColor();
        }

        public IReadOnlyList<ClueType> GetCluesAboutCard(CardInHand cardInHand)
        {
            if (cardInHand == null) throw new ArgumentNullException(nameof(cardInHand));
            // TODO create more concise message
            if (cardInHand.Player != this) throw new ArgumentException("cardInHand.Player  != this");

            return _memory.GetCluesAboutCard(cardInHand);
        }

        public IList<Card> GetKnownCards()
        {
            return
                Guesses
                    .Where(guess => guess.KnowAboutNominalAndColor())
                    .Select(guess => _memory.GetCardByGuess(guess).Card)
                    .ToList();
        }

        #region Raise events

        private void RaiseClueGivenEvent()
        {
            ClueGiven?.Invoke(this, new EventArgs());
        }

        #endregion

        private void LogPlayersCards()
        {
            string logEntry = Environment.NewLine;
            foreach (var player in _game.GetPlayersToTurn(this))
            {
                logEntry += $"Player {player.Name} hand: {Environment.NewLine}";

                foreach (var card in player.ShowCards(this))
                {
                    string logString = $"\t{card}";
                    var logClueVisitor = new LogClueVisitor();
                    foreach (ClueType clue in player._memory.GetCluesAboutCard(card))
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