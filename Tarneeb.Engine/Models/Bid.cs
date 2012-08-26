using System;

namespace Tarneeb.Engine.Models
{
    public class Bid : IComparable<Bid>
    {
        public int Call { get; set; }
        public Suit Suit { get; set; }
        public int TricksRequired { get { return Call + 6; } }

        public Bid() { }

        public Bid(int call, Suit suit)
        {
            Call = call;
            Suit = suit;
        }

        public int CompareTo(Bid other)
        {
            if (Call > other.Call)
                return 1;
            if (Call < other.Call)
                return -1;

            return Suit.CompareTo(other.Suit);
        }

        public bool IsBidSatisfied(int collectedTricks)
        {
            return collectedTricks >= TricksRequired;
        }
    }
}