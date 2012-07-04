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

        public int MaximumNumberOfPlayers { get; set; }

        public bool AddPlayer(Player player)
        {
            if (Players.Count >= MaximumNumberOfPlayers)
                return false;
            Players.Add(player);
            return true;
        }

        public void RemovePlayer(Player player)
        {
            Players.Remove(player);
        }
    }
}