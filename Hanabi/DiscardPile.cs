namespace Hanabi
{
    public class DiscardPile : Pile
    {
        public override bool AddCard(Card card)
        {
            _cards.Add(card);
            return true;
        }
    }
}