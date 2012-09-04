
namespace Tarneeb.Engine.Models
{
    public class Player
    {
        public string Name { get; set; }

        public string Id { get; set; }

        public int Score { get; set; }

        public Player() { }

        public Player(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}