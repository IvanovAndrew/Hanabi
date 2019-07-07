using System;
using System.Collections.Generic;
using System.Linq;

namespace Hanabi
{
    class ManyTinClueStrategy : IClueStrategy
    {
        private readonly Player _clueGiver;
        private readonly IBoardContext _boardContext;

        public ManyTinClueStrategy(Player clueGiver, IBoardContext boardContext)
        {
            _clueGiver = clueGiver ?? throw new ArgumentNullException(nameof(clueGiver));
            _boardContext = boardContext ?? throw new ArgumentNullException(nameof(boardContext));
        }

        public IClueSituationStrategy FindClueCandidate(IReadOnlyList<Player> players)
        {
            // поищем подсказки "на ход"

            var expectedCards = _boardContext.GetExpectedCards();

            var expectedOneRankedCards = expectedCards.Where(card => card.Rank == Rank.One);

            if (expectedOneRankedCards.Any())
            {
                var situation = FindClueToPlayOneRankedCard(players, expectedOneRankedCards);

                if (situation != null)
                {
                    return situation;
                }
                expectedCards = expectedCards.Except(expectedOneRankedCards);
            }


            foreach (var player in players)
            {
                var playerContext = new PlayerContext(player, player.ShowCards(_clueGiver));
                ClueType clue = FindClueToPlay(playerContext, expectedCards);

                if (clue == null) continue;

                return new OnlyClueExistsSituation(playerContext, clue);
            }

            var uniqueCards = _boardContext.GetUniqueCards();
            // поищем подсказки из серии "как бы чего не вышло"
            foreach (var player in players)
            {
                var playerContext = new PlayerContext(player, player.ShowCards(_clueGiver));
                ClueType clue = FindClueToDiscard(playerContext, uniqueCards);

                if (clue == null) continue;
                
                return new OnlyClueExistsSituation(playerContext, clue);
            }


            // поищем подсказки из серии "на будущее"
            var whateverToPlayCards =
                _boardContext.GetWhateverToPlayCards();

            foreach (var player in players)
            {
                var playerContext = new PlayerContext(player, player.ShowCards(_clueGiver));
                ClueType clue = FindClueToWhateverPlay(playerContext, whateverToPlayCards);

                if (clue == null) continue;

                return new OnlyClueExistsSituation(playerContext, clue);
            }

            return new ClueNotExistsSituation();
        }

        private IClueSituationStrategy FindClueToPlayOneRankedCard(IReadOnlyList<Player> players, IEnumerable<Card> expectedOneRankedCards)
        {
            var knownOneRankedCards = new List<Card>();

            var allOneRankedCards = new List<CardInHand>();

            foreach (var player in players)
            {
                var oneRankedCards = player
                                    .ShowCards(_clueGiver)
                                    .Where(cardInHand => expectedOneRankedCards.Contains(cardInHand.Card));

                allOneRankedCards = allOneRankedCards.Concat(oneRankedCards).ToList();

                var clueAboutOneRank = new ClueAboutRank(Rank.One);
                foreach (var card in oneRankedCards)
                {
                    if (player.GetCluesAboutCard(card).Any(c => clueAboutOneRank.Equals(c)))
                    {
                        knownOneRankedCards.Add(card.Card);
                    }
                }
            }

            var unknownOneRankedCards =
                expectedOneRankedCards
                    .Except(knownOneRankedCards)
                    .ToList();

            if (!unknownOneRankedCards.Any()) return null;


            var cardsToClue = 
                allOneRankedCards.Where(cardInHand => unknownOneRankedCards.Contains(cardInHand.Card));

            if (!cardsToClue.Any()) return null;

            var playerToClue =
                cardsToClue
                    .GroupBy(cih => cih.Player)
                    .OrderByDescending(group => group.Count())
                    .Select(group => group.Key)
                    .First();

            var playerContext = new PlayerContext(playerToClue, playerToClue.ShowCards(_clueGiver));

            return new OnlyClueExistsSituation(playerContext, new ClueAboutRank(Rank.One));
        }

        private ClueType FindClueToPlay(IPlayerContext playerContext, IEnumerable<Card> expectedCards)
        {
            // сразу уберём карты, о которых игрок знает.
            var cardsToSearch = expectedCards.Except(playerContext.Player.GetKnownCards()).ToList();

            if (!cardsToSearch.Any()) return null;

            var cardsToPlay =
                playerContext
                    .Hand
                    .Where(cardInHand => cardsToSearch.Contains(cardInHand.Card))
                    .Where(cardInHand => !IsKnownOneRankedCard(cardInHand))
                    .ToList();

            if (!cardsToPlay.Any()) return null;

            cardsToPlay =
                cardsToPlay.OrderBy(cardInHand => (int) cardInHand.Card.Rank).ToList();

            var cardToClue =
                cardsToPlay.Last().Card.Rank == Rank.Five ? cardsToPlay.Last() : cardsToPlay.First();

            return ClueDetailInfo.CreateClues(cardToClue, playerContext).FirstOrDefault();

            bool IsKnownOneRankedCard(CardInHand cardInHand)
            {
                return 
                    playerContext
                        .GetCluesAboutCard(cardInHand)
                        .Any(clue => Equals(new ClueAboutRank(Rank.One), clue));
            }
        }

        private ClueType FindClueToDiscard(IPlayerContext playerContext, IEnumerable<Card> uniqueCards)
        {
            // сразу уберём карты, о которых игрок знает.
            var cardsToSearch = uniqueCards.Except(playerContext.Player.GetKnownCards()).ToList();

            if (!cardsToSearch.Any()) return null;

            var uniqueUnknownCards =
                playerContext
                    .Hand
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

            return ClueDetailInfo.CreateClues(cardToClue, playerContext).First();

            bool KnownFiveRankedCards(CardInHand cardInHand)
            {
                if (cardInHand.Card.Rank != Rank.Five) return false;

                var clueAboutFiveRank = new ClueAboutRank(Rank.Five);
                return
                    playerContext
                        .GetCluesAboutCard(cardInHand)
                        .Any(clue => clueAboutFiveRank.Equals(clue));
            }
        }

        private ClueType FindClueToWhateverPlay(IPlayerContext playerContext, IEnumerable<Card> whateverToPlayCards)
        {
            // сразу уберём карты, о которых игрок знает.
            var cardsToSearch = whateverToPlayCards.Except(playerContext.Player.GetKnownCards()).ToList();

            if (!cardsToSearch.Any()) return null;

            var unknownCards =
                playerContext
                    .Hand
                    .Where(cardInHand => cardsToSearch.Contains(cardInHand.Card))
                    .ToList();

            if (!unknownCards.Any()) return null;

            // TODO изучить...
            unknownCards =
                unknownCards
                    .OrderBy(cardInHand => (int) cardInHand.Card.Rank)
                    .ToList();

            var cardToClue = unknownCards.FirstOrDefault();
            if (cardToClue == null) return null;

            return ClueDetailInfo.CreateClues(playerContext, cardToClue).First();
        }
    }
}