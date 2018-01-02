using System;
using System.Collections.Generic;
using System.Linq;
using Hanabi;
using NUnit.Framework;

namespace HanabiTest
{
    #region Stubs
    public class PlayerContextStub : IPlayerContext
    {
        public Player Player { get; set; }
        public IEnumerable<CardInHand> Hand { get; set; }
        public ClueType PossibleClue { get; set; }

        public Predicate<CardInHand> IsSubtleCluePredicate { get; set; }
        public Predicate<CardInHand> KnowAboutRankOrColorPredicate { get; set; }

        public Dictionary<CardInHand, List<ClueType>> CluesAboutCard { get; set; } = new Dictionary<CardInHand, List<ClueType>>();
        
        public bool IsSubtleClue(CardInHand cardInHand, IEnumerable<Card> expectedCards)
        {
            return IsSubtleCluePredicate(cardInHand);
        }

        public IList<ClueType> GetCluesAboutCard(CardInHand cardInHand)
        {
            return CluesAboutCard.ContainsKey(cardInHand)? 
                CluesAboutCard[cardInHand] : 
                new List<ClueType>();
        }

        public bool KnowAboutRankOrColor(CardInHand cardInHand)
        {
            return KnowAboutRankOrColorPredicate(cardInHand);
        }

        public IPlayerContext Clone()
        {
            throw new NotImplementedException();
        }
    }
    
    public class BoardContextStub : IBoardContext
    {
        public FireworkPile Firework { get; set; }
        public DiscardPile DiscardPile { get; set; }
        public IEnumerable<Card> ExpectedCards { get; set; }
        public IEnumerable<Card> UniqueCards { get; set; }
        public IEnumerable<Card> WhateverToPlayCards { get; set; }
        public IEnumerable<Card> ExcludedCards { get; set; }

        public IEnumerable<Card> GetExpectedCards()
        {
            return ExpectedCards;
        }

        public IEnumerable<Card> GetUniqueCards()
        {
            return UniqueCards;
        }

        public IEnumerable<Card> GetWhateverToPlayCards()
        {
            return WhateverToPlayCards;
        }

        public IEnumerable<Card> GetExcludedCards()
        {
            return ExcludedCards;
        }

        public void AddToFirework(Card card)
        {
            throw new NotImplementedException();
        }

        public void Discard(Card card)
        {
            throw new NotImplementedException();
        }

        public IBoardContext ChangeContext(IEnumerable<Card> otherPlayerCards)
        {
            throw new NotImplementedException();
        }
    }

    #endregion
    

    public class FakeGameProvider : IGameProvider
    {
        public Matrix FullDeckMatrix { get; set; }
        
        public Matrix CreateEmptyMatrix()
        {
            return new Matrix(this);
        }

        public Matrix CreateFullDeckMatrix()
        {
            return FullDeckMatrix;
        }

        public IReadOnlyList<Color> Colors { get; set; }
        public IReadOnlyList<Rank> Ranks { get; set; }
        
        public int ColorToInt(Color color)
        {
            for (int i = 0; i < Colors.Count; i++)
            {
                if (color == Colors[i]) return i;
            }
            return -1;
        }

        public int GetMaximumScore()
        {
            return Ranks.Count * Colors.Count;
        }
    }

    public static class GameProviderFabric
    {
        public static FakeGameProvider Create(Color color)
        {
            return Create(new List<Color> {color}.AsReadOnly());
        }

        public static FakeGameProvider Create(Color one, Color two)
        {
            return Create(new List<Color> {one, two});
        }

        public static FakeGameProvider Create(IEnumerable<Color> colors)
        {
            FakeGameProvider gameProvider = new FakeGameProvider
            {
                Colors = colors.ToList(),
                Ranks = new List<Rank> { Rank.One, Rank.Two, Rank.Three, Rank.Four, Rank.Five }
            };

            gameProvider.FullDeckMatrix = gameProvider.CreateEmptyMatrix();

            foreach (var color in colors)
            {
                gameProvider.FullDeckMatrix[Rank.One, color] = 3;
                gameProvider.FullDeckMatrix[Rank.Two, color] = 2;
                gameProvider.FullDeckMatrix[Rank.Three, color] = 2;
                gameProvider.FullDeckMatrix[Rank.Four, color] = 2;
                gameProvider.FullDeckMatrix[Rank.Five, color] = 1;
            }

            return gameProvider;
        }
    }

    public static class PlayerContextFabric
    {
        public static PlayerContextStub CreateStub(Player player, IEnumerable<CardInHand> hand)
        {
            var playerContext = new PlayerContextStub
            {
                Player = player,
                Hand = hand,
                IsSubtleCluePredicate = cardInHand => false,
                KnowAboutRankOrColorPredicate = cardInHand => false,
            };

            return playerContext;
        }
    }

    public static class TestHelper
    {
        public static void AreMatrixEqual(Matrix expected, Matrix actual, IGameProvider provider)
        {
            foreach (var number in provider.Ranks)
            {
                foreach (var color in provider.Colors)
                {
                    Assert.AreEqual(expected[number, color], actual[number, color]);
                }
            }

            Assert.Pass();
        }
    }
}
