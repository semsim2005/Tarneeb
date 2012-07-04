namespace Tarneeb.Engine.Models
{
    public class Card
    {
        public int Rank { get; set; }
        public Suit Suit { get; set; }

        public Card() { }

        public Card(int rank, Suit suit)
        {
            Rank = rank;
            Suit = suit;
        }

        public int Weight
        {
            get { return Rank == 1 ? Rank * 14 : Rank; }
        }
    }
}