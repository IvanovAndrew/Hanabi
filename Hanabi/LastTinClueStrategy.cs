using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Hanabi
{
    public class HardSolution
    {
        public Player PlayerToClue { get; set; }
        public Clue Clue { get; set; }
        public ClueSituation Situation;
        public IReadOnlyList<CardInHand> CardsToClue;
    }

    public enum ClueSituation
    {
        NoNeedClue,
        ClueExists,
        ClueDoesntExist,
    }

    public class LastTinClueStrategy : IClueStrategy
    {
        private readonly Player _clueGiver;
        private readonly Board _board;
        private readonly IGameProvider _gameProvider;
        private readonly PilesAnalyzer _pilesAnalyzer;

        public LastTinClueStrategy(Player clueGiver, Board board, IGameProvider gameProvider)
        {
            Contract.Requires<ArgumentNullException>(clueGiver != null);
            Contract.Requires<ArgumentNullException>(board != null);
            Contract.Requires<ArgumentNullException>(gameProvider != null);

            _clueGiver = clueGiver;
            _board = board;
            _gameProvider = gameProvider;
            _pilesAnalyzer = new PilesAnalyzer(_gameProvider);
        }


        public HardSolution FindClueCandidate(IReadOnlyList<Player> players)
        {
            // player1 -> player2 -> player3 -> player4
            // подсказку даёт player1.
            // Пока не будет сброса или не будет сыграна 5, то
            //      прогнозируем ход следующего игрока. 
            //      игрок сбрасывает некритичную карту --> выход из цикла [если была подсказка в уме, то дать её]
            //      игрок ходит пятёркой --> выход из цикла [если была в уме подсказка, то дать её]
            //      
            //      игрок ходит не пятёркой, карта ложится в фейерверк --> оценить действия следующего игрока
            //      игрок ходит не пятёркой, что приводит к взрыву     --> дать подсказку, оценить действия следующего игрока
            //              
            //      игрок сбрасывает критичную карту, нет подсказки --> безвыходная ситуация. Надо сбрасывать самому
            //      игрок сбрасывает критичную карту, есть подсказка --> дать подсказку, оценить действия следующего игрока
            HardSolution solution = null;
            HardSolution optionalSolution = null;

            var fireworkPile = _board.FireworkPile.Clone();
            var discardPile = _board.DiscardPile.Clone();

            foreach (var playerToClue in players)
            {
                IList<Card> otherPlayerCards = GetOtherPlayerCards(players, playerToClue);

                var playerHand = playerToClue.ShowCards(_clueGiver);

                BoardContext boardContext = BoardContext.Create(fireworkPile, discardPile, _pilesAnalyzer, otherPlayerCards);
                PlayerContext playerContext = new PlayerContext(playerToClue, playerHand);

                // надо оценить, что будет делать другой игрок.
                IPlayCardStrategy playCardStrategy = PlayStrategyFabric.Create(_gameProvider, playerContext);
                IDiscardStrategy discardStrategy = DiscardStrategyFabric.Create(_gameProvider, playerContext);

                var playerActionPredictor = new PlayerActionPredictor(boardContext, playerContext);

                var action = playerActionPredictor.Predict(playCardStrategy, discardStrategy);

                // игрок играет пятёрку или сбрасывает ненужную карту: 
                // подсказываем, если хотели подсказать. Иначе расслабляемся.
                if (action.AddsClueAfter && action.PlayCard || action is DiscardNoNeedCard)
                {
                    break;
                }

                // игрок собирается ходить. Точно вмешиваться не надо. 
                // Переходим к следующему игроку
                if (action.PlayCard)
                {
                    fireworkPile.AddCard((action as PlayCardAction).CardsToPlay.First());
                    continue;
                }

                // игрок хочет сбросить карту, которую когда-нибудь можно будет сыграть.
                // если получится, то воспрепятствуем
                if (action is DiscardCardWhateverToPlayAction)
                {
                    // подберём потенциальную подсказку

                    if (optionalSolution == null)
                    {
                        var clueAndAction =
                            action.CreateClueToAvoid(boardContext, playerContext, playCardStrategy, discardStrategy);

                        if (clueAndAction != null)
                        {
                            optionalSolution = new HardSolution
                            {
                                Clue = clueAndAction.Clue,
                                Situation = ClueSituation.ClueExists,
                                PlayerToClue = playerToClue,
                                CardsToClue = ClueDetailInfo.GetCardsToClue(playerHand, clueAndAction.Clue).ToList(),
                            };
                        }
                    }
                    
                    continue;
                }

                // игрок хочет сходить так, что будет взрыв, или сбросить уникальную карту
                // срочно вмешиваемся. 
                if (action is BlowCardAction || action is DiscardUniqueCardAction)
                {
                    // кому-то ранее потребовалась срочная подсказка. Ситуация безвыходная. Придётся сбрасывать самому.
                    if (solution != null)
                    {
                        solution = new HardSolution {Situation = ClueSituation.ClueDoesntExist,};
                        break;
                    }

                    var clueAndAction = action.CreateClueToAvoid(boardContext, playerContext, playCardStrategy, discardStrategy);

                    if (clueAndAction == null)
                    {
                        // с помощью подсказки никак не избежать...
                        solution = new HardSolution {Situation = ClueSituation.ClueDoesntExist};
                        break;
                    }

                    if (clueAndAction.Action is PlayCardWithRankFiveAction ||
                        clueAndAction.Action is DiscardNoNeedCard)
                    {
                        solution = new HardSolution
                        {
                            Situation = ClueSituation.ClueExists,
                            Clue = clueAndAction.Clue,
                            PlayerToClue = playerToClue,
                            CardsToClue = ClueDetailInfo.GetCardsToClue(playerHand, clueAndAction.Clue).ToList(),
                        };
                    }
                    else
                    {
                        // непонятно, что тут делать...
                    }
                }
            }


            if (solution != null) return solution;
            if (optionalSolution != null) return optionalSolution;

            // выскочили из цикла, но не придумали подсказку... 
            // Значит, можно расслабиться...
            return new HardSolution {Situation = ClueSituation.NoNeedClue};
        }

        private IList<Card> GetOtherPlayerCards(IReadOnlyList<Player> players, Player playerToClue)
        {
            var cards = new List<Card>();

            // соберём карты других игроков
            foreach (var otherPlayer in players)
            {
                if (playerToClue == otherPlayer) continue;

                cards.AddRange(
                    otherPlayer
                        .ShowCards(_clueGiver)
                        .Select(cardInHand => cardInHand.Card)
                );

                cards.AddRange(_clueGiver.GetKnownCards());
            }

            return cards;
        }
    }
}
