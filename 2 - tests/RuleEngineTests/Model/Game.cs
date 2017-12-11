using System.Collections.Generic;
using System.Linq;

namespace RuleEngineTests.Model
{
    public class Game
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Ranking { get; set; }

        public IList<Player> Players { get; } = new List<Player>();

        public bool HasPlayer(int id) => Players.Any(p => p.Id == id);
    }
}