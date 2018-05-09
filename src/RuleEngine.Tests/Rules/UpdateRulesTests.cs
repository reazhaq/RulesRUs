using System;
using FluentAssertions;
using RuleEngine.Rules;
using SampleModel;
using Xunit;
using Xunit.Abstractions;

namespace RuleEngine.Tests.Rules
{
    public class UpdateRulesTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public UpdateRulesTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void UpdatePropertyStingWithDifferentValue()
        {
            var game = new Game {Name = "game name"};
            var nameChangeRule = new UpdateValueRule<Game, string>
            {
                ObjectToUpdate = "Name"
            };

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
            var nameChangeRule = new UpdateValueRule<Game>
            {
                ObjectToUpdate = "Name",
                SourceDataRule = new ConstantRule<string> {Value = "name from constant rule"}
            };

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
        public void UpdateStringRef()
        {
            // source value is fixed with a constant rule
            var rule = new UpdateRefValueRule<string>
            {
                SourceDataRule = new ConstantRule<string>{Value = "something"}
            };

            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"UpdateRefValueRule<string>:{Environment.NewLine}" +
                                        $"{rule.ExpressionDebugView()}");

            var string1 = "one";
            rule.RefUpdate(ref string1);
            string1.Should().Be("something");

            // source value shall come as argument
            var rule2 = new UpdateRefValueRule<string>();
            compileResult = rule2.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"UpdateRefValueRule<string, string>:{Environment.NewLine}" +
                                        $"{rule2.ExpressionDebugView()}");

            string1 = null;
            rule2.RefUpdate(ref string1, "some other value");
            string1.Should().Be("some other value");
        }

        [Fact]
        public void UpdateIntRef()
        {
            var rule = new UpdateRefValueRule<int>
            {
                SourceDataRule = new ConstantRule<int>{Value = "99"}
            };

            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{rule.ExpressionDebugView()}");

            var myInt = 0;
            rule.RefUpdate(ref myInt);
            myInt.Should().Be(99);

            var rule2 = new UpdateRefValueRule<int>();
            compileResult = rule2.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"UpdateRefValueRule<int, int>:{Environment.NewLine}" +
                                        $"{rule2.ExpressionDebugView()}");

            rule2.RefUpdate(ref myInt, -99);
            myInt.Should().Be(-99);
        }

    }
}