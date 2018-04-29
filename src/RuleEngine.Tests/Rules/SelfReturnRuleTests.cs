using System;
using FluentAssertions;
using RuleEngine.Rules;
using Xunit;
using Xunit.Abstractions;

namespace RuleEngine.Tests.Rules
{
    public class SelfReturnRuleTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public SelfReturnRuleTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Theory]
        [InlineData(5)]
        [InlineData(-5)]
        [InlineData(int.MaxValue)]
        public void IntSelfReturn(int someValue)
        {
            var rule = new SelfReturnRule<int>();
            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"selfReturnRule for Int:{Environment.NewLine}" +
                                        $"{rule.ExpressionDebugView()}");

            var value = rule.Get(someValue);
            value.Should().Be(someValue);
        }

        [Theory]
        [InlineData("one")]
        [InlineData(null)]
        [InlineData("")]
        public void StringSelfReturn(string someValue)
        {
            var rule = new SelfReturnRule<string>();
            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"selfReturnRule for String:{Environment.NewLine}" +
                                        $"{rule.ExpressionDebugView()}");

            var value = rule.Get(someValue);
            value.Should().Be(someValue);
        }
    }
}