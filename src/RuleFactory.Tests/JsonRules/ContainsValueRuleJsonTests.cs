using System;
using FluentAssertions;
using Newtonsoft.Json;
using RuleEngine.Rules;
using Xunit;
using Xunit.Abstractions;

namespace RuleFactory.Tests.JsonRules
{
    public class ContainsValueRuleJsonTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ContainsValueRuleJsonTests(ITestOutputHelper testOutputHelper)
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
            var rule = new ContainsValueRule<string>
            {
                EqualityComparerPropertyName = "OrdinalIgnoreCase",
                EqualityComparerClassName = "System.StringComparer",
                CollectionToSearch = {"one", "two", "three", "four", "five", "six"}
            };

            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(rule)}:{Environment.NewLine}" +
                                        $"{rule.ExpressionDebugView()}");

            var containsValue = rule.ContainsValue(valueToSearch);
            _testOutputHelper.WriteLine($"expected: {expectedResult} - actual: {containsValue}");
            containsValue.Should().Be(expectedResult);

            // convert to Json
            var ruleJson = JsonConvert.SerializeObject(rule, new CustomRuleJsonConverter());
            _testOutputHelper.WriteLine($"{nameof(ruleJson)}:{Environment.NewLine}{ruleJson}");
            // re-hydrate from json
            var ruleFromJson = JsonConvert.DeserializeObject<Rule>(ruleJson, new CustomRuleJsonConverter());
            compileResult = ruleFromJson.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(ruleFromJson)}:{Environment.NewLine}" +
                                        $"{ruleFromJson.ExpressionDebugView()}");

            containsValue = ((ContainsValueRule<string>)ruleFromJson).ContainsValue(valueToSearch);
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
            var rule = new ContainsValueRule<string>
            {
                EqualityComparerPropertyName = "Ordinal",
                EqualityComparerClassName = "System.StringComparer",
                CollectionToSearch = { "one", "two", "three", "four", "five", "six" }
            };

            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(rule)}:{Environment.NewLine}" +
                                        $"{rule.ExpressionDebugView()}");

            var containsValue = rule.ContainsValue(valueToSearch);
            _testOutputHelper.WriteLine($"expected: {expectedResult} - actual: {containsValue}");
            containsValue.Should().Be(expectedResult);

            // convert to Json
            var ruleJson = JsonConvert.SerializeObject(rule, new CustomRuleJsonConverter());
            _testOutputHelper.WriteLine($"{nameof(ruleJson)}:{Environment.NewLine}{ruleJson}");
            // re-hydrate from json
            var ruleFromJson = JsonConvert.DeserializeObject<Rule>(ruleJson, new CustomRuleJsonConverter());
            compileResult = ruleFromJson.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(ruleFromJson)}:{Environment.NewLine}" +
                                        $"{ruleFromJson.ExpressionDebugView()}");

            containsValue = ((ContainsValueRule<string>)ruleFromJson).ContainsValue(valueToSearch);
            _testOutputHelper.WriteLine($"expected: {expectedResult} - actual: {containsValue}");
            containsValue.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(2, true)]
        [InlineData(7, false)]
        public void ContainsValueTestForIntCollection(int valueToSearch, bool expectedResult)
        {
            var rule = new ContainsValueRule<int>
            {
                EqualityComparer = null,
                CollectionToSearch = {1, 2, 3, 4, 5, 6}
            };

            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(rule)}:{Environment.NewLine}" +
                                        $"{rule.ExpressionDebugView()}");

            var containsValue = rule.ContainsValue(valueToSearch);
            _testOutputHelper.WriteLine($"expected: {expectedResult} - actual: {containsValue}");
            containsValue.Should().Be(expectedResult);

            // convert to Json
            var ruleJson = JsonConvert.SerializeObject(rule, new CustomRuleJsonConverter());
            _testOutputHelper.WriteLine($"{nameof(ruleJson)}:{Environment.NewLine}{ruleJson}");
            // re-hydrate from json
            var ruleFromJson = JsonConvert.DeserializeObject<Rule>(ruleJson, new CustomRuleJsonConverter());
            compileResult = ruleFromJson.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(ruleFromJson)}:{Environment.NewLine}" +
                                        $"{ruleFromJson.ExpressionDebugView()}");

            containsValue = ((ContainsValueRule<int>)ruleFromJson).ContainsValue(valueToSearch);
            _testOutputHelper.WriteLine($"expected: {expectedResult} - actual: {containsValue}");
            containsValue.Should().Be(expectedResult);
        }
    }
}