using System;
using System.Collections.Generic;
using System.Linq;

namespace Hanabi
{

    // состояния:
    // никаких подсказок не требуется
    // уже есть подсказка, чтобы сохранить карту, которую можно выбросить
    // уже есть нужная подсказка
    // есть действие, гарантированно добывающее подсказку (сброс или ход пятёркой)
    // безвыходная ситуация

    // действия: 
    // ход картой
    // ход пятёркой
    // взрыв
    // сброс ненужной карты
    // сброс некритичной карты
    // сброс критичной карты

    // переходы :
    // [_, ход пятёркой] -> "плюс подсказка"
    // [_, сброс ненужной карты] -> "плюс подсказка"
    // [A, ход картой] -> A

    // ["никаких подсказок не требуется", взрыв] -> "требуется подсказка"
    // ["никаких подсказок не требуется", сброс некритичной карты] -> "возможна подсказка" или "плюс подсказка"
    // ["никаких подсказок не требуется", сброс критичной карты] -> "требуется подсказка"

    // ["возможна подсказка", взрыв] -> "требуется подсказка"
    // ["возможна подсказка", сброс некритичной карты] -> "возможна подсказка"
    // ["возможна подсказка", сброс критичной карты] -> "требуется подсказка"

    // ["требуется подсказка", взрыв] -> "безвыходная ситуация"
    // ["требуется подсказка", сброс некритичной карты] -> "требуется подсказка"
    // ["требуется подсказка", сброс критичной карты] -> "безвыходная ситуация"

    // подумать над этими переходами
    // ["плюс подсказка", взрыв] -> "требуется подсказка"
    // ["плюс подсказка", сброс некритичной карты] -> "возможна подсказка"
    // ["плюс подсказка", сброс критичной карты] -> "безвыходная ситуация"
    public abstract class State
    {
        public IClueSituationStrategy Solution { get; protected set; }
        public IClueSituationStrategy OptionalSolution { get; protected set; }

        public Player IgnoreUntil { get; protected set; }

        public IBoardContext BoardContext { get; set; }

        public bool IsFinalState { get; protected set; }

        public abstract State Handle(PlayerAction action, IPlayerContext playerContext);

        protected State()
        {
            
        }

        protected State(IBoardContext boardContext)
        {
            BoardContext = boardContext;
        }

        protected State(State previous)
        {
            Solution = previous.Solution;
            OptionalSolution = previous.OptionalSolution;
            IgnoreUntil = previous.IgnoreUntil;
            BoardContext = previous.BoardContext;
        }

        protected ClueType CreateClue(IPlayerContext playerContext, IEnumerable<Card> cards)
        {
            if (playerContext == null) throw new ArgumentNullException(nameof(playerContext));
            if (cards == null) throw new ArgumentNullException(nameof(cards));

            foreach (var cardInHand in playerContext.Hand.Where(c => cards.Contains(c.Card)))
            {
                var clues = ClueDetailInfo.CreateClues(cardInHand, playerContext);
                if (clues.Any()) return clues.First();
            }
            return null;
        }

        protected static bool IsPlayFiveRankedAction(PlayerAction action)
        {
            return (action.Outcome & OutcomeFlags.PlayFiveRankCard) > 0;
        }

        protected static bool IsDiscardNoNeedCard(PlayerAction action)
        {
            return (action.Outcome & OutcomeFlags.DiscardNoNeedCard) > 0;
        }

        protected static bool IsPlayCardAction(PlayerAction action)
        {
            return (action.Outcome & OutcomeFlags.Play) > 0;
        }

        protected static bool IsWhateverToPlayCardAction(PlayerAction action)
        {
            return (action.Outcome & OutcomeFlags.DiscardWhateverToPlayCard) > 0;
        }
    }


    internal class NoCluesState : State
    {
        public NoCluesState(IBoardContext boardContext) : base(boardContext)
        {
            IsFinalState = false;
        }

        public override State Handle(PlayerAction action, IPlayerContext playerContext)
        {
            // [_, ход пятёркой] -> "конечное состояние без подсказки"
            
            if (IsPlayFiveRankedAction(action))
                return new FinalWithoutClueState(this, playerContext.Player);

            // [_, сброс ненужной карты] -> "конечное состояние без подсказки"
            if (IsDiscardNoNeedCard(action))
            {
                // пробуем найти альтернативу.
                var alternativeAction = action.CreateClueToAvoid(BoardContext, playerContext);
                var defaultState = new FinalWithoutClueState(this, playerContext.Player);

                if (alternativeAction != null)
                {
                    BoardContext.AddToFirework(action.Cards.First());
                    var situation = new OnlyClueExistsSituation(playerContext, alternativeAction.Clue);

                    var optionalState = new OptionalClueState(this, situation);

                    return new MixedState(optionalState, defaultState);
                }
                else
                {
                    return defaultState;
                }

            }

            // [A, ход картой] -> A
            if (IsPlayCardAction(action))
            {
                BoardContext.AddToFirework(action.Cards.First());
                return this;
            }

            // ["никаких подсказок не требуется", сброс некритичной карты] -> "возможна подсказка" или "плюс подсказка"
            if (IsWhateverToPlayCardAction(action))
            {
                var clueAndAction =
                    action.CreateClueToAvoid(BoardContext, playerContext);

                if (clueAndAction != null)
                {
                    // добавить в сброшенные карты карту, которая полетит?
                    return new OptionalClueState(this, new OnlyClueExistsSituation(playerContext, clueAndAction.Clue));
                }
                else
                {
                    return new FinalWithoutClueState(this, playerContext.Player);
                }
            }

            // ["никаких подсказок не требуется", взрыв] -> "требуется подсказка" или безвыходная ситуация
            // ["никаких подсказок не требуется", сброс критичной карты] -> "требуется подсказка" или безвыходная ситуация
            if (action.IsActionToAvoid)
            {
                ClueAndAction clueAndAction = action.CreateClueToAvoid(BoardContext, playerContext);

                if (clueAndAction == null) return new NoExitState(this);

                // TODO выяснить, что будет делать игрок после этой подсказки
                // Если действие порождает подсказку, то можно уходить в конечной состояние.
                var correctedAction = clueAndAction.Action;

                var solution = new OnlyClueExistsSituation(playerContext, clueAndAction.Clue);
                var requiredClueState = new RequiredClue(this, solution);
                if ((correctedAction.Outcome & OutcomeFlags.Play) > 0)
                {
                    return requiredClueState;
                }
                else if ((correctedAction.Outcome &
                          (OutcomeFlags.DiscardNoNeedCard | 
                           OutcomeFlags.DiscardWhateverToPlayCard |
                           OutcomeFlags.PlayFiveRankCard)) > 0)
                {
                    return new FinalWithClueState(requiredClueState);
                }
                throw new InvalidOperationException($"{action.GetType()}");
            }

            throw new InvalidOperationException($"Неизвестный тип {action.GetType()}");
        }
    }

    // [_, ход пятёркой] -> "конечное состояние. Подсказка есть."
    // [_, сброс ненужной карты] -> "конечное состояние. Подсказка есть."
    // [A, ход картой] -> A
    // ["возможна подсказка", взрыв] -> "требуется подсказка" или "безвыходная ситуация"
    // ["возможна подсказка", сброс некритичной карты] -> "возможна подсказка"
    // ["возможна подсказка", сброс критичной карты] -> "требуется подсказка" или "безвыходная ситуация"
    internal class OptionalClueState : State
    {
        public OptionalClueState(State state, IClueSituationStrategy solution) : base(state)
        {
            OptionalSolution = solution;
            IsFinalState = false;
        }

        public override State Handle(PlayerAction action, IPlayerContext playerContext)
        {
            // [_, ход пятёркой] -> "конечное состояние. Подсказка есть."
            
            if (IsPlayFiveRankedAction(action) || IsDiscardNoNeedCard(action))
                return new FinalWithClueState(this);

            // [A, ход картой] -> A
            // ["возможна подсказка", сброс некритичной карты] -> "возможна подсказка"
            if (IsPlayCardAction(action) || IsWhateverToPlayCardAction(action)) return this;

            // ["возможна подсказка", взрыв] -> "требуется подсказка" или "безвыходная ситуация"
            // ["возможна подсказка", сброс критичной карты] -> "требуется подсказка" или "безвыходная ситуация"
            if (action.IsActionToAvoid)
            {
                ClueAndAction clueAndAction = action.CreateClueToAvoid(BoardContext, playerContext);
                IClueSituationStrategy solution;

                if (clueAndAction == null)
                {
                    // придумаем подсказку про запас

                    var possibleClue = CreateClue(playerContext, action.Cards);
                    if (possibleClue != null)
                    {
                        solution = new OnlyClueExistsSituation(playerContext, possibleClue);
                        return new RequiredClue(this, solution);
                    }
                    else return this;
                }

                // TODO выяснить, что будет делать игрок после этой подсказки
                // Если действие порождает подсказку, то можно уходить в конечной состояние.

                solution = new OnlyClueExistsSituation(playerContext, clueAndAction.Clue);
                return new RequiredClue(this, solution);
            }

            throw new ArgumentException(action.ToString());
        }
    }

    // [_, ход пятёркой] -> "плюс подсказка"
    // [_, сброс ненужной карты] -> "плюс подсказка"
    // [A, ход картой] -> A
    // ["требуется подсказка", взрыв] -> "безвыходная ситуация"
    // ["требуется подсказка", сброс некритичной карты] -> "требуется подсказка"
    // ["требуется подсказка", сброс критичной карты] -> "безвыходная ситуация"
    internal class RequiredClue : State
    {
        public RequiredClue(State previous, IClueSituationStrategy solution) : base(previous)
        {
            Solution = solution ?? throw new ArgumentNullException(nameof(solution));
            IsFinalState = false;
        }

        public override State Handle(PlayerAction action, IPlayerContext playerContext)
        {
            if (IsPlayFiveRankedAction(action) || IsDiscardNoNeedCard(action))
            {
                return new FinalWithClueState(this);
            }
            
            // [A, ход картой] -> A
            if (IsPlayCardAction(action)) return this;

            // ["требуется подсказка", взрыв] -> "безвыходная ситуация"
            // ["требуется подсказка", сброс критичной карты] -> "безвыходная ситуация"
            if (action.IsActionToAvoid)
            {
                return new NoExitState(this);
            }

            // ["требуется подсказка", сброс некритичной карты] -> "конечное состояние. Подсказка есть"
            if (IsWhateverToPlayCardAction(action)) return new FinalWithClueState(this);

            throw new InvalidOperationException($"Неожиданное состояние {action.GetType()}");
        }
    }

    internal class FinalWithClueState : State
    {
        public FinalWithClueState(State previous) : base(previous)
        {
            IsFinalState = true;
        }

        public override State Handle(PlayerAction action, IPlayerContext playerContext)
        {
            return this;
        }
    }

    internal class FinalWithoutClueState : State
    {
        public FinalWithoutClueState(State previous, Player ignoreUntil) : base(previous)
        {
            IgnoreUntil = ignoreUntil;
            IsFinalState = true;
        }

        public override State Handle(PlayerAction action, IPlayerContext playerContext)
        {
            return this;
        }
    }

    internal class NoExitState : State
    {
        public NoExitState(State previous) : base(previous)
        {
            Solution = null;
            OptionalSolution = null;
            IsFinalState = true;
        }

        public override State Handle(PlayerAction action, IPlayerContext playerContext)
        {
            return this;
        }
    }

    internal class MixedState : State
    {
        private State _preferredState;

        private readonly State _defaultState;

        public MixedState(State preferredState, State defaultState) : base(preferredState)
        {
            _preferredState = preferredState;
            _defaultState = defaultState;
        }

        public override State Handle(PlayerAction action, IPlayerContext playerContext)
        {
            _preferredState.BoardContext = this.BoardContext;

            _preferredState = _preferredState.Handle(action, playerContext);
            BoardContext = _preferredState.BoardContext;

            bool isExitState = false;
            if (_preferredState.IsFinalState)
            {
                isExitState = 
                    _preferredState.Solution == null && 
                    _preferredState.OptionalSolution == null;
            }

            return isExitState ? _defaultState : this;
        }
    }
}
