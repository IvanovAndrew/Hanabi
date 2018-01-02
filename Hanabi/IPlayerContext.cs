using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Hanabi
{
    [ContractClass(typeof(PlayerContextContract))]
    public interface IPlayerContext
    {
        Player Player { get; }
        IEnumerable<CardInHand> Hand { get; }
        ClueType PossibleClue { get; set; }

        bool IsSubtleClue(CardInHand cardInHand, IEnumerable<Card> expectedCards);
        IList<ClueType> GetCluesAboutCard(CardInHand cardInHand);
        bool KnowAboutRankOrColor(CardInHand cardInHand);
        IPlayerContext Clone();
    }

    [ContractClassFor(typeof(IPlayerContext))]
    abstract class PlayerContextContract : IPlayerContext
    {
        public Player Player
        {
            get
            {
                Contract.Ensures(Contract.Result<Player>() != null);
                throw new NotSupportedException();
            }
        }

        public IEnumerable<CardInHand> Hand
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<CardInHand>>() != null);
                throw new NotSupportedException();
            }
        }

        public ClueType PossibleClue { get; set; }

        public bool IsSubtleClue(CardInHand cardInHand, IEnumerable<Card> expectedCards)
        {
            Contract.Requires<ArgumentNullException>(cardInHand != null);
            Contract.Requires<ArgumentNullException>(expectedCards != null);
            
            throw new NotSupportedException();
        }

        public IList<ClueType> GetCluesAboutCard(CardInHand cardInHand)
        {
            Contract.Requires<ArgumentNullException>(cardInHand != null);
            var contractResult = Contract.Result<IList<ClueType>>();
            Contract.Ensures(contractResult != null);

            throw new NotSupportedException();
        }

        public bool KnowAboutRankOrColor(CardInHand cardInHand)
        {
            Contract.Requires<ArgumentNullException>(cardInHand != null);
            throw new NotSupportedException();
        }

        public IPlayerContext Clone()
        {
            Contract.Ensures(Contract.Result<IPlayerContext>() != null);
            throw new NotSupportedException();
        }
    }
}
