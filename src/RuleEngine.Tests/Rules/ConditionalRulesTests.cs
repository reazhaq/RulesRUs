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

            searchValue = valueReplacementIfBad.Execute(searchValue);
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

            var ruleResult = evenOrOddResult.Execute(evenOddValue);
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

            var ruleResult = containsTextRule.Execute(valueToCheck);
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

            var game = new Game {Name = "some name"};
            conditionalUpdateValue.Execute(game);
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

            var game = new Game {Name = "some name"};
            conditionalIfThElRule.Execute(game);
            game.Name.Should().Be("true name");

            conditionalIfThElRule.Execute(game);
            game.Name.Should().Be("false name");
        }
    }
}