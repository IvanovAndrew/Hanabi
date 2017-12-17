using System.Collections.Generic;

namespace Hanabi
{
    public class ThoughtsAboutCard
    {
        public CardInHand CardInHand { get; set; }
        public Guess Guess { get; set; }
        public IList<Clue> Clues { get; set; }
    }
}