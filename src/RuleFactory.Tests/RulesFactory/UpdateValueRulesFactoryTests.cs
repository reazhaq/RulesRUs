using System;
using FluentAssertions;
using ModelForUnitTests;
using RuleEngine.Rules;
using RuleFactory.RulesFactory;
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
        public void UpdatePropertyStingWithDifferentValueUsingFactory()
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
        public void UpdatePropertyFromAnotherRuleUsingFactory()
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

        [Fact]
        public void UpdateStringRefUsingFactory()
        {
            var sourceDataRule = ConstantRulesFactory.CreateConstantRule<string>("something");
            var rule = UpdateValueRulesFactory.CreateUpdateRefValueRule<string>(sourceDataRule);

            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"UpdateRefValueRule<string>:{Environment.NewLine}" +
                                        $"{rule.ExpressionDebugView()}");

            var string1 = "one";
            rule.RefUpdate(ref string1);
            string1.Should().Be("something");

            // source value shall come as argument
            var rule2 = UpdateValueRulesFactory.CreateUpdateRefValueRule<string>();
            compileResult = rule2.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"UpdateRefValueRule<string, string>:{Environment.NewLine}" +
                                        $"{rule2.ExpressionDebugView()}");

            string1 = null;
            rule2.RefUpdate(ref string1, "some other value");
            string1.Should().Be("some other value");
        }

        [Fact]
        public void UpdateIntRefUsingFactory()
        {
            var sourceDataRule = ConstantRulesFactory.CreateConstantRule<int>("99");
            var rule = UpdateValueRulesFactory.CreateUpdateRefValueRule<int>(sourceDataRule);

            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{rule.ExpressionDebugView()}");

            var myInt = 0;
            rule.RefUpdate(ref myInt);
            myInt.Should().Be(99);

            var rule2 = UpdateValueRulesFactory.CreateUpdateRefValueRule<int>();
            compileResult = rule2.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"UpdateRefValueRule<int, int>:{Environment.NewLine}" +
                                        $"{rule2.ExpressionDebugView()}");

            rule2.RefUpdate(ref myInt, -99);
            myInt.Should().Be(-99);
        }
    }
}