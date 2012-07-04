using System.Collections.Generic;
using System.Linq;
using Tarneeb.Engine.Models;

namespace Tarneeb.Engine
{
    public class Teams
    {
        private readonly IList<Team> _teams = new List<Team>();
        public int MaximumNumberOfPlayers { get; set; }
        public int MaximumNumberOfTeams { get; set; }

        public Team this[string name]
        {
            get { return _teams.FirstOrDefault(t => t.Name == name); }
        }

        private bool AddTeam(string name)
        {
            if (_teams.Count >= MaximumNumberOfTeams)
                return false;
            _teams.Add(new Team { Name = name, MaximumNumberOfPlayers = MaximumNumberOfPlayers });
            return true;
        }

        public bool AddPlayerToTeam(string teamName, Player player)
        {
            var results = false;

            if (!_teams.Any(t => t.Name == teamName))
            {
                results = AddTeam(teamName);
            }
            return results && _teams.First(t => t.Name == teamName).AddPlayer(player);
        }
    }
}