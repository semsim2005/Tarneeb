using System;
using Tarneeb.Engine.Models;

namespace Tarneeb.Engine.EventArguments
{
    public class CardPlayerArgs : EventArgs
    {
        public Card Card { get; set; }

        public Player Player { get; set; }

        public Player NextPlayer { get; set; }
    }
}
