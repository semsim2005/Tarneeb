using System;
using System.Collections.Generic;
using Tarneeb.Engine.Models;

namespace Tarneeb.Engine.EventArguments
{
    public class GameSetupCompletedEventArgs : EventArgs
    {
        public Dictionary<Card, Player> CardsPlayers { get; set; }
    }
}
