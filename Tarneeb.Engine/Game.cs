
using Tarneeb.Engine.Models;

namespace Tarneeb.Engine
{
    public class Game
    {

        public string Id { get; set; }

        public void PlayCard(Card card)
        {

        }

        private void ResetGame()
        {
            _cardsPlayers.Clear();
            _bidTeam = null;
            IsDoube = false;
        }

        public void AddPlayerToTeam(string teamName, Player player)
        {
            var team = Add(teamName);
            team.AddPlayer(player);
        }
    }
}
