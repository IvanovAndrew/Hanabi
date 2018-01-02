using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Hanabi
{
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


        public IClueSituationStrategy FindClueCandidate(IReadOnlyList<Player> players)
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
            IClueSituationStrategy solution = null;
            IClueSituationStrategy optionalSolution = null;
            bool willDiscard = false;
            Player ignoreUntil = null;

            LogUniqueCards();

            IBoardContext boardContext = BoardContext.Create(_board.FireworkPile, _board.DiscardPile, _pilesAnalyzer, new Card[0]);

            foreach (var playerToClue in players)
            {
                IList<Card> otherPlayerCards = GetOtherPlayerCards(players, playerToClue);

                var playerHand = playerToClue.ShowCards(_clueGiver);

                boardContext = boardContext.ChangeContext(otherPlayerCards);
                IPlayerContext playerContext = new PlayerContext(playerToClue, playerHand);

                // надо оценить, что будет делать другой игрок.
                IPlayCardStrategy playCardStrategy = PlayStrategyFabric.Create(_gameProvider, playerContext);
                IDiscardStrategy discardStrategy = DiscardStrategyFabric.Create(_gameProvider, playerContext);

                var playerActionPredictor = new PlayerActionPredictor(boardContext, playerContext);

                var action = playerActionPredictor.Predict(playCardStrategy, discardStrategy);

                // игрок играет пятёрку или сбрасывает ненужную карту: 
                // подсказываем, если хотели подсказать. Иначе расслабляемся.
                if (action.AddsClueAfter && action.PlayCard || action is DiscardNoNeedCard)
                {
                    willDiscard |= true;
                    ignoreUntil = playerToClue;

                    LogThoughtActionAddingClue(playerToClue);
                    break;
                }

                // игрок собирается ходить. Точно вмешиваться не надо. 
                // Переходим к следующему игроку
                if (action.PlayCard)
                {
                    var playCardAction = action as PlayCardAction;
                    var card = playCardAction.CardsToPlay.First();
                    boardContext.AddToFirework(card);

                    LogThoughtPlayAction(playerToClue, playCardAction.CardsToPlay);
                    continue;
                }

                // игрок хочет сбросить карту, которую когда-нибудь можно будет сыграть.
                // если получится, то воспрепятствуем
                if (action is DiscardCardWhateverToPlayAction discardAction)
                {
                    LogThoughtDiscardWhateverToPlayCards(playerToClue, discardAction.CardsToDiscard);

                    // подберём потенциальную подсказку
                    if (optionalSolution == null)
                    {
                        var clueAndAction =
                            action.CreateClueToAvoid(boardContext, playerContext);

                        if (clueAndAction != null)
                        {
                            if (clueAndAction.Action.PlayCard)
                            {
                                Logger.Log.Info($"There is a clue {clueAndAction.Clue} after that will be play card action");
                            }
                            willDiscard |= clueAndAction.Action.Discard;
                            optionalSolution = new OnlyClueExistsSituation(playerContext, clueAndAction.Clue);
                        }
                        else
                        {
                            willDiscard = true;
                        }
                    }
                    else
                    {
                        willDiscard = true;
                    }

                    continue;
                }

                // игрок хочет сходить так, что будет взрыв, или сбросить уникальную карту
                // срочно вмешиваемся. 
                if (action is BlowCardAction || action is DiscardUniqueCardAction)
                {
                    if (action is BlowCardAction b)
                    {
                        LogThoughtAboutBlowAction(playerToClue, b.CardsToBlow);
                        
                    }
                    else if (action is DiscardUniqueCardAction d)
                    {
                        LogThoughtAboutDiscardUniqueCardsAction(playerToClue, d.CardsToDiscard);
                    }
                    

                    // кому-то ранее потребовалась срочная подсказка. Ситуация безвыходная. Придётся сбрасывать самому.
                    if (solution != null)
                    {
                        if (!willDiscard)
                        {
                            solution = new ClueNotExistsSituation();
                            break;
                        }
                    }

                    ClueAndAction clueAndAction = action.CreateClueToAvoid(boardContext, playerContext);

                    if (clueAndAction == null)
                    {
                        if (!willDiscard)
                        {
                            // даже с помощью подсказки беды никак не избежать...
                            solution = new ClueNotExistsSituation();
                            break;
                        }
                        else
                        {
                            IEnumerable<Card> cards = new List<Card>();
                            if (action is BlowCardAction b1)
                            {
                                cards = b1.CardsToBlow;
                                LogThoughtAboutBlowAction(playerToClue, b1.CardsToBlow);
                            }
                            else if (action is DiscardUniqueCardAction d1)
                            {
                                cards = d1.CardsToDiscard;
                                LogThoughtAboutDiscardUniqueCardsAction(
                                    playerToClue, 
                                    d1.CardsToDiscard);
                            }
                            
                            // придумать подсказку про запас

                            var possibleClue = CreateClue(playerContext, cards);
                            if (possibleClue != null)
                            {
                                solution = new OnlyClueExistsSituation(playerContext, possibleClue);
                                break;
                            }
                            continue;
                        }
                    }

                    solution = new OnlyClueExistsSituation(playerContext, clueAndAction.Clue);

                    if (clueAndAction.Action is PlayCardWithRankFiveAction ||
                        clueAndAction.Action is DiscardNoNeedCard ||
                        clueAndAction.Action is DiscardCardWhateverToPlayAction)
                    {
                        break;
                    }
                    else
                    {
                        
                    }
                }
            }


            Logger.Log.Info("");
            if (solution != null) return solution;
            if (optionalSolution != null) return optionalSolution;

            // выскочили из цикла, но не придумали подсказку... 
            // Значит, можно расслабиться...

            var playersCircle = players;
            if (ignoreUntil != null)
            {
                playersCircle =
                    players.SkipWhile(p => p != ignoreUntil)
                        .Skip(1)
                        .Concat(players.TakeWhile(p => p != ignoreUntil))
                        .ToList()
                        .AsReadOnly();
            }

            return 
                new NoCriticalSituation(_clueGiver, playersCircle, boardContext);
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
            }

            cards.AddRange(_clueGiver.GetKnownCards());

            return cards;
        }

        private ClueType CreateClue(IPlayerContext playerContext, IEnumerable<Card> cards)
        {
            Contract.Requires<ArgumentNullException>(playerContext != null);
            Contract.Requires(cards.Any());
            Contract.Ensures(Contract.Result<ClueType>() == null || Contract.Result<ClueType>().IsStraightClue);

            foreach (var cardInHand in playerContext.Hand.Where(c => cards.Contains(c.Card)))
            {
                var clues = ClueDetailInfo.CreateClues(cardInHand, playerContext.Player);
                if (clues.Any()) return clues.First();
            }
            return null;
        }

        private void LogUniqueCards()
        {
            var str = "Unique cards: ";
            foreach (var card in _pilesAnalyzer
                                    .GetUniqueCards(_board.FireworkPile, _board.DiscardPile)
                                    .OrderBy(c => c.Rank))
            {
                str += $"{card}; ";
            }
            Logger.Log.Info(str);
        }

        private void LogThoughtPlayAction(Player player, IEnumerable<Card> cards)
        {
            string str = $"Player {_clueGiver.Name} thinks " +
                         $"that player {player.Name} plays ";
                            
            foreach (var cardToPlay in cards)
            {
                str += $"{cardToPlay} ";
            }
            Logger.Log.Info(str);
        }

        private void LogThoughtDiscardWhateverToPlayCards(Player player, IEnumerable<Card> cards)
        {
            string info = $"Player {_clueGiver.Name} thinks " +
                            $"that player {player.Name} " +
                            "discards ";

            foreach (var card in cards)
            {
                info += $"{card} ";
            }

            Logger.Log.Info(info);
        }

        private void LogThoughtActionAddingClue(Player player)
        {
            Logger.Log.Info(
                $"Player {_clueGiver.Name} thinks " +
                $"that player {player.Name} " +
                "discards no need cards or plays five-ranked card");
        }

        private void LogThoughtAboutBlowAction(Player player, IEnumerable<Card> cards)
        {
            string info = $"Player {_clueGiver.Name} thinks " +
                          $"that player {player.Name} blows ";
            foreach (var card in cards)
            {
                info += $" {card}";
            }
            Logger.Log.Info(info);
        }

        private void LogThoughtAboutDiscardUniqueCardsAction(Player player, IEnumerable<Card> cards)
        {
            string info = $"Player {_clueGiver.Name} thinks " +
                          $"that player {player.Name} discards uniqueCard(s) ";

            foreach (var card in cards)
            {
                info += $" {card}";
            }
                            
            Logger.Log.Info(info);
        }
    }
}
