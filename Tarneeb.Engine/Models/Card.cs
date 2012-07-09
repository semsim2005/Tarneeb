namespace Tarneeb.Engine.Models
{
    public class Card
    {
        public int Rank { get; set; }
        public Suit Suit { get; set; }
        public Suit Trump { get; set; }
        public bool IsPlayed { get; set; }

        public Card() { }

        public Card(int rank, Suit suit)
        {
            Rank = rank;
            Suit = suit;
        }

        public int Weight
        {
            get
            {
                var extraTrumpWeight = (Trump == Suit) ? 20 : 0;
                return Rank == 1 ? 14 + extraTrumpWeight : Rank + extraTrumpWeight;
            }
        }
    }
}