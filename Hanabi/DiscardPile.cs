namespace Hanabi
{
    public class DiscardPile : Pile
    {
        public override bool AddCard(Card card)
        {
            Matrix[card]++;
            return true;
        }

        public DiscardPile(IGameProvider provider) : base(provider)
        {
        }
    }
}