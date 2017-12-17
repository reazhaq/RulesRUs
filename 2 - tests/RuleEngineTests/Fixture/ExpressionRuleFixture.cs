using System;
using RuleEngineTests.Model;

namespace RuleEngineTests.Fixture
{
    public class ExpressionRuleFixture : IDisposable
    {
        public void Dispose(){}
        public Game Game1 { get; }
        public Game Game2 { get; }

        public ExpressionRuleFixture()
        {
            Game1 = new Game
            {
                Name = "Game 1",
                Description = "super boring game",
                Active = false,
                Ranking = 99
            };
            Game1.Players.Add(new Player
            {
                Id = 1,
                Name = "Player1",
                Country = new Country { CountryCode = "US" },
                CurrentScore = 99,
                CurrentCoOrdinates = new CoOrdinate { X = 1, Y = 1 }
            });
            Game1.Players.Add(new Player
            {
                Id = 2,
                Name = "Player2",
                Country = new Country { CountryCode = "CA" },
                CurrentScore = 98,
                CurrentCoOrdinates = new CoOrdinate { X = 2, Y = 2 }
            });
            Game1.Players.Add(new Player
            {
                Id = 3,
                Name = "Player3",
                Country = new Country { CountryCode = "MX" },
                CurrentScore = 97,
                CurrentCoOrdinates = new CoOrdinate { X = 3, Y = 3 }
            });
            Game1.Players.Add(new Player
            {
                Id = 4,
                Name = "Player4",
                Country = new Country { CountryCode = "CR" },
                CurrentScore = 96,
                CurrentCoOrdinates = new CoOrdinate { X = 4, Y = 4 }
            });

            Game2 = new Game
            {
                Name = "Game 2",
                Description = "super cool game",
                Active = true,
                Ranking = 98
            };
            Game2.Players.Add(new Player
            {
                Id = 1,
                Name = "Player1",
                Country = new Country { CountryCode = "US" },
                CurrentScore = 99,
                CurrentCoOrdinates = new CoOrdinate { X = 1, Y = 1 }
            });
            Game2.Players.Add(new Player
            {
                Id = 2,
                Name = "Player2",
                Country = new Country { CountryCode = "CA" },
                CurrentScore = 98,
                CurrentCoOrdinates = new CoOrdinate { X = 2, Y = 2 }
            });
            Game2.Players.Add(new Player
            {
                Id = 3,
                Name = "Player3",
                Country = new Country { CountryCode = "MX" },
                CurrentScore = 97,
                CurrentCoOrdinates = new CoOrdinate { X = 3, Y = 3 }
            });
            Game2.Players.Add(new Player
            {
                Id = 4,
                Name = "Player4",
                Country = new Country { CountryCode = "CR" },
                CurrentScore = 96,
                CurrentCoOrdinates = new CoOrdinate { X = 4, Y = 4 }
            });
        }
    }
}