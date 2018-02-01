using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Hanabi
{
    public class ClueCandidate
    {
        public Player Candidate { get; set; }
        public Clue Clue { get; set; }
    }

    public interface IClueSituationStrategy
    {
        ClueCandidate GetClueCandidate();
    }

    public class NoCriticalSituation : IClueSituationStrategy
    {
        private readonly Player _clueGiver;
        private readonly IBoardContext _boardContext;
        private readonly IReadOnlyList<Player> _players;

        public NoCriticalSituation(Player clueGiver, IReadOnlyList<Player> players, IBoardContext boardContext)
        {
            Contract.Requires<ArgumentNullException>(clueGiver != null);
            Contract.Requires<ArgumentException>(players.Count > 0);
            Contract.Requires<ArgumentNullException>(boardContext != null);

            _clueGiver = clueGiver;
            _players = players;
            _boardContext = boardContext;
        }

        public ClueCandidate GetClueCandidate()
        {
            var otherPlayerCards = GetOtherPlayerCards();


            var manyTinClueStrategy = new ManyTinClueStrategy(_clueGiver, _boardContext);
            var solution = manyTinClueStrategy.FindClueCandidate(_players);

            return solution?.GetClueCandidate();
        }

        private IEnumerable<Card> GetOtherPlayerCards()
        {
            IEnumerable<Card> result = _clueGiver.GetKnownCards();

            foreach (var player in _players)
            {
                result = result.Concat(player.ShowCards(_clueGiver).Select(cih => cih.Card));
            }

            return result;
        }
    }

    public class OnlyClueExistsSituation : IClueSituationStrategy
    {
        private readonly IPlayerContext _playerContext;
        private readonly ClueType _clueType;

        public OnlyClueExistsSituation(IPlayerContext playerContext, ClueType clueType)
        {
            _playerContext = playerContext;
            _clueType = clueType;
        }

        public ClueCandidate GetClueCandidate()
        {
            return new ClueCandidate
                {
                    Candidate = _playerContext.Player,
                    Clue = Clue.Create(_clueType, _playerContext.Hand)
                };
        }
    }

    public class ClueNotExistsSituation : IClueSituationStrategy
    {
        public ClueCandidate GetClueCandidate()
        {
            return null;
        }
    }
}
