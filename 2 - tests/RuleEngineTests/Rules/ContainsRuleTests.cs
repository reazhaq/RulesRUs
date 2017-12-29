using System;
using System.Collections.Generic;
using FluentAssertions;
using RuleEngine.Rules;
using Xunit;

namespace RuleEngineTests.Rules
{
    public class ContainsRuleTests
    {
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

            var containsValue = containsRule.ContainsValue(valueToSearch);
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

            var containsValue = containsRule.ContainsValue(valueToSearch);
            containsValue.Should().Be(expectedResult);
        }
    }
}