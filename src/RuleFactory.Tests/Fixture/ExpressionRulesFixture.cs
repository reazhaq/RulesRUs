using System;
using System.Linq;
using RuleFactory.Tests.Model;

namespace RuleFactory.Tests.Fixture
{
    public class ExpressionRulesFixture : IDisposable
    {
        public void Dispose(){}
        public Game Game1 { get; }
        public Game Game2 { get; }

        public ExpressionRulesFixture()
        {
            var someRandomNumber = new Random();
            Game1 = new Game
            {
                Name = "Game 1",
                Description = "super boring game",
                Active = false,
                Ranking = 99,
                Rating = "Low"
            };
            Game1.Players.AddRange(Enumerable.Range(1, 40).Select(x => new Player
            {
                Id = x,
                Name = $"Player{x}",
                Country = new Country
                {
                    CountryCode = Country.Countries[someRandomNumber.Next(x, Country.Countries.Length - 1)]
                },
                CurrentScore = 100 - x,
                CurrentCoOrdinates = new CoOrdinate { X = x, Y = x }
            }));

            Game2 = new Game
            {
                Name = "Game 2",
                Description = "super cool game",
                Active = true,
                Ranking = 98,
                Rating = "Medium"
            };
            Game2.Players.AddRange(Enumerable.Range(1, 60).Select(x => new Player
            {
                Id = 99-x,
                Name = $"Player{99-x}",
                Country = new Country
                {
                    CountryCode = Country.Countries[someRandomNumber.Next(x, Country.Countries.Length - 1)]
                },
                CurrentScore = 100 - x,
                CurrentCoOrdinates = new CoOrdinate { X = 99-x, Y = 99-x }
            }));
        }
    }
}