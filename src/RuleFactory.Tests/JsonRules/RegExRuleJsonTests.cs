using System;
using FluentAssertions;
using Newtonsoft.Json;
using RuleEngine.Rules;
using SampleModel;
using Xunit;
using Xunit.Abstractions;

namespace RuleFactory.Tests.JsonRules
{
    public class RegExRuleJsonTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public RegExRuleJsonTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Theory]
        [InlineData("ValidName", @"^[a-zA-Z]*$", true)]
        [InlineData("BadName1", @"^[a-zA-Z]*$", false)]
        [InlineData("AnotherBadName#", @"^[a-zA-Z]*$", false)]
        [InlineData("BadName1", @"^[a-zA-Z0-9]*$", true)]
        public void NameMatchesRegEx(string nameToUse, string regExToUse, bool expectedResult)
        {
            var rule = new RegExRule<Game>
            {
                ObjectToValidate = "Name",
                RegExToUse = regExToUse
            };

            var compileRuleResult = rule.Compile();
            compileRuleResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(rule)}:{Environment.NewLine}{rule.ExpressionDebugView()}");

            var game = new Game {Name = nameToUse};

            var executeResult = rule.IsMatch(game);
            _testOutputHelper.WriteLine($"executeResult={executeResult}; expectedResult={expectedResult} for nameToUse={nameToUse}");
            executeResult.Should().Be(expectedResult);

            // convert to json
            var ruleJson = JsonConvert.SerializeObject(rule, new CustomRuleJsonConverter());
            _testOutputHelper.WriteLine($"{nameof(ruleJson)}:{Environment.NewLine}{ruleJson}");
            // re-hydrate from json
            var ruleFromJson = JsonConvert.DeserializeObject<Rule>(ruleJson, new CustomRuleJsonConverter());
            var compileResult = ruleFromJson.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(ruleFromJson)}:{Environment.NewLine}" +
                                        $"{ruleFromJson.ExpressionDebugView()}");

            game.Name = nameToUse;
            executeResult = ((RegExRule<Game>)ruleFromJson).IsMatch(game);
            _testOutputHelper.WriteLine($"executeResult={executeResult}; expectedResult={expectedResult} for nameToUse={nameToUse}");
            executeResult.Should().Be(expectedResult);
        }
    }
}