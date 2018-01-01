using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Hanabi
{
    public class ClueDetailInfo
    {
        /// <summary>
        /// Возвращает подсказки, которые можно дать игроку на карту.
        /// Убирает подсказки, которые давали игроку ранее
        /// </summary>
        /// <param name="card"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public static IList<ClueType> CreateClues(CardInHand card, Player player)
        {
            Contract.Requires<ArgumentNullException>(card != null);
            Contract.Requires<ArgumentNullException>(player != null);
            Contract.Requires(card.Player == player);
            var contractResult = Contract.Result<IList<ClueType>>();
            Contract.Ensures(contractResult != null);
            Contract.Ensures(Contract.ForAll(contractResult, clue => clue.IsStraightClue));

            return new List<ClueType> {new ClueAboutRank(card.Card.Rank), new ClueAboutColor(card.Card.Color)}
                .Except(player.GetCluesAboutCard(card))
                .ToList();
        }

        public static IList<ClueType> CreateClues(Player player, CardInHand card)
        {
            return CreateClues(card, player);
        }
    }
}