using System;
using System.Collections.Generic;
using System.Linq;

namespace Hanabi
{
    public class ClueAndAction
    {
        public ClueType Clue { get; set; }
        public PlayerAction Action { get; set; }
    }
    
    public interface IPlayActionVisitor
    {
        ClueType CreateClueToAvoidAction(PlayCardAction action);
        ClueType CreateClueToAvoidAction(BlowCardAction action);
        ClueType CreateClueToAvoidAction(DiscardCardWhateverToPlayAction action);
        ClueType CreateClueToAvoidAction(DiscardUniqueCardAction action);
    }
    
    public abstract class PlayerAction
    {
        protected IList<Type> ActionsToAvoid = new List<Type> { typeof(BlowCardAction), typeof(DiscardUniqueCardAction) };

        public abstract bool PlayCard { get; }
        public abstract bool AddsClueAfter { get; }
        public abstract bool RequiresImmediateClue { get; }

        public abstract bool Discard { get; }

        public abstract ClueAndAction CreateClueToAvoid(IBoardContext boardContext, IPlayerContext playerContext, IPlayCardStrategy playCardStrategy, IDiscardStrategy discardStrategy);
    }

    public class PlayCardAction : PlayerAction
    {
        public IList<Card> CardsToPlay;

        public override bool PlayCard => true;

        public override bool AddsClueAfter => false;
        public override bool RequiresImmediateClue => false;
        public override bool Discard => false;

        public override ClueAndAction CreateClueToAvoid(
            IBoardContext boardContext, 
            IPlayerContext playerContext, 
            IPlayCardStrategy playCardStrategy,
            IDiscardStrategy discardStrategy)
        {
            return null;
        }
    }

    public class PlayCardWithRankFiveAction : PlayCardAction
    {
        public override bool AddsClueAfter => true;
    }

    public class BlowCardAction : PlayerAction
    {
        public IList<Card> CardsToBlow;

        public override bool PlayCard => false;

        public override bool AddsClueAfter => false;

        public override bool RequiresImmediateClue => true;
        public override bool Discard => false;

        public override ClueAndAction CreateClueToAvoid(
            IBoardContext boardContext, 
            IPlayerContext playerContext, 
            IPlayCardStrategy playCardStrategy,
            IDiscardStrategy discardStrategy)
        {
            foreach (var card in CardsToBlow)
            {
                var cardInHand = playerContext.Hand.First(c => c.Card == card);
                foreach (var clue in ClueDetailInfo.CreateClues(cardInHand, playerContext.Player))
                {
                    // применим подсказку и посмотрим, будет ли всё хорошо
                    playerContext.PossibleClue = clue;

                    // контекст должен чуток измениться...
                    var playerPredictor = new PlayerActionPredictor(boardContext, playerContext);
                    var newAction = playerPredictor.Predict(playCardStrategy, discardStrategy);


                    //1. Ходит
                    // a) ходит 5                   => можно из цикла выходить
                    // б) ходит не 5                => перейти к следующему игроку
                    // в) взрыв                     => рассмотреть следующий вариант
                    //2. Сбрасывает
                    //  а) ненужную карту           => можно из цикла выходить
                    //  б) нужную карту
                    //     *) уникальную карту      => рассмотреть другой вариант
                    //     *) неуникальную карту    => рассмотреть другой вариант

                    // если новое действие не требует вмешательства, то считаем подсказку приемлемой
                    if (!newAction.RequiresImmediateClue) return new ClueAndAction
                    {
                        Clue = clue,
                        Action = newAction,
                    };
                }
            }

            return null;
        }
    }

    public abstract class DiscardAction : PlayerAction
    {
        public IList<Card> CardsToDiscard;

        public override bool PlayCard => false;

        public override bool AddsClueAfter => true;
        public override bool Discard => true;

        public override ClueAndAction CreateClueToAvoid(
            IBoardContext boardContext, 
            IPlayerContext playerContext, 
            IPlayCardStrategy playCardStrategy,
            IDiscardStrategy discardStrategy)
        {
            return null;
        }
    }

    public class DiscardCardWhateverToPlayAction : DiscardAction
    {
        public override bool RequiresImmediateClue => true;

        public override ClueAndAction CreateClueToAvoid(
            IBoardContext boardContext, 
            IPlayerContext playerContext,
            IPlayCardStrategy playCardStrategy,
            IDiscardStrategy discardStrategy)
        {
            foreach (var card in CardsToDiscard)
            {
                var cardInHand = playerContext.Hand.First(c => c.Card == card);
                foreach (var clue in ClueDetailInfo.CreateClues(cardInHand, playerContext.Player))
                {
                    // применим подсказку и посмотрим, будет ли всё хорошо
                    playerContext.PossibleClue = clue;

                    // контекст должен чуток измениться...
                    var playerPredictor = new PlayerActionPredictor(boardContext, playerContext);
                    var newAction = playerPredictor.Predict(playCardStrategy, discardStrategy);


                    //1. Ходит
                    // a) ходит 5                   => можно из цикла выходить
                    // б) ходит не 5                => перейти к следующему игроку
                    // в) взрыв                     => рассмотреть следующий вариант
                    //2. Сбрасывает
                    //  а) ненужную карту           => можно из цикла выходить
                    //  б) нужную карту
                    //     *) уникальную карту      => рассмотреть другой вариант
                    //     *) неуникальную карту    => рассмотреть другой вариант

                    // если новое действие не требует вмешательства, то считаем подсказку приемлемой
                    if (!newAction.RequiresImmediateClue) return new ClueAndAction
                    {
                        Clue = clue,
                        Action = newAction,
                    };
                }
            }

            return null;
        }
    }

    public class DiscardUniqueCardAction : DiscardAction
    {
        public override bool PlayCard => false;

        public override bool AddsClueAfter => false;

        public override bool RequiresImmediateClue => true;

        public override ClueAndAction CreateClueToAvoid(
            IBoardContext boardContext, 
            IPlayerContext playerContext, 
            IPlayCardStrategy playCardStrategy,
            IDiscardStrategy discardStrategy)
        {
            // 1. Поищем подсказки, после которых игрок точно сходит


            var possibleCluesAndActions = new List<ClueAndAction>();

            foreach (var card in CardsToDiscard)
            {
                var cardInHand = playerContext.Hand.First(c => c.Card == card);
                foreach (var clue in ClueDetailInfo.CreateClues(cardInHand, playerContext.Player))
                {
                    // применим подсказку и посмотрим, будет ли всё хорошо
                    playerContext.PossibleClue = clue;

                    // контекст должен чуток измениться...
                    var playerPredictor = new PlayerActionPredictor(boardContext, playerContext);
                    var newAction = playerPredictor.Predict(playCardStrategy, discardStrategy);

                    // если новое действие не ведёт к взрыву или сбросу нужной карты, то считаем подсказку приемлемой
                    if (ActionsToAvoid.All(a => a != newAction.GetType()))
                    {
                        var clueAndAction = new ClueAndAction{Clue = clue, Action = newAction};
                        possibleCluesAndActions.Add(clueAndAction);
                    }
                }
            }

            // на этой стадии отобраны возможны такие последствия:
            // игрок ходит 5,
            // игрок сбрасывает ненужную карту,
            // игрок ходит не 5,
            // игрок сбрасывает нужную некритичную карту
            // выводим именно в таком порядке.

            var result = 
                possibleCluesAndActions.FirstOrDefault(ca => ca.Action is PlayCardWithRankFiveAction);

            if (result != null) return result;

            result =
                possibleCluesAndActions.FirstOrDefault(ca => ca.Action is DiscardNoNeedCard);

            if (result != null) return result;

            result =
                possibleCluesAndActions.FirstOrDefault(ca => ca.Action is PlayCardAction);
            if (result != null) return result;

            result = possibleCluesAndActions.FirstOrDefault(ca => ca.Action is DiscardCardWhateverToPlayAction);

            return result;
        }
    }

    public class DiscardNoNeedCard : DiscardAction
    {
        public override bool RequiresImmediateClue => false;
    }
}
