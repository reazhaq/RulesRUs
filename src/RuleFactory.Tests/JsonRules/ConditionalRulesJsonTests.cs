using System;
using FluentAssertions;
using Newtonsoft.Json;
using RuleEngine.Rules;
using SampleModel;
using Xunit;
using Xunit.Abstractions;

namespace RuleFactory.Tests.JsonRules
{
    public class ConditionalRulesJsonTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ConditionalRulesJsonTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Theory]
        [InlineData("one", "element is present in the collection")]
        [InlineData("nine", "element is not present in the collection")]
        public void ConditionalWithConstantRule(string valueToCheck, string expectedOutput)
        {
            var rule = new ConditionalFuncRule<string, string>
            {
                ConditionRule = new ContainsValueRule<string>
                {
                    EqualityComparerClassName = "System.StringComparer",
                    EqualityComparerPropertyName = "OrdinalIgnoreCase",
                    CollectionToSearch = { "one", "two", "three", "four", "five", "six" }
                },
                TrueRule = new ConstantRule<string, string> { Value = "element is present in the collection" },
                FalseRule = new ConstantRule<string, string> { Value = "element is not present in the collection" }
            };

            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(rule)}:{Environment.NewLine}" +
                                        $"{rule.ExpressionDebugView()}");

            var ruleResult = rule.Execute(valueToCheck);
            _testOutputHelper.WriteLine($"expected: {expectedOutput} - actual: {ruleResult}");
            ruleResult.Should().BeEquivalentTo(expectedOutput);

            // convert to json
            var ruleJson = JsonConvert.SerializeObject(rule, new JsonConverterForRule());
            _testOutputHelper.WriteLine($"{nameof(ruleJson)}:{Environment.NewLine}{ruleJson}");
            // re-hydrate from json
            var ruleFromJson = JsonConvert.DeserializeObject<Rule>(ruleJson, new JsonConverterForRule());
            compileResult = ruleFromJson.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(ruleFromJson)}:{Environment.NewLine}" +
                                        $"{ruleFromJson.ExpressionDebugView()}");

            ruleResult = ((ConditionalFuncRule<string, string>) ruleFromJson).Execute(valueToCheck);
            _testOutputHelper.WriteLine($"expected: {expectedOutput} - actual: {ruleResult}");
            ruleResult.Should().BeEquivalentTo(expectedOutput);
        }

        [Fact]
        public void ConditionalRuleToUpdateName()
        {
            var rule = new ConditionalIfThActionRule<Game>
            {
                ConditionRule = new MethodCallRule<Game, bool>
                {
                    ObjectToCallMethodOn = "Name",
                    MethodToCall = "Equals",
                    MethodParameters = { new ConstantRule<string> { Value = "some name" }, 
                        new ConstantRule<StringComparison> { Value = "CurrentCultureIgnoreCase" }
                    }
                },
                TrueRule = new UpdateValueRule<Game>
                {
                    ObjectToUpdate = "Name",
                    SourceDataRule = new ConstantRule<string> { Value = "updated name" }
                }
            };

            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(rule)}:{Environment.NewLine}" +
                                        $"{rule.ExpressionDebugView()}");

            var game = new Game { Name = "some name" };
            _testOutputHelper.WriteLine($"before game.Name: {game.Name}");
            rule.Execute(game);
            _testOutputHelper.WriteLine($"after game.Name: {game.Name}");
            game.Name.Should().Be("updated name");

            // convert to json
            var ruleJson = JsonConvert.SerializeObject(rule, new JsonConverterForRule());
            _testOutputHelper.WriteLine($"{nameof(ruleJson)}:{Environment.NewLine}{ruleJson}");
            // re-hydrate from json
            var ruleFromJson = (ConditionalIfThActionRule<Game>)JsonConvert.DeserializeObject<Rule>(ruleJson, new JsonConverterForRule());
            var compileResult2 = ruleFromJson.Compile();
            compileResult2.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(ruleFromJson)}:{Environment.NewLine}" +
                                        $"{ruleFromJson.ExpressionDebugView()}");

            game.Name = "some name";
            ruleFromJson.Execute(game);
            game.Name.Should().Be("updated name");
        }

        [Fact]
        public void ConditionalRuleLookAtOneValueUpdateAnother()
        {
            var rule = new ConditionalIfThActionRule<Player>
            {
                ConditionRule = new ValidationRule<Player>
                {
                    ObjectToValidate = "Country.CountryCode",
                    OperatorToUse = "Equal",
                    ValueToValidateAgainst = new ConstantRule<string> { Value = "ab" }
                },
                TrueRule = new UpdateValueRule<Player>
                {
                    ObjectToUpdate = "CurrentCoOrdinates.X",
                    SourceDataRule = new ConstantRule<int> { Value = "999" }
                }
            };

            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(rule)}:{Environment.NewLine}" +
                                        $"{rule.ExpressionDebugView()}");

            var player = new Player
            {
                Country = new Country { CountryCode = "ab" },
                CurrentCoOrdinates = new CoOrdinate { X = 1, Y = 1 }
            };
            rule.Execute(player);
            player.CurrentCoOrdinates.X.Should().Be(999);
            _testOutputHelper.WriteLine($"expected: 999 - actual: {player.CurrentCoOrdinates.X}");

            // convert to json
            var ruleJson = JsonConvert.SerializeObject(rule, new JsonConverterForRule());
            _testOutputHelper.WriteLine($"{nameof(ruleJson)}:{Environment.NewLine}{ruleJson}");
            // re-hydrate from json
            var ruleFromJson = (ConditionalIfThActionRule<Player>)JsonConvert.DeserializeObject<Rule>(ruleJson, new JsonConverterForRule());
            var compileResult2 = ruleFromJson.Compile();
            compileResult2.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(ruleFromJson)}:{Environment.NewLine}" +
                                        $"{ruleFromJson.ExpressionDebugView()}");

            player.CurrentCoOrdinates.X = 1;
            ruleFromJson.Execute(player);
            player.CurrentCoOrdinates.X.Should().Be(999);
        }

        [Fact]
        public void ConditionalRuleToUpdateNameToSomethingElse()
        {
            var rule = new ConditionalIfThElActionRule<Game>
            {
                ConditionRule = new MethodCallRule<Game, bool>
                {
                    ObjectToCallMethodOn = "Name",
                    MethodToCall = "Equals",
                    MethodParameters = { new ConstantRule<string> { Value = "some name" }, 
                        new ConstantRule<StringComparison> { Value = "CurrentCultureIgnoreCase" }
                    }
                },
                TrueRule = new UpdateValueRule<Game>
                {
                    ObjectToUpdate = "Name",
                    SourceDataRule = new ConstantRule<string> { Value = "true name" }
                },
                FalseRule = new UpdateValueRule<Game>
                {
                    ObjectToUpdate = "Name",
                    SourceDataRule = new ConstantRule<string> { Value = "false name" }
                }
            };

            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(rule)}:{Environment.NewLine}" +
                                        $"{rule.ExpressionDebugView()}");

            var game = new Game { Name = "some name" };
            _testOutputHelper.WriteLine($"before game.Name: {game.Name}");
            rule.Execute(game);
            _testOutputHelper.WriteLine($"after game.Name: {game.Name}");
            game.Name.Should().Be("true name");

            rule.Execute(game);
            _testOutputHelper.WriteLine($"after after game.Name: {game.Name}");
            game.Name.Should().Be("false name");

            // convert to json
            var ruleJson = JsonConvert.SerializeObject(rule, new JsonConverterForRule());
            _testOutputHelper.WriteLine($"{nameof(ruleJson)}:{Environment.NewLine}{ruleJson}");
            // re-hydrate from json
            var ruleFromJson = (ConditionalIfThElActionRule<Game>)JsonConvert.DeserializeObject<Rule>(ruleJson, new JsonConverterForRule());
            var compileResult2 = ruleFromJson.Compile();
            compileResult2.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(ruleFromJson)}:{Environment.NewLine}" +
                                        $"{ruleFromJson.ExpressionDebugView()}");

            game.Name = "some name";
            _testOutputHelper.WriteLine($"before game.Name: {game.Name}");
            ruleFromJson.Execute(game);
            _testOutputHelper.WriteLine($"after game.Name: {game.Name}");
            game.Name.Should().Be("true name");

            ruleFromJson.Execute(game);
            _testOutputHelper.WriteLine($"after after game.Name: {game.Name}");
            game.Name.Should().Be("false name");
        }
    }
}