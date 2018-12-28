using System;
using FluentAssertions;
using ModelForUnitTests;
using Newtonsoft.Json;
using RuleEngine.Rules;
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
        public void UpdatePropertyStingWithDifferentValueToAndFromJson()
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
        public void UpdatePropertyFromAnotherRuleToAndFromJson()
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
        public void UpdateStringRefToAndFromJson()
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
        public void UpdateStringRef2ToAndFromJson()
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

        [Fact]
        public void UpdateIntRefToAndFromJson()
        {
            var rule = new UpdateRefValueRule<int>
            {
                SourceDataRule = new ConstantRule<int>{Value = "99"}
            };

            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();

            var myInt = 0;
            rule.RefUpdate(ref myInt);
            myInt.Should().Be(99);

            var jsonConverterForRule = new JsonConverterForRule();
            // convert to json
            var ruleJson = JsonConvert.SerializeObject(rule, jsonConverterForRule);
            _testOutputHelper.WriteLine($"ruleJson:{Environment.NewLine}{ruleJson}");
            // new rule from Json
            var newRule = JsonConvert.DeserializeObject<UpdateRefValueRule<int>>(ruleJson, jsonConverterForRule);

            var compileResult2 = newRule.Compile();
            compileResult2.Should().BeTrue();
            var myInt2 = 0;
            newRule.RefUpdate(ref myInt2);
            myInt2.Should().Be(99);

            var rule2 = new UpdateRefValueRule<int>();
            compileResult = rule2.Compile();
            compileResult.Should().BeTrue();

            rule2.RefUpdate(ref myInt, -99);
            myInt.Should().Be(-99);

            // convert to json
            var ruleJson2 = JsonConvert.SerializeObject(rule2, jsonConverterForRule);
            _testOutputHelper.WriteLine($"ruleJson2:{Environment.NewLine}{ruleJson2}");
            // new rule from json
            var newRule2 = JsonConvert.DeserializeObject<UpdateRefValueRule<int>>(ruleJson2, jsonConverterForRule);

            var compileResult3 = newRule2.Compile();
            compileResult3.Should().BeTrue();

            newRule2.RefUpdate(ref myInt2, -99);
            myInt2.Should().Be(-99);
        }
    }
}