namespace Hanabi
{
    public class CardInHand
    {
        public Card Card { get; }
        public Player Player { get; }

        public CardInHand(Player player, Card card) : this(card, player) { }

        public CardInHand(Card card, Player player)
        {
            Card = card;
            Player = player;
        }

        public override string ToString()
        {
            return Card.ToString();
        }
    }
}
