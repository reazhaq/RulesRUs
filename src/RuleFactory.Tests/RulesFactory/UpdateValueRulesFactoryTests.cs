using System;
using FluentAssertions;
using RuleFactory.RulesFactory;
using RuleFactory.Tests.Model;
using Xunit;
using Xunit.Abstractions;

namespace RuleFactory.Tests.RulesFactory
{
    public class UpdateValueRulesFactoryTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public UpdateValueRulesFactoryTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void UpdatePropertyStingWithDifferentValue()
        {
            var game = new Game {Name = "game name"};
            var nameChangeRule = UpdateValueRulesFactory.CreateUpdateValueRule<Game, string>((g => g.Name));

            var compileResult = nameChangeRule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(nameChangeRule)}:{Environment.NewLine}" +
                                        $"{nameChangeRule.ExpressionDebugView()}");

            _testOutputHelper.WriteLine($"before game.Name: {game.Name}");
            nameChangeRule.UpdateFieldOrPropertyValue(game, "new name");
            game.Name.Should().Be("new name");
            _testOutputHelper.WriteLine($"after game.Name: {game.Name}");
        }

        [Fact]
        public void UpdatePropertyFromAnotherRule()
        {
            var game = new Game {Name = "game name"};
            var const1 = ConstantRulesFactory.CreateConstantRule<string>("name from constant rule");
            var nameChangeRule = UpdateValueRulesFactory.CreateUpdateValueRule<Game>((g => g.Name), const1);

            var compileResult = nameChangeRule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(nameChangeRule)}:{Environment.NewLine}" +
                                        $"{nameChangeRule.ExpressionDebugView()}");

            _testOutputHelper.WriteLine($"before game.Name: {game.Name}");
            nameChangeRule.UpdateFieldOrPropertyValue(game);
            game.Name.Should().Be("name from constant rule");
            _testOutputHelper.WriteLine($"after game.Name: {game.Name}");
        }
    }
}