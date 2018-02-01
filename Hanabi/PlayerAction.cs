using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Hanabi
{
    public class ClueAndAction
    {
        public ClueType Clue { get; set; }
        public PlayerAction Action { get; set; }
    }

    [ContractClass(typeof(PlayerActionContract))]
    public abstract class PlayerAction
    {
        public abstract bool PlayCard { get; }
        public abstract bool AddsClueAfter { get; }
        public abstract bool DiscardWhateverToPlayCard { get; }
        public abstract bool IsActionToAvoid { get; }

        public abstract bool Discard { get; }

        public IEnumerable<Card> Cards { get; set; }

        public ClueAndAction CreateClueToAvoid(IBoardContext boardContext, IPlayerContext playerContext)
        {
            // 1. Поищем подсказки, после которых игрок точно сходит
            var clueToPlayFinder = new ClueToPlayFinder(boardContext, playerContext);
            var actionToPlay = clueToPlayFinder.Find();

            if (actionToPlay != null) return actionToPlay;

            var variants = new List<ClueAndAction>();

            var clues =
                playerContext.Hand
                    .Where(cih => Cards.Contains(cih.Card))
                    .Select(cih => ClueDetailInfo.CreateClues(cih, playerContext.Player))
                    .Aggregate((acc, c) => acc.Concat(c).ToList())
                    .Distinct();

            foreach (var clue in clues)
            {
                // применим подсказку и посмотрим, будет ли всё хорошо
                playerContext.PossibleClue = clue;

                // контекст должен чуток измениться...
                var playCardStrategy = PlayStrategyFabric.Create(playerContext.Player.GameProvider, playerContext);
                var discardStrategy =
                    DiscardStrategyFabric.Create(playerContext.Player.GameProvider, playerContext);

                var playerPredictor = new PlayerActionPredictor(boardContext, playerContext);
                var newAction = playerPredictor.Predict(playCardStrategy, discardStrategy);


                if (IsNewActionCorrect(newAction))
                    variants.Add(new ClueAndAction {Clue = clue, Action = newAction});
            }

            return ChooseClue(playerContext, variants);
        }

        protected abstract bool IsNewActionCorrect(PlayerAction action);

        protected abstract ClueAndAction ChooseClue(IPlayerContext playerContext, IEnumerable<ClueAndAction> possibleCluesAndActions);
    }

    [ContractClassFor(typeof(PlayerAction))]
    abstract class PlayerActionContract : PlayerAction
    {
        protected override bool IsNewActionCorrect(PlayerAction action)
        {
            Contract.Requires<ArgumentNullException>(action != null);

            throw new NotSupportedException();
        }

        protected override ClueAndAction ChooseClue(IPlayerContext playerContext, IEnumerable<ClueAndAction> possibleCluesAndActions)
        {
            Contract.Requires<ArgumentNullException>(playerContext != null);
            Contract.Requires<ArgumentNullException>(possibleCluesAndActions != null);

            throw new NotSupportedException();
        }
    }

    public class PlayCardAction : PlayerAction
    {
        public override bool PlayCard => true;

        public override bool AddsClueAfter => false;
        public override bool DiscardWhateverToPlayCard => false;
        public override bool IsActionToAvoid => false;
        public override bool Discard => false;

        protected override bool IsNewActionCorrect(PlayerAction action)
        {
            return false;
        }

        protected override ClueAndAction ChooseClue(IPlayerContext playerContext, IEnumerable<ClueAndAction> possibleCluesAndActions)
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
        public override bool PlayCard => false;

        public override bool AddsClueAfter => false;

        public override bool DiscardWhateverToPlayCard => true;
        public override bool IsActionToAvoid => true;
        public override bool Discard => false;

        protected override bool IsNewActionCorrect(PlayerAction action)
        {
            // игрок собирался взрывать...
            // что угодно, только не взрыв и не сброс нужной карты!
            return !action.IsActionToAvoid;
        }

        protected override ClueAndAction ChooseClue(IPlayerContext playerContext, IEnumerable<ClueAndAction> possibleCluesAndActions)
        {
            // TODO придумать выход из ситуации
            return possibleCluesAndActions.FirstOrDefault();
        }
    }

    public abstract class DiscardAction : PlayerAction
    {
        public override bool PlayCard => false;

        public override bool AddsClueAfter => true;
        public override bool Discard => true;
    }

    public class DiscardCardWhateverToPlayAction : DiscardAction
    {
        public override bool DiscardWhateverToPlayCard => true;
        public override bool IsActionToAvoid => false;

        protected override bool IsNewActionCorrect(PlayerAction action)
        {
            // не будем усугублять и менять шило на мыло
            return !action.IsActionToAvoid && !action.DiscardWhateverToPlayCard;
        }

        protected override ClueAndAction ChooseClue(IPlayerContext playerContext, IEnumerable<ClueAndAction> possibleCluesAndActions)
        {
            return possibleCluesAndActions.FirstOrDefault();
        }
    }

    public class DiscardUniqueCardAction : DiscardAction
    {
        public override bool PlayCard => false;
        public override bool DiscardWhateverToPlayCard => true;

        public override bool AddsClueAfter => false;
        public override bool IsActionToAvoid => true;

        protected override bool IsNewActionCorrect(PlayerAction action)
        {
            return !action.IsActionToAvoid;
        }

        protected override ClueAndAction ChooseClue(IPlayerContext playerContext, IEnumerable<ClueAndAction> possibleCluesAndActions)
        {
            // на этой стадии возможны такие последствия:
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
        public override bool DiscardWhateverToPlayCard => false;
        public override bool IsActionToAvoid => false;

        protected override bool IsNewActionCorrect(PlayerAction action)
        {
            return action.PlayCard;
        }

        protected override ClueAndAction ChooseClue(IPlayerContext playerContext, IEnumerable<ClueAndAction> possibleCluesAndActions)
        {
            return possibleCluesAndActions.FirstOrDefault();
        }
    }
}