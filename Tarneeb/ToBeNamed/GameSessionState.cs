using System;
using System.Collections.Generic;

namespace Tarneeb.Models
{
    public class GameSessionState
    {
        public Guid Id { get; set; }
        public Dictionary<PlayerPosition, Player> Players { get; set; }
        public GameSessionStatus Status { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
