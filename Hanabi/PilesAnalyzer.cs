using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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
            Contract.Requires<ArgumentNullException>(fireworkPile != null);
            Contract.Requires<ArgumentNullException>(discardPile != null);

            Contract.Ensures(Contract.Result<IReadOnlyList<Card>>() != null);

            Matrix resultMatrix = _provider.CreateFullDeckMatrix();

            // вычитаем сброшенные карты
            Matrix thrown = _converter.Encode(discardPile.Cards);

            foreach (var number in _provider.Numbers)
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
                foreach (var number in _provider.Numbers)
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

            foreach (var number in _provider.Numbers)
            {
                foreach (var color in _provider.Colors)
                {
                    if (played[number, color] == 1)
                        resultMatrix[number, color] = 0;
                }
            }

            // интересуют только те ячейки, в которых значение 1
            // остальные зануляем
            foreach (var number in _provider.Numbers)
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

        public IReadOnlyList<Card> GetThrownCards(FireworkPile fireworkPile, DiscardPile discardPile)
        {
            Contract.Requires<ArgumentNullException>(fireworkPile != null);
            Contract.Requires<ArgumentNullException>(discardPile != null);

            Contract.Ensures(Contract.Result<IReadOnlyList<Card>>() != null);

            return fireworkPile.Cards.Concat(discardPile.Cards)
                .ToList()
                .AsReadOnly();
        }

        public IReadOnlyList<Card> GetCardsWhateverToPlay(FireworkPile fireworkPile, DiscardPile discardPile)
        {
            Contract.Requires<ArgumentNullException>(fireworkPile != null);
            Contract.Requires<ArgumentNullException>(discardPile != null);

            Contract.Ensures(Contract.Result<IReadOnlyList<Card>>() != null);

            Matrix possibleToPlay = _provider.CreateFullDeckMatrix();

            Matrix played = _converter.Encode(fireworkPile.Cards);

            foreach (var number in _provider.Numbers)
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

            foreach (var number in _provider.Numbers)
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
                foreach (var number in _provider.Numbers)
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
