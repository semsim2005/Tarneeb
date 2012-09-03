using System.Collections.Generic;
using System.Linq;

namespace Tarneeb.Engine.Models
{
    public class Team
    {
        private readonly List<Player> _players = new List<Player>(2);
        public List<Player> Players { get { return _players; } }

        public int PlayersScore
        {
            get { return _players.Sum(p => p.Score); }
        }

        public string Name { get; set; }

        public int TeamScore { get; set; }

        public bool AddPlayer(Player player)
        {
            if (_players.Count == 2)
                return false;

            _players.Add(player);
            return true;
        }

        public void RemovePlayer(Player player)
        {
            _players.Remove(player);
        }
    }
}