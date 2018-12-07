using System;
using System.Collections.Generic;
using FluentAssertions;
using ModelForUnitTests;
using RuleEngine.Rules;
using RuleFactory.RulesFactory;
using Xunit;
using Xunit.Abstractions;

namespace RuleFactory.Tests.RulesFactory
{
    public class ConditionalRuleFactoryTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ConditionalRuleFactoryTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Theory]
        [InlineData("one", "six-six-six")]
        [InlineData("tWo", "six-six-six")]
        [InlineData("blah", "blah")]
        [InlineData("nine", "nine")]
        public void IfValueContainsReturnDiffValueUsingFactory(string searchValue, string expectedValue)
        {
            IList<string> collectionToSearch = new List<string>{ "one", "two", "three", "four", "five", "six" };
            var containsValueRule = ContainsValueRuleFactory.CreateContainsValueRule(collectionToSearch,
                                                                "System.StringComparer", "OrdinalIgnoreCase");

            var trueRule = new ExpressionFuncRule<string, string>(s => "six-six-six");
            var falseRule = new ExpressionFuncRule<string, string>(s => s);
            var valueReplacementIfBad = ConditionalRulesFactory.CreateConditionalFuncRule<string, string>(containsValueRule,
                                                                                                        trueRule, falseRule);
            var compileResult = valueReplacementIfBad.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(valueReplacementIfBad)}:{Environment.NewLine}" +
                                        $"{valueReplacementIfBad.ExpressionDebugView()}");

            var ruleResult = valueReplacementIfBad.Execute(searchValue);
            _testOutputHelper.WriteLine($"expected: {expectedValue} - actual: {ruleResult}");
            ruleResult.Should().Be(expectedValue);
        }

        [Theory]
        [InlineData("one", "six-six-six")]
        [InlineData("tWo", "six-six-six")]
        [InlineData("blah", "blah")]
        [InlineData("nine", "nine")]
        public void IfValueContainsReturnDiffValue2UsingFactory(string searchValue, string expectedValue)
        {
            var collectionToSearch = new List<string>{ "one", "two", "three", "four", "five", "six" };
            var containsValueRule = ContainsValueRuleFactory.CreateContainsValueRule(collectionToSearch,
                                                "System.StringComparer", "OrdinalIgnoreCase");

            var trueRule = ConstantRulesFactory.CreateConstantRule<string, string>("six-six-six");
            var falseRule = SelfReturnRuleFactory.CreateSelfReturnRule<string>();
            var valueReplacementIfBad = ConditionalRulesFactory.CreateConditionalFuncRule<string,string>(containsValueRule,
                                                                                                        trueRule, falseRule);
            var compileResult = valueReplacementIfBad.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(valueReplacementIfBad)}:{Environment.NewLine}" +
                                        $"{valueReplacementIfBad.ExpressionDebugView()}");

            var ruleResult = valueReplacementIfBad.Execute(searchValue);
            _testOutputHelper.WriteLine($"expected: {expectedValue} - actual: {ruleResult}");
            ruleResult.Should().Be(expectedValue);
        }

        [Theory]
        [InlineData("one", "element is present in the collection")]
        [InlineData("nine", "element is not present in the collection")]
        public void ConditionalWithConstantRule(string valueToCheck, string expectedOutput)
        {
            var trueRule =
                ConstantRulesFactory.CreateConstantRule<string, string>("element is present in the collection");
            var falseRule =
                ConstantRulesFactory.CreateConstantRule<string, string>("element is not present in the collection");

            var searchList = new List<string> {"one", "two", "three", "four", "five", "six"};
            var containsRule =
                ContainsValueRuleFactory.CreateContainsValueRule(searchList, "System.StringComparer",
                    "OrdinalIgnoreCase");

            var containsTextRule =
                ConditionalRulesFactory.CreateConditionalFuncRule<string, string>(containsRule, trueRule, falseRule);

            var compileResult = containsTextRule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(containsTextRule)}:{Environment.NewLine}" +
                                        $"{containsTextRule.ExpressionDebugView()}");

            var ruleResult = containsTextRule.Execute(valueToCheck);
            _testOutputHelper.WriteLine($"expected: {expectedOutput} - actual: {ruleResult}");
            ruleResult.Should().BeEquivalentTo(expectedOutput);
        }

        [Fact]
        public void ConditionalRuleToUpdateName()
        {
            var trueRule = UpdateValueRulesFactory.CreateUpdateValueRule<Game>(g => g.Name,
                ConstantRulesFactory.CreateConstantRule<string>("updated name"));

            var methodParams = new List<Rule>
            {
                ConstantRulesFactory.CreateConstantRule<string>("some name"),
                ConstantRulesFactory.CreateConstantRule<StringComparison>("CurrentCultureIgnoreCase")
            };
            var methodCallRule = MethodCallRulesFactory.CreateMethodCallRule<Game, bool>("Equals", null,
                                                                                        g => g.Name, methodParams);
            var conditionalUpdateValue =
                ConditionalRulesFactory.CreateConditionalIfThActionRule<Game>(methodCallRule, trueRule);

            var compileResult = conditionalUpdateValue.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(conditionalUpdateValue)}:{Environment.NewLine}" +
                                        $"{conditionalUpdateValue.ExpressionDebugView()}");

            var game = new Game { Name = "some name" };
            _testOutputHelper.WriteLine($"before game.Name: {game.Name}");
            conditionalUpdateValue.Execute(game);
            _testOutputHelper.WriteLine($"after game.Name: {game.Name}");
            game.Name.Should().Be("updated name");
        }

        [Fact]
        public void ConditionalRuleToUpdateNameToSomethingElse()
        {
            var const1 = ConstantRulesFactory.CreateConstantRule<string>("true name");
            var trueRule = UpdateValueRulesFactory.CreateUpdateValueRule<Game>(g => g.Name, const1);

            var const2 = ConstantRulesFactory.CreateConstantRule<string>("false name");
            var falseRule = UpdateValueRulesFactory.CreateUpdateValueRule<Game>(g => g.Name, const2);

            var methodParams = new List<Rule>
            {
                ConstantRulesFactory.CreateConstantRule<string>("some name"),
                ConstantRulesFactory.CreateConstantRule<StringComparison>("CurrentCultureIgnoreCase")
            };
            var methodCallRule = MethodCallRulesFactory.CreateMethodCallRule<Game, bool>("Equals", null,
                                                                            g => g.Name, methodParams);

            var conditionalIfThElRule = ConditionalRulesFactory.CreateConditionalIfThElActionRule<Game>(methodCallRule, trueRule, falseRule);

            var compileResult = conditionalIfThElRule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(conditionalIfThElRule)}:{Environment.NewLine}" +
                                        $"{conditionalIfThElRule.ExpressionDebugView()}");

            var game = new Game { Name = "some name" };
            _testOutputHelper.WriteLine($"before game.Name: {game.Name}");
            conditionalIfThElRule.Execute(game);
            _testOutputHelper.WriteLine($"after game.Name: {game.Name}");
            game.Name.Should().Be("true name");

            conditionalIfThElRule.Execute(game);
            _testOutputHelper.WriteLine($"after after game.Name: {game.Name}");
            game.Name.Should().Be("false name");
        }

        [Fact]
        public void ConditionalRuleLookAtOneValueUpdateAnother()
        {
            var const1 = ConstantRulesFactory.CreateConstantRule<int>("999");
            var trueRule = UpdateValueRulesFactory.CreateUpdateValueRule<Player>(p => p.CurrentCoOrdinates.X, const1);

            var const2 = ConstantRulesFactory.CreateConstantRule<string>("ab");
            var validationRule = ValidationRulesFactory.CreateValidationRule<Player>(p => p.Country.CountryCode,
                LogicalOperatorAtTheRootLevel.Equal, const2);

            var conditionalUpdate =
                ConditionalRulesFactory.CreateConditionalIfThActionRule<Player>(validationRule, trueRule);

            var compileResult = conditionalUpdate.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(conditionalUpdate)}:{Environment.NewLine}" +
                                        $"{conditionalUpdate.ExpressionDebugView()}");

            var player = new Player
            {
                Country = new Country { CountryCode = "ab" },
                CurrentCoOrdinates = new CoOrdinate { X = 1, Y = 1 }
            };
            conditionalUpdate.Execute(player);
            player.CurrentCoOrdinates.X.Should().Be(999);
            _testOutputHelper.WriteLine($"expected: 999 - actual: {player.CurrentCoOrdinates.X}");
        }
    }
}