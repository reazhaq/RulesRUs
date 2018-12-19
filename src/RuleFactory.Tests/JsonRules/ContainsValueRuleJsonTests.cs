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
        public void ContainsValueTestWithIgnoreCaseToAndFromJson(string valueToSearch, bool expectedResult)
        {
            var rule = new ContainsValueRule<string>
            {
                EqualityComparerPropertyName = "OrdinalIgnoreCase",
                EqualityComparerClassName = "System.StringComparer",
                CollectionToSearch = {"one", "two", "three", "four", "five", "six"}
            };

            // convert to Json
            var ruleJson = JsonConvert.SerializeObject(rule, Formatting.Indented, new JsonConverterForRule());
            _testOutputHelper.WriteLine($"{nameof(ruleJson)}:{Environment.NewLine}{ruleJson}");
            // re-hydrate from json
            var ruleFromJson = JsonConvert.DeserializeObject<ContainsValueRule<string>>(ruleJson, new JsonConverterForRule());
            var compileResult = ruleFromJson.Compile();
            compileResult.Should().BeTrue();

            var containsValue = ruleFromJson.ContainsValue(valueToSearch);
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
        public void ContainsValueTestCaseSensitiveToAndFromJson(string valueToSearch, bool expectedResult)
        {
            var rule = new ContainsValueRule<string>
            {
                EqualityComparerPropertyName = "Ordinal",
                EqualityComparerClassName = "System.StringComparer",
                CollectionToSearch = { "one", "two", "three", "four", "five", "six" }
            };

            // convert to Json
            var ruleJson = JsonConvert.SerializeObject(rule, Formatting.Indented, new JsonConverterForRule());
            _testOutputHelper.WriteLine($"{nameof(ruleJson)}:{Environment.NewLine}{ruleJson}");
            // re-hydrate from json
            var ruleFromJson = JsonConvert.DeserializeObject<ContainsValueRule<string>>(ruleJson, new JsonConverterForRule());
            var compileResult = ruleFromJson.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(ruleFromJson)}:{Environment.NewLine}" +
                                        $"{ruleFromJson.ExpressionDebugView()}");

            var containsValue = ruleFromJson.ContainsValue(valueToSearch);
            _testOutputHelper.WriteLine($"expected: {expectedResult} - actual: {containsValue}");
            containsValue.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(2, true)]
        [InlineData(7, false)]
        public void ContainsValueTestForIntCollectionToAndFromJson(int valueToSearch, bool expectedResult)
        {
            var rule = new ContainsValueRule<int>
            {
                EqualityComparer = null,
                CollectionToSearch = {1, 2, 3, 4, 5, 6}
            };

            // convert to Json
            var ruleJson = JsonConvert.SerializeObject(rule, Formatting.Indented, new JsonConverterForRule());
            _testOutputHelper.WriteLine($"{nameof(ruleJson)}:{Environment.NewLine}{ruleJson}");
            // re-hydrate from json
            var ruleFromJson = JsonConvert.DeserializeObject<ContainsValueRule<int>>(ruleJson, new JsonConverterForRule());
            var compileResult = ruleFromJson.Compile();
            compileResult.Should().BeTrue();

            var containsValue = ruleFromJson.ContainsValue(valueToSearch);
            _testOutputHelper.WriteLine($"expected: {expectedResult} - actual: {containsValue}");
            containsValue.Should().Be(expectedResult);
        }

        [Fact]
        public void CreateComparerOnTheFlyUsingReflectionToAndFromJson()
        {
            var containsRule = new ContainsValueRule<string>
            {
                EqualityComparerClassName = "System.StringComparer",
                EqualityComparerPropertyName = "OrdinalIgnoreCase",
                CollectionToSearch = { "one", "two", "three", "four", "five", "six" }
            };

            var converter = new JsonConverterForRule();
            // convert to json
            var json = JsonConvert.SerializeObject(containsRule, Formatting.Indented, converter);
            _testOutputHelper.WriteLine($"rule in json:{Environment.NewLine}{json}");
            // bring back from json
            var ruleFromJson = JsonConvert.DeserializeObject<ContainsValueRule<string>>(json, converter);
            var compileResult = ruleFromJson.Compile();
            compileResult.Should().BeTrue();

            var a1 = ruleFromJson.ContainsValue("One");
            a1.Should().BeTrue();
            var a2 = ruleFromJson.ContainsValue("tWo");
            a2.Should().BeTrue();
            var a7 = ruleFromJson.ContainsValue("seven");
            a7.Should().BeFalse();
        }
    }
}