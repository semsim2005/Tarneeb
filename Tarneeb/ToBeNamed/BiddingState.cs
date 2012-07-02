using System.Collections.Generic;

namespace Tarneeb.Models
{
    public class BiddingState
    {
        public List<Bid> Bids { get; set; }
        public PlayerPosition CurrentTurn { get; set; }
        public bool IsClosed { get; set; }
        public PlayerPosition? Winner { get; set; }
        public Bid WinnerBid { get; set; }
    }
}
