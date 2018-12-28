using System;
using System.Linq;
using ModelForUnitTests;

namespace RuleFactory.Tests.Fixture
{
    public class ValidationRulesFixture : IDisposable
    {
        public void Dispose() { }

        public Game Game { get; }

        public ValidationRulesFixture()
        {
            var someRandomNumber = new Random();

            Game = new Game
            {
                Name = "Game 1",
                Description = "super boring game",
                Active = false,
                Ranking = 99,
                Rating = "High"
            };
            Game.Players.AddRange(Enumerable.Range(1, 4).Select(x => new Player
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
        }
    }
}