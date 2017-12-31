using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Hanabi
{
    class ManyTinClueStrategy : IClueStrategy
    {
        private readonly Player _clueGiver;
        private readonly IBoardContext _boardContext;

        public ManyTinClueStrategy(Player clueGiver, IBoardContext boardContext)
        {
            Contract.Requires<ArgumentNullException>(clueGiver != null);
            Contract.Requires<ArgumentNullException>(boardContext != null);

            _clueGiver = clueGiver;
            _boardContext = boardContext;
        }

        public HardSolution FindClueCandidate(IReadOnlyList<Player> players)
        {
            Logger.Log.Info($"Many tin clue strategy. First player is {players[0].Name}");
            // поищем подсказки "на ход"

            var expectedCards = _boardContext.GetExpectedCards();

            foreach (var player in players)
            {
                Clue clue = FindClueToPlay(player, expectedCards);

                if (clue == null) continue;

                return new HardSolution
                {
                    Clue = clue,
                    PlayerToClue = player,
                    Situation = ClueSituation.ClueExists,
                };
            }

            var uniqueCards = _boardContext.GetUniqueCards();
            // поищем подсказки из серии "как бы чего не вышло"
            foreach (var player in players)
            {
                Clue clue = FindClueToDiscard(player, uniqueCards);

                if (clue == null) continue;

                return new HardSolution
                {
                    Clue = clue,
                    PlayerToClue = player,
                    Situation = ClueSituation.ClueExists,
                };
            }


            // поищем подсказки из серии "на будущее"
            var whateverToPlayCards =
                _boardContext.GetWhateverToPlayCards();
                //_pilesAnalyzer.GetCardsWhateverToPlay(_boardContext.Firework, _boardContext.DiscardPile).ToList();

            foreach (var player in players)
            {
                Clue clue = FindClueToWhateverPlay(player, whateverToPlayCards);

                if (clue == null) continue;

                return new HardSolution
                {
                    Clue = clue,
                    PlayerToClue = player,
                    Situation = ClueSituation.ClueExists,
                };
            }

            return new HardSolution {Situation = ClueSituation.ClueDoesntExist};
        }

        private Clue FindClueToPlay(Player playerToClue, IEnumerable<Card> expectedCards)
        {
            // сразу уберём карты, о которых игрок знает.
            var cardsToSearch = expectedCards.Except(playerToClue.GetKnownCards()).ToList();

            if (!cardsToSearch.Any()) return null;

            var cardsToPlay =
                playerToClue
                    .ShowCards(_clueGiver)
                    .Where(cardInHand => cardsToSearch.Contains(cardInHand.Card))
                    .Where(cardInHand => !IsKnownOneRankedCard(cardInHand))
                    .ToList();

            if (!cardsToPlay.Any()) return null;

            cardsToPlay =
                cardsToPlay.OrderBy(cardInHand => (int)cardInHand.Card.Rank).ToList();

            var cardToClue =
                cardsToPlay.Last().Card.Rank == Rank.Five ? cardsToPlay.Last() : cardsToPlay.First();

            return ClueDetailInfo.CreateClues(cardToClue, playerToClue).FirstOrDefault();

            bool IsKnownOneRankedCard(CardInHand cardInHand)
            {
                return playerToClue.GetCluesAboutCard(cardInHand)
                    .Where(clue => clue.IsStraightClue)
                    .Any(clue => ClueDetailInfo.GetClueInfo(clue).Rank == Rank.One);
            }
        }

        private Clue FindClueToDiscard(Player playerToClue, IEnumerable<Card> uniqueCards)
        {
            // сразу уберём карты, о которых игрок знает.
            var cardsToSearch = uniqueCards.Except(playerToClue.GetKnownCards()).ToList();

            if (!cardsToSearch.Any()) return null;

            var uniqueUnknownCards =
                playerToClue
                    .ShowCards(_clueGiver)
                    .Where(cardInHand => cardsToSearch.Contains(cardInHand.Card))
                    .ToList();

            if (!uniqueUnknownCards.Any()) return null;

            uniqueUnknownCards = 
                uniqueUnknownCards
                    .OrderBy(cardInHand => (int) cardInHand.Card.Rank)
                    .ToList();

            // если игрок знает о пятёрке, то указывать на неё не будем
            var cardToClue =
                uniqueUnknownCards.FirstOrDefault(cardInHand => !KnownFiveRankedCards(cardInHand));

            if (cardToClue == null) return null;

            return ClueDetailInfo.CreateClues(cardToClue, playerToClue).First();

            bool KnownFiveRankedCards(CardInHand cardInHand)
            {
                if (cardInHand.Card.Rank != Rank.Five) return false;

                ClueAboutRankVisitor visitor = new ClueAboutRankVisitor();
                return 
                    playerToClue
                        .GetCluesAboutCard(cardInHand)
                        .Any(clue => clue.Accept(visitor));
            }
        }

        private Clue FindClueToWhateverPlay(Player playerToClue, IEnumerable<Card> whateverToPlayCards)
        {
            // сразу уберём карты, о которых игрок знает.
            var cardsToSearch = whateverToPlayCards.Except(playerToClue.GetKnownCards()).ToList();

            if (!cardsToSearch.Any()) return null;

            var unknownCards =
                playerToClue
                    .ShowCards(_clueGiver)
                    .Where(cardInHand => cardsToSearch.Contains(cardInHand.Card))
                    .ToList();

            if (!unknownCards.Any()) return null;

            // TODO изучить...
            unknownCards =
                unknownCards
                    .OrderBy(cardInHand => (int)cardInHand.Card.Rank)
                    .ToList();

            var cardToClue = unknownCards.FirstOrDefault();

            if (cardToClue == null) return null;

            return ClueDetailInfo.CreateClues(playerToClue, cardToClue).First();
        }
    }
}

