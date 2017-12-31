using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Hanabi
{
    public class ClueDetailInfo
    {
        public Rank? Rank { get; private set; }
        public Color? Color { get; private set; }

        public static ClueDetailInfo GetClueInfo(Clue clue)
        {
            Contract.Requires<ArgumentNullException>(clue != null);
            Contract.Ensures(Contract.Result<ClueDetailInfo>() != null);

            var clueDetailInfo = new ClueDetailInfo();

            ClueAboutRankVisitor rankVisitor = new ClueAboutRankVisitor();
            ClueAboutColorVisitor colorVisitor = new ClueAboutColorVisitor();
            if (clue.Accept(rankVisitor))
            {
                clueDetailInfo.Rank = rankVisitor.Rank;
            }
            else if (clue.Accept(colorVisitor))
            {
                clueDetailInfo.Color = colorVisitor.Color;
            }
            else
            {
                throw new InvalidOperationException("Unexpected type of clue!");
            }

            return clueDetailInfo;
        }

        /// <summary>
        /// Возвращает подсказки, которые можно дать игроку на карту.
        /// Убирает подсказки, которые давали игроку ранее
        /// </summary>
        /// <param name="card"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public static IList<Clue> CreateClues(CardInHand card, Player player)
        {
            Contract.Requires<ArgumentNullException>(card != null);
            Contract.Requires<ArgumentNullException>(player != null);
            Contract.Requires(card.Player == player);
            var contractResult = Contract.Result<IList<Clue>>();
            Contract.Ensures(contractResult != null);
            Contract.Ensures(Contract.ForAll(contractResult, clue => clue.IsStraightClue));

            return new List<Clue> { new ClueAboutRank(card.Card.Rank), new ClueAboutColor(card.Card.Color) }
                .Except(player.GetCluesAboutCard(card))
                .ToList();
        }

        public static IList<Clue> CreateClues(Player player, CardInHand card)
        {
            return CreateClues(card, player);
        }

        /// <summary>
        /// Возвращает карты, которые напрямую затрагиваются подсказкой.
        /// TODO должна жить где-то в другом месте
        /// </summary>
        /// <param name="hand"></param>
        /// <param name="clue"></param>
        /// <returns></returns>
        public static IList<CardInHand> GetCardsToClue(IEnumerable<CardInHand> hand, Clue clue)
        {
            Contract.Requires<ArgumentNullException>(hand != null);
            Contract.Requires(hand.Any());
            Contract.Requires<ArgumentNullException>(clue != null);
            var contractResult = Contract.Result<IList<CardInHand>>();
            Contract.Ensures(contractResult != null);
            Contract.Ensures(contractResult.Any());
            Contract.Ensures(Contract.ForAll(contractResult, hand.Contains));

            var result = new List<CardInHand>();

            foreach (var cardInHand in hand)
            {
                if (ClueAndCardMatcher.Match(cardInHand.Card, clue).IsStraightClue)
                    result.Add(cardInHand);
            }

            return result;
        }
    }

    public static class ClueAndCardMatcher
    {
        public static Clue Match(Clue clue, Card card)
        {
            return Match(card, clue);
        }
        
        public static Clue Match(Card card, Clue clue)
        {
            Contract.Requires<ArgumentNullException>(card != null);
            Contract.Requires<ArgumentNullException>(clue != null);
            Contract.Ensures(Contract.Result<Clue>() != null);
            
            ClueDetailInfo clueDetail = ClueDetailInfo.GetClueInfo(clue);

            return card.Color == clueDetail.Color || card.Rank == clueDetail.Rank ? 
                clue : 
                clue.Revert();
        }
    }
}