using System;
using System.Collections.Generic;
using Tarneeb.Engine.Models;

namespace Tarneeb.Engine.EventArguments
{
    public class PlayersInformationEventArgs : EventArgs
    {
        public List<Team> Teams { get; set; }
    }
}
