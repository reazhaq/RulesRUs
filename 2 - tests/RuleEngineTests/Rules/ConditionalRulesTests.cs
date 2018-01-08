using System;
using FluentAssertions;
using RuleEngine.Rules;
using Xunit;
using Xunit.Abstractions;

namespace RuleEngineTests.Rules
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
        [InlineData("blah", null)]
        public void IfValueContainsReturnDiffValue(string searchValue, string expectedValue)
        {
            var valueReplacementIfBad = new ConditionalFuncRule<string, string>
            {
                ConditionRule = new ContainsValueRule<string>
                {
                    EqualityComparer = StringComparer.OrdinalIgnoreCase,
                    CollectionToSearch = {"one", "two", "three", "four", "five", "six"}
                },
                TrueRule = new ConstantRule<string> {Value = "six-six-six"},
                FalseRule = new ConstantRule<string> {Value = null}
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
            var evenOrOddOutput = new ConditionalActionRule<int>
            {
                ConditionRule = new ExpressionFuncRules<int, bool>(i => i % 2 == 0),
                TrueRule = new ExpressionActionRule<int>(i=>_testOutputHelper.WriteLine($"{i} is even")),
                FalseRule = new ExpressionActionRule<int>(i=>_testOutputHelper.WriteLine($"{i} is odd"))
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
                ConditionRule = new ExpressionFuncRules<int, bool>(i => i % 2 == 0),
                TrueRule = new ExpressionFuncRules<int, string>(i => string.Format($"{i} is even")),
                FalseRule = new ExpressionFuncRules<int, string>(i => string.Format($"{i} is odd"))
            };

            var compileResult = evenOrOddResult.Compile();
            compileResult.Should().BeTrue();

            var ruleResult = evenOrOddResult.Execute(evenOddValue);
            ruleResult.Should().BeEquivalentTo(expectedResult);
        }
    }
}