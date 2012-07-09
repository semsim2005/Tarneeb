using System.Collections.Generic;
using System.Linq;
using Tarneeb.Engine.Models;

namespace Tarneeb.Engine
{
    public class Teams
    {
        private readonly IList<Team> _teams = new List<Team>();

        public Team this[string name]
        {
            get { return _teams.FirstOrDefault(t => t.Name == name); }
        }

        private Team Add(string name)
        {
            var team = _teams.FirstOrDefault(t => t.Name == name);
            if (team == null)
            {
                team = new Team { Name = name };
                _teams.Add(team);
            }

            return team;
        }

        public void AddPlayerToTeam(string teamName, Player player)
        {
            var team = Add(teamName);
            team.AddPlayer(player);
        }
    }
}