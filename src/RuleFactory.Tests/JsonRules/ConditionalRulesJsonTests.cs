using FluentAssertions;
using Newtonsoft.Json;
using RuleEngine.Rules;
using System;
using ModelForUnitTests;
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
        [InlineData("one", "six-six-six")]
        [InlineData("tWo", "six-six-six")]
        [InlineData("blah", "blah")]
        [InlineData("nine", "nine")]
        public void IfValueContainsReturnDiffValue2ToAndFromJson(string searchValue, string expectedValue)
        {
            var valueReplacementIfBad = new ConditionalFuncRule<string, string>
            {
                ConditionRule = new ContainsValueRule<string>
                {
                    EqualityComparerClassName = "System.StringComparer",
                    EqualityComparerPropertyName = "OrdinalIgnoreCase",
                    CollectionToSearch = { "one", "two", "three", "four", "five", "six" }
                },
                TrueRule = new ConstantRule<string,string>{Value = "six-six-six"},
                FalseRule = new SelfReturnRule<string>()
            };

            var compileResult = valueReplacementIfBad.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(valueReplacementIfBad)}:{Environment.NewLine}" +
                                        $"{valueReplacementIfBad.ExpressionDebugView()}");

            var ruleResult = valueReplacementIfBad.Execute(searchValue);
            _testOutputHelper.WriteLine($"expected: {expectedValue} - actual: {ruleResult}");
            ruleResult.Should().Be(expectedValue);

            var converter = new JsonConverterForRule();
            // convert to json
            var ruleJson = JsonConvert.SerializeObject(valueReplacementIfBad, converter);
            _testOutputHelper.WriteLine(ruleJson);
            // re-hydrate
            var ruleFromJson = JsonConvert.DeserializeObject<ConditionalFuncRule<string, string>>(ruleJson, converter);

            var compileResult2 = ruleFromJson.Compile();
            compileResult2.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(ruleFromJson)}:{Environment.NewLine}" +
                                        $"{ruleFromJson.ExpressionDebugView()}");
            var ruleResult2 = ruleFromJson.Execute(searchValue);
            _testOutputHelper.WriteLine($"expected: {expectedValue} - actual: {ruleResult2}");
            ruleResult2.Should().Be(expectedValue);
        }

        [Theory]
        [InlineData("one", "element is present in the collection")]
        [InlineData("nine", "element is not present in the collection")]
        public void ConditionalWithConstantRuleToAndFromJson(string valueToCheck, string expectedOutput)
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
        public void ConditionalRuleToUpdateNameToAndFromJson()
        {
            var rule = new ConditionalIfThActionRule<Game>
            {
                ConditionRule = new MethodCallRule<Game, bool>
                {
                    ObjectToCallMethodOn = "Name",
                    MethodToCall = "Equals",
                    MethodParameters = {
                        new ConstantRule<string> { Value = "some name" }, 
                        new ConstantRule<StringComparison> { Value = "CurrentCultureIgnoreCase" }
                    }
                },
                TrueRule = new UpdateValueRule<Game>
                {
                    ObjectToUpdate = "Name",
                    SourceDataRule = new ConstantRule<string> { Value = "updated name" }
                }
            };

            // convert to json
            var ruleJson = JsonConvert.SerializeObject(rule, Formatting.Indented, new JsonConverterForRule());
            _testOutputHelper.WriteLine($"{nameof(ruleJson)}:{Environment.NewLine}{ruleJson}");
            // re-hydrate from json
            var ruleFromJson = JsonConvert.DeserializeObject<ConditionalIfThActionRule<Game>>(ruleJson, new JsonConverterForRule());
            var compileResult = ruleFromJson.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(ruleFromJson)}:{Environment.NewLine}" +
                                        $"{ruleFromJson.ExpressionDebugView()}");

            var game = new Game {Name = "some name"};
            ruleFromJson.Execute(game);
            game.Name.Should().Be("updated name");
        }

        [Fact]
        public void ConditionalRuleLookAtOneValueUpdateAnotherToAndFromJson()
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

            // convert to json
            var ruleJson = JsonConvert.SerializeObject(rule, Formatting.Indented, new JsonConverterForRule());
            _testOutputHelper.WriteLine($"{nameof(ruleJson)}:{Environment.NewLine}{ruleJson}");
            // re-hydrate from json
            var ruleFromJson = JsonConvert.DeserializeObject<ConditionalIfThActionRule<Player>>(ruleJson, new JsonConverterForRule());
            var compileResult = ruleFromJson.Compile();
            compileResult.Should().BeTrue();

            var player = new Player
            {
                Country = new Country { CountryCode = "ab" },
                CurrentCoOrdinates = new CoOrdinate { X = 1, Y = 1 }
            };
            ruleFromJson.Execute(player);
            player.CurrentCoOrdinates.X.Should().Be(999);
        }

        [Fact]
        public void ConditionalRuleToUpdateNameToSomethingElseToAndFromJson()
        {
            var rule = new ConditionalIfThElActionRule<Game>
            {
                ConditionRule = new MethodCallRule<Game, bool>
                {
                    ObjectToCallMethodOn = "Name",
                    MethodToCall = "Equals",
                    MethodParameters = {
                        new ConstantRule<string> { Value = "some name" }, 
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

            // convert to json
            var ruleJson = JsonConvert.SerializeObject(rule, Formatting.Indented, new JsonConverterForRule());
            _testOutputHelper.WriteLine($"{nameof(ruleJson)}:{Environment.NewLine}{ruleJson}");
            // re-hydrate from json
            var ruleFromJson = JsonConvert.DeserializeObject<ConditionalIfThElActionRule<Game>>(ruleJson, new JsonConverterForRule());
            var compileResult = ruleFromJson.Compile();
            compileResult.Should().BeTrue();

            var game = new Game { Name = "some name" };
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