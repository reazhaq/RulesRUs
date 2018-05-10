using System;
using FluentAssertions;
using Newtonsoft.Json;
using RuleEngine.Rules;
using SampleModel;
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
            var ruleJson = JsonConvert.SerializeObject(rule, new JsonConverterForRule());
            _testOutputHelper.WriteLine($"{nameof(ruleJson)}:{Environment.NewLine}{ruleJson}");
            // re-hydrate from json
            var ruleFromJson = JsonConvert.DeserializeObject<Rule>(ruleJson, new JsonConverterForRule());
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
            var ruleJson = JsonConvert.SerializeObject(rule, new JsonConverterForRule());
            _testOutputHelper.WriteLine($"{nameof(ruleJson)}:{Environment.NewLine}{ruleJson}");
            // re-hydrate from json
            var ruleFromJson = JsonConvert.DeserializeObject<Rule>(ruleJson, new JsonConverterForRule());
            compileResult = ruleFromJson.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(ruleFromJson)}:{Environment.NewLine}" +
                                        $"{ruleFromJson.ExpressionDebugView()}");

            game.Name = "game name";
            ((UpdateValueRule<Game>) ruleFromJson).UpdateFieldOrPropertyValue(game);
            game.Name.Should().Be("name from constant rule");
        }

        [Fact]
        public void UpdateStringRef()
        {
            // source value is fixed with a constant rule
            var ruleBefore = new UpdateRefValueRule<string>
            {
                SourceDataRule = new ConstantRule<string>{Value = "something"}
            };

            var ruleJsonConverter = new JsonConverterForRule();
            // convert to json
            var ruleJson = JsonConvert.SerializeObject(ruleBefore, ruleJsonConverter);
            _testOutputHelper.WriteLine($"ruleJson:{Environment.NewLine}{ruleJson}");
            // read from json
            var ruleAfter = JsonConvert.DeserializeObject<Rule>(ruleJson, ruleJsonConverter);


            var compileResult = ruleAfter.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"UpdateRefValueRule<string>:{Environment.NewLine}" +
                                        $"{ruleAfter.ExpressionDebugView()}");

            var string1 = "one";
            ((UpdateRefValueRule<string>)ruleAfter).RefUpdate(ref string1);
            string1.Should().Be("something");
        }

        [Fact]
        public void UpdateStringRef2()
        {
            // source value shall come as argument
            var ruleBefore = new UpdateRefValueRule<string>();

            var ruleJsonConverter = new JsonConverterForRule();
            // convert to json
            var ruleJson = JsonConvert.SerializeObject(ruleBefore, ruleJsonConverter);
            _testOutputHelper.WriteLine($"ruleJson:{Environment.NewLine}{ruleJson}");
            // read from json
            var ruleAfter = JsonConvert.DeserializeObject<Rule>(ruleJson, ruleJsonConverter);


            var compileResult = ruleAfter.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"UpdateRefValueRule<string, string>:{Environment.NewLine}" +
                                        $"{ruleAfter.ExpressionDebugView()}");

            string string1 = null;
            ((UpdateRefValueRule<string>)ruleAfter).RefUpdate(ref string1, "some other value");
            string1.Should().Be("some other value");
        }
    }
}