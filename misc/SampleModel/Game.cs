using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SampleModel
{
    public class Game
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Ranking { get; set; }
        public bool Active { get; set; }

        public string Rating { get; set; }
        public List<Player> Players { get; } = new List<Player>();

        public bool HasPlayer(int id) => Players.Any(p => p.Id == id);

        public void FlipActive() => Active = !Active;

        public static Game CreateGame() => new Game {Active = false, Name = "new"};
        public static Game CreateGame(string name) => new Game {Active = false, Name = name};

        public static Game CreateGame(string name, string description, int ranking, bool active) =>
            new Game {Name = name, Description = description, Ranking = ranking, Active = active};

        public static int SomeStaticIntValue;
        public static void SomeVoidStaticMethod() => SomeStaticIntValue++;
        public static void SomeVoidStaticMethod(int newValue) => SomeStaticIntValue = newValue;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}