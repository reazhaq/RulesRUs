using System;
using FluentAssertions;
using Newtonsoft.Json;
using RuleEngine.Rules;
using RuleFactory.Tests.Model;
using Xunit;
using Xunit.Abstractions;

namespace RuleFactory.Tests.JsonRules
{
    public class UpdateRulesJsonTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public UpdateRulesJsonTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void UpdatePropertyStingWithDifferentValue()
        {
            var game = new Game {Name = "game name"};
            var rule = new UpdateValueRule<Game, string>
            {
                ObjectToUpdate = "Name"
            };

            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(rule)}:{Environment.NewLine}" +
                                        $"{rule.ExpressionDebugView()}");

            _testOutputHelper.WriteLine($"before game.Name: {game.Name}");
            rule.UpdateFieldOrPropertyValue(game, "new name");
            game.Name.Should().Be("new name");
            _testOutputHelper.WriteLine($"after game.Name: {game.Name}");

            // convert to json
            var ruleJson = JsonConvert.SerializeObject(rule, new CustomRuleJsonConverter());
            _testOutputHelper.WriteLine($"{nameof(ruleJson)}:{Environment.NewLine}{ruleJson}");
            // re-hydrate from json
            var ruleFromJson = JsonConvert.DeserializeObject<Rule>(ruleJson, new CustomRuleJsonConverter());
            compileResult = ruleFromJson.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(ruleFromJson)}:{Environment.NewLine}" +
                                        $"{ruleFromJson.ExpressionDebugView()}");

            game.Name = "game name";
            ((UpdateValueRule<Game, string>)ruleFromJson).UpdateFieldOrPropertyValue(game, "new name");
            game.Name.Should().Be("new name");
        }

        [Fact]
        public void UpdatePropertyFromAnotherRule()
        {
            var game = new Game {Name = "game name"};
            var rule = new UpdateValueRule<Game>
            {
                ObjectToUpdate = "Name",
                SourceDataRule = new ConstantRule<string> {Value = "name from constant rule"}
            };

            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(rule)}:{Environment.NewLine}" +
                                        $"{rule.ExpressionDebugView()}");

            _testOutputHelper.WriteLine($"before game.Name: {game.Name}");
            rule.UpdateFieldOrPropertyValue(game);
            game.Name.Should().Be("name from constant rule");
            _testOutputHelper.WriteLine($"after game.Name: {game.Name}");

            // convert to json
            var ruleJson = JsonConvert.SerializeObject(rule, new CustomRuleJsonConverter());
            _testOutputHelper.WriteLine($"{nameof(ruleJson)}:{Environment.NewLine}{ruleJson}");
            // re-hydrate from json
            var ruleFromJson = JsonConvert.DeserializeObject<Rule>(ruleJson, new CustomRuleJsonConverter());
            compileResult = ruleFromJson.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(ruleFromJson)}:{Environment.NewLine}" +
                                        $"{ruleFromJson.ExpressionDebugView()}");

            game.Name = "game name";
            ((UpdateValueRule<Game>) ruleFromJson).UpdateFieldOrPropertyValue(game);
            game.Name.Should().Be("name from constant rule");
        }
    }
}