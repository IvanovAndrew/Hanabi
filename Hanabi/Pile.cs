using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Hanabi
{
    public abstract class Pile
    {
        protected readonly List<Card> _cards = new List<Card>();

        public IReadOnlyList<Card> Cards
        {
            get { return _cards.AsReadOnly(); }
        }

        public static int[,] DefaultMatrix = 
        {
            {3, 3, 3, 3, 3},
            {2, 2, 2, 2, 2},
            {2, 2, 2, 2, 2},
            {2, 2, 2, 2, 2},
            {1, 1, 1, 1, 1},
        };

        // карты, которые нельзя сбрасывать, для которых верно:
        // их нет в фейерверке;
        // они должны быть в фейерверке;
        // на руках или в колоде осталась всего одна такая карта.
        public static IReadOnlyList<Card> GetUniqueCards(FireworkPile fireworkPile, DiscardPile discardPile)
        {
            Contract.Requires<ArgumentNullException>(fireworkPile != null);
            Contract.Requires<ArgumentNullException>(discardPile != null);

            Contract.Ensures(Contract.Result<IReadOnlyList<Card>>() != null);

            int[,] resultMatrix = (int[,])DefaultMatrix.Clone();

            int[,] played = fireworkPile.ToMatrix();

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (played[i, j] == 1)
                        resultMatrix[i, j] = 0;
                }
            }

            // теперь вычитаем сброшенные карты
            int[,] thrown = discardPile.ToMatrix();

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (resultMatrix[i, j] > 0 && thrown[i, j] > 0)
                        resultMatrix[i ,j] -= thrown[i, j];
                }
            }

            // интересуют только те ячейки, в которых значение 1
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (resultMatrix[i, j] != 1)
                        resultMatrix[i, j] = 0;
                }
            }

            return CardsToMatrixConverter.Decode(resultMatrix);
        }

        public static IReadOnlyList<Card> GetThrownCards(FireworkPile fireworkPile, DiscardPile discardPile)
        {
            Contract.Requires<ArgumentNullException>(fireworkPile != null);
            Contract.Requires<ArgumentNullException>(discardPile != null);
            
            Contract.Ensures(Contract.Result<IReadOnlyList<Card>>() != null);

            return fireworkPile.Cards.Concat(discardPile.Cards)
                                    .ToList()
                                    .AsReadOnly();
        }

        public static IReadOnlyList<Card> GetCardsWhateverToPlay(FireworkPile fireworkPile, DiscardPile discardPile)
        {
            Contract.Requires<ArgumentNullException>(fireworkPile != null);
            Contract.Requires<ArgumentNullException>(discardPile != null);

            Contract.Ensures(Contract.Result<IReadOnlyList<Card>>() != null);

            int[,] possibleToPlay = (int[,])DefaultMatrix.Clone();

            int[,] played = fireworkPile.ToMatrix();

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (played[i, j] == 1)
                        possibleToPlay[i, j] = 0;
                    else
                    {
                        possibleToPlay[i, j] = 1;
                    }
                }
            }


            // теперь вычитаем сброшенные карты
            int[,] thrown = discardPile.ToMatrix();

            int[,] remain = (int[,]) DefaultMatrix.Clone();

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                        remain[i, j] -= thrown[i, j];
                }
            }

            // итак, в матрице result у нас карты, которые могут быть добавлены в фейерверк
            // теперь надо собрать их воедино.
            
            for (int j = 0; j < 5; j++)
            {
                // цвет зафиксирован.
                // теперь разберёмся с номиналами.
                bool wasOne = false;
                for (int i = 0; i < 5; i++)
                {
                    if (possibleToPlay[i,j] == 0 && !wasOne) continue;

                    if (remain[i, j] == 0 || wasOne)
                    {
                        wasOne = true;
                        possibleToPlay[i, j] = 0;
                    }
                }
            }

            return CardsToMatrixConverter.Decode(possibleToPlay);
        }

        public int[,] ToMatrix()
        {
            Contract.Ensures(Contract.Result<int[,]>() != null);
            
            return CardsToMatrixConverter.Encode(Cards);
        }

        public abstract bool AddCard(Card card);
    }
}
