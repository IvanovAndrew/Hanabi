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

        public DiscardPile Clone()
        {
            var clone = new DiscardPile(this.Provider);

            foreach (var card in Cards)
            {
                clone.AddCard(card);
            }
            return clone;
        }
    }
}