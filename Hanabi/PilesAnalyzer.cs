using System;
using System.Collections.Generic;
using System.Linq;

namespace Hanabi
{
    public class PilesAnalyzer
    {
        private readonly IGameProvider _provider;
        private readonly CardsToMatrixConverter _converter;

        public PilesAnalyzer(IGameProvider provider)
        {
            _provider = provider;
            _converter = new CardsToMatrixConverter(provider);
        }

        // карты, которые нельзя сбрасывать, для которых верно:
        // их нет в фейерверке;
        // они должны быть в фейерверке;
        // на руках или в колоде осталась всего одна такая карта.
        public IReadOnlyList<Card> GetUniqueCards(FireworkPile fireworkPile, DiscardPile discardPile)
        {
            if (fireworkPile == null) throw new ArgumentNullException(nameof(fireworkPile));
            if (discardPile == null) throw new ArgumentNullException(nameof(discardPile));

            Matrix resultMatrix = _provider.CreateFullDeckMatrix();

            // вычитаем сброшенные карты
            Matrix thrown = _converter.Encode(discardPile.Cards);

            foreach (var number in _provider.Ranks)
            {
                foreach (var color in _provider.Colors)
                {
                    if (resultMatrix[number, color] > 0 && thrown[number, color] > 0)
                        resultMatrix[number, color] -= thrown[number, color];
                }
            }

            // уберём недостижимые карты
            foreach (var color in _provider.Colors)
            {
                bool wasZero = false;
                foreach (var number in _provider.Ranks)
                {
                    if (!wasZero)
                    {
                        if (resultMatrix[number, color] == 0)
                        wasZero = true;
                    }
                    else
                    {
                        resultMatrix[number, color] = 0;
                    }
                }
            }

            // учтём сыгранные карты
            Matrix played = _converter.Encode(fireworkPile.Cards);

            foreach (var number in _provider.Ranks)
            {
                foreach (var color in _provider.Colors)
                {
                    if (played[number, color] == 1)
                        resultMatrix[number, color] = 0;
                }
            }

            // интересуют только те ячейки, в которых значение 1
            // остальные зануляем
            foreach (var number in _provider.Ranks)
            {
                foreach (var color in _provider.Colors)
                {
                    if (resultMatrix[number, color] != 1)
                        resultMatrix[number, color] = 0;
                }
            }

            // в итоге получили матрицу, в которой 
            // 0 стоит у неуникальных карт,
            // 1 стоит у уникальных карт
            return _converter.Decode(resultMatrix);
        }

        /// <summary>
        /// Возвращает карты, которые уже вне игры: то есть карты из сброса и фейерверка.
        /// </summary>
        /// <param name="fireworkPile"></param>
        /// <param name="discardPile"></param>
        /// <returns></returns>
        public IReadOnlyList<Card> GetThrownCards(FireworkPile fireworkPile, DiscardPile discardPile)
        {
            if (fireworkPile == null) throw new ArgumentNullException(nameof(fireworkPile));
            if (discardPile == null) throw new ArgumentNullException(nameof(discardPile));

            return fireworkPile.Cards.Concat(discardPile.Cards)
                .ToList()
                .AsReadOnly();
        }

        /// <summary>
        /// Возвращает карты, которые могут быть добавлены в фейерверк
        /// </summary>
        /// <param name="fireworkPile"></param>
        /// <param name="discardPile"></param>
        /// <returns></returns>
        public IReadOnlyList<Card> GetCardsWhateverToPlay(FireworkPile fireworkPile, DiscardPile discardPile)
        {
            if (fireworkPile == null) throw new ArgumentNullException(nameof(fireworkPile));
            if (discardPile == null) throw new ArgumentNullException(nameof(discardPile));

            Matrix possibleToPlay = _provider.CreateFullDeckMatrix();

            Matrix played = _converter.Encode(fireworkPile.Cards);

            foreach (var number in _provider.Ranks)
            {
                foreach (var color in _provider.Colors)
                {
                    if (played[number, color] == 1)
                        possibleToPlay[number, color] = 0;
                    else
                    {
                        possibleToPlay[number, color] = 1;
                    }
                }
            }

            // теперь вычитаем сброшенные карты
            Matrix thrown = _converter.Encode(discardPile.Cards);

            Matrix remain = _provider.CreateFullDeckMatrix();

            foreach (var number in _provider.Ranks)
            {
                foreach (var color in _provider.Colors)
                {
                    remain[number, color] -= thrown[number, color];
                }
            }

            // итак, в матрице result у нас карты, которые могут быть добавлены в фейерверк
            // теперь надо собрать их воедино.

            foreach (var color in _provider.Colors)
            {
                // цвет зафиксирован.
                // теперь разберёмся с номиналами.
                bool wasOne = false;
                foreach (var number in _provider.Ranks)
                {
                    if (possibleToPlay[number, color] == 0 && !wasOne) continue;

                    if (remain[number, color] == 0 || wasOne)
                    {
                        wasOne = true;
                        possibleToPlay[number, color] = 0;
                    }
                }
            }

            return _converter.Decode(possibleToPlay);
        }
    }
}
