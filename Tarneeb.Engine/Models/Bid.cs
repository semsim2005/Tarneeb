using System;

namespace Tarneeb.Engine.Models
{
    public class Bid : IComparable<Bid>
    {
        public int Tricks { get; set; }
        public Suit Suit { get; set; }

        public Bid() { }

        public Bid(int tricks, Suit suit)
        {
            Tricks = tricks;
            Suit = suit;
        }

        public int CompareTo(Bid other)
        {
            if (Tricks > other.Tricks)
                return 1;
            if (Tricks < other.Tricks)
                return -1;

            return Suit.CompareTo(other.Suit);
        }
    }
}