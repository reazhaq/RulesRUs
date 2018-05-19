using System;
using System.Collections.Generic;
using FluentAssertions;
using RuleFactory.RulesFactory;
using Xunit;
using Xunit.Abstractions;

namespace RuleFactory.Tests.RulesFactory
{
    public class ContainsValueRuleFactoryTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ContainsValueRuleFactoryTests(ITestOutputHelper testOutputHelper)
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
            IList<string> collectionToSearch = new List<string> { "one", "two", "three", "four", "five", "six" };
            var containsRule = ContainsValueRuleFactory.CreateContainsValueRule(collectionToSearch,
                                                    "System.StringComparer", "OrdinalIgnoreCase");

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
            IList<string> collectionToSearch = new List<string> { "one", "two", "three", "four", "five", "six" };
            var containsRule = ContainsValueRuleFactory.CreateContainsValueRule(collectionToSearch, null, null);

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
            var containsRule = ContainsValueRuleFactory.CreateContainsValueRule(new List<int>{ 1, 2, 3, 4, 5, 6 },
                                                                                        null,null);

            var compileResult = containsRule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(containsRule)}:{Environment.NewLine}" +
                                        $"{containsRule.ExpressionDebugView()}");

            var containsValue = containsRule.ContainsValue(valueToSearch);
            _testOutputHelper.WriteLine($"expected: {expectedResult} - actual: {containsValue}");
            containsValue.Should().Be(expectedResult);
        }
    }
}