using System;
using System.Diagnostics;
using System.Linq.Expressions;
using FluentAssertions;
using RuleEngine.Rules;
using RuleEngine.Tests.Model;
using Xunit;
using Xunit.Abstractions;

namespace RuleEngine.Tests.Rules
{
    public class ConditionalRulesTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ConditionalRulesTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Theory]
        [InlineData("one", "six-six-six")]
        [InlineData("tWo", "six-six-six")]
        [InlineData("blah", "blah")]
        [InlineData("nine", "nine")]
        public void IfValueContainsReturnDiffValue(string searchValue, string expectedValue)
        {
            var valueReplacementIfBad = new ConditionalFuncRule<string, string>
            {
                ConditionRule = new ContainsValueRule<string>
                {
                    EqualityComparer = StringComparer.OrdinalIgnoreCase,
                    CollectionToSearch = { "one", "two", "three", "four", "five", "six" }
                },
                TrueRule = new ExpressionFuncRule<string, string>(s => "six-six-six"),
                FalseRule = new ExpressionFuncRule<string, string>(s => s)
            };

            var compileResult = valueReplacementIfBad.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(valueReplacementIfBad)}:{Environment.NewLine}" +
                                        $"{valueReplacementIfBad.ExpressionDebugView()}");

            searchValue = valueReplacementIfBad.Execute(searchValue);
            _testOutputHelper.WriteLine($"expected: {expectedValue} - actual: {searchValue}");
            searchValue.ShouldBeEquivalentTo(expectedValue);
        }

        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        public void ConditionalOutput(int evenOddValue)
        {
            var evenOrOddOutput = new ConditionalIfThElActionRule<int>
            {
                ConditionRule = new ExpressionFuncRule<int, bool>(i => i % 2 == 0),
                TrueRule = new ExpressionActionRule<int>(i => _testOutputHelper.WriteLine($"{i} is even")),
                FalseRule = new ExpressionActionRule<int>(i => _testOutputHelper.WriteLine($"{i} is odd"))
            };

            var compileResult = evenOrOddOutput.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(evenOrOddOutput)}:{Environment.NewLine}" +
                                        $"{evenOrOddOutput.ExpressionDebugView()}");

            evenOrOddOutput.Execute(evenOddValue);
        }

        [Theory]
        [InlineData(22)]
        [InlineData(33)]
        [InlineData(44)]
        [InlineData(55)]
        public void ConditionalOutputWithElseEmpty(int evenOddValue)
        {
            var evenOrOddOutput = new ConditionalIfThElActionRule<int>
            {
                ConditionRule = new ExpressionFuncRule<int, bool>(i => i % 2 == 0),
                TrueRule = new ExpressionActionRule<int>(i => _testOutputHelper.WriteLine($"{i} is even")),
                FalseRule = new ExpressionActionRule<int>(i => Expression.Empty())
            };

            var compileResult = evenOrOddOutput.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(evenOrOddOutput)}:{Environment.NewLine}" +
                                        $"{evenOrOddOutput.ExpressionDebugView()}");

            evenOrOddOutput.Execute(evenOddValue);
        }

        [Theory]
        [InlineData(2, "2 is even")]
        [InlineData(3, "3 is odd")]
        public void ConditionalOut(int evenOddValue, string expectedResult)
        {
            var evenOrOddResult = new ConditionalFuncRule<int, string>
            {
                ConditionRule = new ExpressionFuncRule<int, bool>(i => i % 2 == 0),
                TrueRule = new ExpressionFuncRule<int, string>(i => string.Format($"{i} is even")),
                FalseRule = new ExpressionFuncRule<int, string>(i => string.Format($"{i} is odd"))
            };

            var compileResult = evenOrOddResult.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(evenOrOddResult)}:{Environment.NewLine}" +
                                        $"{evenOrOddResult.ExpressionDebugView()}");

            var ruleResult = evenOrOddResult.Execute(evenOddValue);
            _testOutputHelper.WriteLine($"expected: {expectedResult} - actual: {ruleResult}");
            ruleResult.Should().BeEquivalentTo(expectedResult);
        }

        [Theory]
        [InlineData("one", "element is present in the collection")]
        [InlineData("nine", "element is not present in the collection")]
        public void ConditionalWithConstantRule(string valueToCheck, string expectedOutput)
        {
            var containsTextRule = new ConditionalFuncRule<string, string>
            {
                ConditionRule = new ContainsValueRule<string>
                {
                    EqualityComparer = StringComparer.OrdinalIgnoreCase,
                    CollectionToSearch = { "one", "two", "three", "four", "five", "six" }
                },
                TrueRule = new ConstantRule<string, string> { Value = "element is present in the collection" },
                FalseRule = new ConstantRule<string, string> { Value = "element is not present in the collection" }
            };

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
            var conditionalUpdateValue = new ConditionalIfThActionRule<Game>
            {
                ConditionRule = new MethodCallRule<Game, bool>
                {
                    ObjectToCallMethodOn = "Name",
                    MethodToCall = "Equals",
                    Inputs = { "some name", StringComparison.CurrentCultureIgnoreCase }
                },
                TrueRule = new UpdateValueRule<Game>
                {
                    ObjectToUpdate = "Name",
                    SourceDataRule = new ConstantRule<string> { Value = "updated name" }
                }
            };

            var compileResult = conditionalUpdateValue.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(conditionalUpdateValue)}:{Environment.NewLine}" +
                                        $"{conditionalUpdateValue.ExpressionDebugView()}");

            var game = new Game {Name = "some name"};
            _testOutputHelper.WriteLine($"before game.Name: {game.Name}");
            conditionalUpdateValue.Execute(game);
            _testOutputHelper.WriteLine($"after game.Name: {game.Name}");
            game.Name.Should().Be("updated name");
        }

        [Fact]
        public void ConditionalRuleToUpdateNameToSomethingElse()
        {
            var conditionalIfThElRule = new ConditionalIfThElActionRule<Game>
            {
                ConditionRule = new MethodCallRule<Game, bool>
                {
                    ObjectToCallMethodOn = "Name",
                    MethodToCall = "Equals",
                    Inputs = { "some name", StringComparison.CurrentCultureIgnoreCase }
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

            var compileResult = conditionalIfThElRule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(conditionalIfThElRule)}:{Environment.NewLine}" +
                                        $"{conditionalIfThElRule.ExpressionDebugView()}");

            var game = new Game {Name = "some name"};
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
            var conditionalUpdate = new ConditionalIfThActionRule<Player>
            {
                ConditionRule = new ValidationRule<Player>
                {
                    ObjectToValidate = "Country.CountryCode",
                    OperatorToUse = "Equal",
                    ValueToValidateAgainst = new ConstantRule<string>{Value = "ab"}
                },
                TrueRule = new UpdateValueRule<Player>
                {
                    ObjectToUpdate = "CurrentCoOrdinates.X",
                    SourceDataRule = new ConstantRule<int>{Value = "999"}
                }
            };

            var compileResult = conditionalUpdate.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(conditionalUpdate)}:{Environment.NewLine}" +
                                        $"{conditionalUpdate.ExpressionDebugView()}");

            var player = new Player
            {
                Country = new Country {CountryCode = "ab"},
                CurrentCoOrdinates = new CoOrdinate {X = 1, Y = 1}
            };
            conditionalUpdate.Execute(player);
            player.CurrentCoOrdinates.X.Should().Be(999);
            _testOutputHelper.WriteLine($"expected: 999 - actual: {player.CurrentCoOrdinates.X}");
        }
    }
}