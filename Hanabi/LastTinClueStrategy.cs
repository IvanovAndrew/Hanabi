using System.Collections.Generic;
using System.Linq;

namespace Hanabi
{
    public class LastTinClueStrategy : IClueStrategy
    {
        private readonly Player _clueGiver;
        private readonly IBoardContext _boardContext;
        private readonly IGameProvider _gameProvider;
        private readonly IEnumerable<Card> _otherPlayersCards;

        public LastTinClueStrategy(Player clueGiver, IBoardContext boardContext, IGameProvider gameProvider,
            IEnumerable<Card> otherPlayerCards)
        {
            _clueGiver = clueGiver;
            _boardContext = boardContext;
            _gameProvider = gameProvider;
            _otherPlayersCards = otherPlayerCards;
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

            LogUniqueCards();

            var boardContext = _boardContext;
            //IBoardContext boardContext = BoardContext.Create(_board.FireworkPile, _board.DiscardPile, _pilesAnalyzer, new Card[0]);

            State state = new NoCluesState(boardContext);

            foreach (var playerToClue in players)
            {
                IEnumerable<Card> otherPlayerCards = GetPlayersCards(playerToClue);

                var playerHand = playerToClue.ShowCards(_clueGiver);

                boardContext = boardContext.ChangeContext(otherPlayerCards);
                IPlayerContext playerContext = new PlayerContext(playerToClue, playerHand);

                // надо оценить, что будет делать другой игрок.
                IPlayCardStrategy playCardStrategy = PlayStrategyFabric.Create(_gameProvider, playerContext);
                IDiscardStrategy discardStrategy = DiscardStrategyFabric.Create(_gameProvider, playerContext);

                var playerActionPredictor = new PlayerActionPredictor(boardContext, playerContext);

                var action = playerActionPredictor.Predict(playCardStrategy, discardStrategy);
                action.Log(_clueGiver.Name, playerToClue.Name);

                state.BoardContext = boardContext;
                state = state.Handle(action, playerContext);
                boardContext = state.BoardContext;

                if (state.IsFinalState) break;
            }


            Logger.Log.Info("");

            if (state.Solution != null) return state.Solution;
            if (state.OptionalSolution != null)
            {
                // TODO проверить, что подсказка ещё актуальна. 
                // Пример: после подсказки игрок А сходит жёлтой двойкой.
                // Но мы знаем, что по-любому игрок Б сходит жёлтой двойкой. 


                return state.OptionalSolution;
            }

            // выскочили из цикла, но не придумали подсказку... 
            // Значит, можно расслабиться...

            var playersCircle = players;
            if (state.IgnoreUntil != null)
            {
                playersCircle =
                    players.SkipWhile(p => p != state.IgnoreUntil)
                        .Skip(1)
                        .Concat(players.TakeWhile(p => p != state.IgnoreUntil))
                        .ToList()
                        .AsReadOnly();
            }
            // костыль на случай конца игры
            // осталось по ходу у двух игроков: А и Б. 
            // А попытался подсказать Б. Не вышло.
            // Без костыля playersCircle будет пустым.
            if (!playersCircle.Any()) playersCircle = players;

            return
                new NoCriticalSituation(_clueGiver, playersCircle, boardContext);
        }

        private IEnumerable<Card> GetPlayersCards(Player playerToExclude)
        {
            var result = _otherPlayersCards.ToList();

            var cardsToExclude = playerToExclude.ShowCards(_clueGiver).Select(cih => cih.Card);
            foreach (var card in cardsToExclude)
            {
                result.Remove(card);
            }

            return result;
        }

        //private ClueType CreateClue(IPlayerContext playerContext, IEnumerable<Card> cards)
        //{
        //    Contract.Requires<ArgumentNullException>(playerContext != null);
        //    Contract.Requires(cards.Any());

        //    foreach (var cardInHand in playerContext.Hand.Where(c => cards.Contains(c.Card)))
        //    {
        //        var clues = ClueDetailInfo.CreateClues(cardInHand, playerContext.Player);
        //        if (clues.Any()) return clues.First();
        //    }
        //    return null;
        //}

        #region Log methods

        private void LogUniqueCards()
        {
            var str = "Unique cards:";
            foreach (var card in _boardContext
                .GetUniqueCards()
                .OrderBy(c => c.Rank))
            {
                str += $" {{{card}}}";
            }
            Logger.Log.Info(str);
        }

        //private void LogThoughtPlayAction(Player player, IEnumerable<Card> cards)
        //{
        //    string str = $"Player {_clueGiver.Name} thinks " +
        //                 $"that player {player.Name} plays ";

        //    foreach (var cardToPlay in cards)
        //    {
        //        str += $"{{{cardToPlay}}} ";
        //    }
        //    Logger.Log.Info(str);
        //}

        //private void LogThoughtDiscardWhateverToPlayCards(Player player, IEnumerable<Card> cards)
        //{
        //    string info = $"Player {_clueGiver.Name} thinks " +
        //                    $"that player {player.Name} " +
        //                    "discards ";

        //    foreach (var card in cards)
        //    {
        //        info += $"{{{card}}} ";
        //    }

        //    Logger.Log.Info(info);
        //}

        //private void LogThoughtPlayFiveRankedAction(Player player)
        //{
        //    Logger.Log.Info(
        //        $"Player {_clueGiver.Name} thinks " +
        //        $"that player {player.Name} plays five-ranked card");
        //}

        //private void LogThoughtAboutBlowAction(Player player, IEnumerable<Card> cards)
        //{
        //    string info = $"Player {_clueGiver.Name} thinks " +
        //                  $"that player {player.Name} blows ";
        //    foreach (var card in cards)
        //    {
        //        info += $"{{{card}}}";
        //    }
        //    Logger.Log.Info(info);
        //}

        //private void LogThoughtAboutDiscardUniqueCardsAction(Player player, IEnumerable<Card> cards)
        //{
        //    string info = $"Player {_clueGiver.Name} thinks " +
        //                  $"that player {player.Name} discards uniqueCard(s) ";

        //    foreach (var card in cards)
        //    {
        //        info += $"{{{card}}}";
        //    }

        //    Logger.Log.Info(info);
        //}

        //private void LogThoughtAboutDiscardNoNeedCardsAction(Player player, IEnumerable<Card> cards)
        //{
        //    string info = $"Player {_clueGiver.Name} thinks " +
        //                  $"that player {player.Name} discards no need card(s)";

        //    foreach (var card in cards)
        //    {
        //        info += $" {{{card}}}";
        //    }

        //    Logger.Log.Info(info);
        //}

        #endregion
    }
}