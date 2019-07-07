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
        public static IList<ClueType> CreateClues(CardInHand card, IPlayerContext playerContext)
        {
            if (card == null) throw new ArgumentNullException(nameof(card));
            if (playerContext == null) throw new ArgumentNullException(nameof(playerContext));
            // TODO create more concise error description
            if (card.Player != playerContext.Player) throw new ArgumentException("Different players");

            return new List<ClueType> {new ClueAboutRank(card.Card.Rank), new ClueAboutColor(card.Card.Color)}
                .Except(playerContext.GetCluesAboutCard(card))
                .ToList();
        }

        public static IList<ClueType> CreateClues(IPlayerContext playerContext, CardInHand card)
        {
            return CreateClues(card, playerContext);
        }
    }
}