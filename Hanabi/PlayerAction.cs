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


    public interface IHanabiInfoLog
    {
        void Log(string clueGiver, string player);
    }

    [Flags]
    public enum OutcomeFlags
    {
        Play = 0x01,
        PlayFiveRankCard = 0x02,
        Blow = 0x04,
        DiscardNoNeedCard = 0x08,
        DiscardWhateverToPlayCard = 0x10,
        DiscardUniqueCard = 0x20,
    }

    public abstract class PlayerAction : IHanabiInfoLog
    {
        public abstract OutcomeFlags Outcome { get; }

        public bool IsActionToAvoid
        {
            get
            {
                var actionsToAvoid = OutcomeFlags.DiscardUniqueCard | OutcomeFlags.Blow;
                return (Outcome & actionsToAvoid) > 0;
            }
        }

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
                    .Select(cih => ClueDetailInfo.CreateClues(cih, playerContext))
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

        protected virtual ClueAndAction ChooseClue(IPlayerContext playerContext,
            IEnumerable<ClueAndAction> possibleCluesAndActions)
        {
            if (playerContext == null) throw new ArgumentNullException(nameof(playerContext));
            if (possibleCluesAndActions == null) throw new ArgumentNullException(nameof(possibleCluesAndActions));

            if (possibleCluesAndActions.Count() <= 1) return possibleCluesAndActions.FirstOrDefault();

            int max = 0;
            ClueAndAction result = null;
            foreach (var clueAndAction in possibleCluesAndActions)
            {
                int affectedCards = GetAffectedCardsCount(playerContext, clueAndAction.Clue);

                if (affectedCards > max)
                {
                    max = affectedCards;
                    result = clueAndAction;
                }
            }

            return result;

            int GetAffectedCardsCount(IPlayerContext context, ClueType clue)
            {
                // возможно, стоит исключить карты, о которых игрок и так знает...

                return context.Hand
                    .Select(cardInHand => cardInHand.Card)
                    .Select(card => new ClueAndCardMatcher(card))
                    .Count(clueAndCardMatcher => clue.Accept(clueAndCardMatcher));
            }
        }


        public abstract void Log(string clueGiver, string player);
    }

    abstract class PlayerActionContract : PlayerAction
    {
        protected override bool IsNewActionCorrect(PlayerAction action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            throw new NotSupportedException();
        }

        protected override ClueAndAction ChooseClue(IPlayerContext playerContext, IEnumerable<ClueAndAction> possibleCluesAndActions)
        {
            throw new NotSupportedException();
        }
    }

    public class PlayCardAction : PlayerAction
    {
        public override OutcomeFlags Outcome => OutcomeFlags.Play;

        protected override bool IsNewActionCorrect(PlayerAction action)
        {
            return false;
        }

        protected override ClueAndAction ChooseClue(IPlayerContext playerContext, IEnumerable<ClueAndAction> possibleCluesAndActions)
        {
            return null;
        }

        public override void Log(string clueGiver, string player)
        {
            string str = $"Player {clueGiver} thinks that player {player} plays ";

            foreach (var cardToPlay in this.Cards)
            {
                str += $"{{{cardToPlay}}} ";
            }
            Logger.Log.Info(str);
        }
    }

    public class PlayCardWithRankFiveAction : PlayCardAction
    {
        public override OutcomeFlags Outcome => OutcomeFlags.Play | OutcomeFlags.PlayFiveRankCard;

        public override void Log(string clueGiver, string player)
        {
            Logger.Log.Info($"Player {clueGiver} thinks that player {player} plays five-ranked card");
        }
    }

    public class BlowCardAction : PlayerAction
    {
        public override OutcomeFlags Outcome => OutcomeFlags.Blow;

        protected override bool IsNewActionCorrect(PlayerAction action)
        {
            // игрок собирался взрывать...
            // что угодно, только не взрыв и не сброс нужной карты!
            return !action.IsActionToAvoid;
        }

        public override void Log(string clueGiver, string player)
        {
            string info = $"Player {clueGiver} thinks that player {player} blows ";
            foreach (var card in this.Cards)
            {
                info += $" {{{card}}}";
            }
            Logger.Log.Info(info);
        }
    }

    public abstract class DiscardAction : PlayerAction
    {
    }

    public class DiscardCardWhateverToPlayAction : DiscardAction
    {
        public override OutcomeFlags Outcome => OutcomeFlags.DiscardWhateverToPlayCard;

        protected override bool IsNewActionCorrect(PlayerAction action)
        {
            // принимается:
            // ход картой и сброс ненужной карты

            var expectedOutcome = OutcomeFlags.Play | OutcomeFlags.DiscardNoNeedCard;

            return (action.Outcome & expectedOutcome) > 0;
        }

        public override void Log(string clueGiver, string player)
        {
            string info = $"Player {clueGiver} thinks that player {player} discards ";

            foreach (var card in Cards)
            {
                info += $"{{{card}}} ";
            }

            Logger.Log.Info(info);
        }
    }

    public class DiscardUniqueCardAction : DiscardAction
    {
        public override OutcomeFlags Outcome => OutcomeFlags.DiscardUniqueCard;

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

        public override void Log(string clueGiver, string player)
        {
            string info = $"Player {clueGiver} thinks that player {player} discards uniqueCard(s)";

            foreach (var card in this.Cards)
            {
                info += $" {{{card}}}";
            }

            Logger.Log.Info(info);
        }
    }

    public class DiscardNoNeedCard : DiscardAction
    {
        public override OutcomeFlags Outcome => OutcomeFlags.DiscardNoNeedCard;

        protected override bool IsNewActionCorrect(PlayerAction action)
        {
            return (action.Outcome & OutcomeFlags.Play) > 0;
        }

        public override void Log(string clueGiver, string player)
        {
            string info = $"Player {clueGiver} thinks that player {player} discards no need card(s)";

            foreach (var card in this.Cards)
            {
                info += $" {{{card}}}";
            }

            Logger.Log.Info(info);
        }
    }
}