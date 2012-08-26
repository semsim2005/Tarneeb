using System.Collections.Generic;
using System.Linq;

namespace Tarneeb.Engine.Models
{
    public class Team
    {
        private readonly IList<Player> _players = new List<Player>();

        public int PlayersScore
        {
            get { return _players.Sum(p => p.Score); }
        }

        public string Name { get; set; }

        public int TeamScore { get; set; }

        public void AddPlayer(Player player)
        {
            _players.Add(player);
        }

        public void RemovePlayer(Player player)
        {
            _players.Remove(player);
        }
    }
}