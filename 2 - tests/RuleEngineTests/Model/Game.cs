using System.Collections.Generic;
using System.Linq;

namespace RuleEngineTests.Model
{
    public class Game
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Ranking { get; set; }
        public bool Active { get; set; }

        public List<Player> Players { get; } = new List<Player>();

        public bool HasPlayer(int id) => Players.Any(p => p.Id == id);

        public void FlipActive() => Active = !Active;
    }
}