using System;
using Tarneeb.Engine.Models;

namespace Tarneeb.Engine.EventArguments
{
    public class BidEventArgs : EventArgs
    {
        public Bid Bid { get; set; }

        public Player Caller { get; set; }

        public Player NextCaller { get; set; }
    }
}
