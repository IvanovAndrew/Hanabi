using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;

namespace Hanabi
{
    public class DiscardStrategy : IDiscardStrategy
    {
        private readonly IReadOnlyList<Guess> _guesses;

        public DiscardStrategy(IEnumerable<Guess> guesses)
        {
            Contract.Requires<ArgumentNullException>(guesses != null);
            Contract.Requires<ArgumentException>(guesses.Any());

            _guesses = guesses.ToList().AsReadOnly();
        }

        /// <summary>
        /// Для каждой карты оценивает вероятность быть сброшенной
        /// </summary>
        /// <param name="boardContext"></param>
        /// <returns></returns>
        public CardProbability EstimateDiscardProbability(IBoardContext boardContext)
        {
            var result = new CardProbability();

            // для каждой карты посчитаем вероятность того, что она будет сброшена
            Parallel.ForEach(
                _guesses,
                guess =>
                {
                    // я считаю вероятность того, что карта будет сброшена
                    // в идеале можно определить как вероятность того, что эта карта принадлежит множеству карт, которые уже не нужны
                    // то есть {все карты} \ {карты, которые ещё можно положить на стол}

                    var probability = 1 - guess.GetProbability(boardContext.UniqueCards, boardContext.ExcludedCards);

                    if (!result.TryAdd(guess.CardInHand, probability))
                    {
                        throw new Exception("Не получилось добавить в словарь.");
                    }
                }
            );
            
            return result;
        }

        /// <summary>
        /// Анализирует последствия возможной подсказки
        /// Возвращает true, если сброшенная карта не будет уникальной
        /// </summary>
        /// <param name="boardContext"></param>
        /// <param name="playerContext"></param>
        /// <returns></returns>
        //public bool CheckIfRightClue(IBoardContext boardContext, IPlayerContext playerContext)
        //{
        //    IEstimator discardEstimator = new DiscardEstimator(this);
        //    IList<Card> cardsToDiscard = discardEstimator.GetPossibleCards(boardContext, playerContext);

        //    //var cardsWhateverToPlay = _pilesAnalyzer.GetCardsWhateverToPlay(_board.FireworkPile, _board.DiscardPile);
        //    //if (!cardsWhateverToPlay.Contains(cardToDiscard.Card)) return true;
            
        //    var uniqueCards = _pilesAnalyzer.GetUniqueCards(_board.FireworkPile, _board.DiscardPile);

        //    return !cardsToDiscard.Intersect(uniqueCards).Any();
        //}
    }
}
