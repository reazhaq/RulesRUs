using System;
using System.Collections.Generic;
using FluentAssertions;
using RuleEngine.Rules;
using Xunit;
using Xunit.Abstractions;

namespace RuleEngine.Tests.Rules
{
    public class ContainsValueRuleTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ContainsValueRuleTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Theory]
        [InlineData("one", true)]
        [InlineData("Two", true)]
        [InlineData("THREE", true)]
        [InlineData("fIVe", true)]
        [InlineData("Six", true)]
        [InlineData("seven", false)]
        public void ContainsValueTestWithIgnoreCase(string valueToSearch, bool expectedResult)
        {
            var containsRule = new ContainsValueRule<string>
            {
                EqualityComparer = StringComparer.OrdinalIgnoreCase,
                CollectionToSearch = {"one", "two", "three", "four", "five", "six"}
            };

            var compileResult = containsRule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(containsRule)}:{Environment.NewLine}" +
                                        $"{containsRule.ExpressionDebugView()}");

            var containsValue = containsRule.ContainsValue(valueToSearch);
            _testOutputHelper.WriteLine($"expected: {expectedResult} - actual: {containsValue}");
            containsValue.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData("one", true)]
        [InlineData("Two", false)]
        [InlineData("THREE", false)]
        [InlineData("fIVe", false)]
        [InlineData("Six", false)]
        [InlineData("seven", false)]
        public void ContainsValueTestCaseSensitive(string valueToSearch, bool expectedResult)
        {
            var containsRule = new ContainsValueRule<string>
            {
                EqualityComparer = StringComparer.Ordinal,
                CollectionToSearch = { "one", "two", "three", "four", "five", "six" }
            };

            var compileResult = containsRule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(containsRule)}:{Environment.NewLine}" +
                                        $"{containsRule.ExpressionDebugView()}");

            var containsValue = containsRule.ContainsValue(valueToSearch);
            _testOutputHelper.WriteLine($"expected: {expectedResult} - actual: {containsValue}");
            containsValue.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(2, true)]
        [InlineData(7, false)]
        public void ContainsValueTestForIntCollection(int valueToSearch, bool expectedResult)
        {
            var containsRule = new ContainsValueRule<int>
            {
                EqualityComparer = null,
                CollectionToSearch = {1, 2, 3, 4, 5, 6}
            };

            var compileResult = containsRule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(containsRule)}:{Environment.NewLine}" +
                                        $"{containsRule.ExpressionDebugView()}");

            var containsValue = containsRule.ContainsValue(valueToSearch);
            _testOutputHelper.WriteLine($"expected: {expectedResult} - actual: {containsValue}");
            containsValue.Should().Be(expectedResult);
        }

        [Fact]
        public void CreateComparerOnTheFlyUsingReflection()
        {
            var containsRule = new ContainsValueRule<string>
            {
                EqualityComparerClassName = "System.StringComparer",
                EqualityComparerPropertyName = "OrdinalIgnoreCase",
                CollectionToSearch = { "one", "two", "three", "four", "five", "six" }
            };

            var compileResult = containsRule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(containsRule)}:{Environment.NewLine}" +
                                        $"{containsRule.ExpressionDebugView()}");

            var a1 = containsRule.ContainsValue("One");
            a1.Should().BeTrue();
            var a2 = containsRule.ContainsValue("tWo");
            a2.Should().BeTrue();
            var a7 = containsRule.ContainsValue("seven");
            a7.Should().BeFalse();
        }
    }
}