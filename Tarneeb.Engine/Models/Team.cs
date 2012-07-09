using System.Collections.Generic;
using System.Linq;

namespace Tarneeb.Engine.Models
{
    public class Team
    {
        private IList<Player> _players;

        private IList<Player> Players
        {
            get { return _players ?? (_players = new List<Player>()); }
        }

        public int PlayersScore
        {
            get { return Players.Sum(p => p.Score); }
        }

        public string Name { get; set; }

        public int TeamScore { get; set; }

        public void AddPlayer(Player player)
        {
            Players.Add(player);
        }

        public void RemovePlayer(Player player)
        {
            Players.Remove(player);
        }
    }
}