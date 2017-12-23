using FluentAssertions;
using RuleEngine.Rules;
using RuleEngineTests.Fixture;
using RuleEngineTests.Model;
using Xunit;
using Xunit.Abstractions;

namespace RuleEngineTests.Rules
{
    public class RegExRuleTests// : IClassFixture<ExpressionRulesFixture>
    {
        private readonly ITestOutputHelper _testOutcomeHelper;

        public RegExRuleTests(ITestOutputHelper testOutcomeHelper)
        {
            _testOutcomeHelper = testOutcomeHelper;
        }

        [Theory]
        [InlineData("ValidName", @"^[a-zA-Z]*$", true)]
        [InlineData("BadName1", @"^[a-zA-Z]*$", false)]
        [InlineData("AnotherBadName#", @"^[a-zA-Z]*$", false)]
        [InlineData("BadName1", @"^[a-zA-Z0-9]*$", true)]
        public void NameMatchesRegEx(string nameToUse, string regExToUse, bool expectedResult)
        {
            var alphaRule = new RegExRule<Game>
            {
                ObjectToValidate = "Name",
                OperatorToUse = "IsMatch",
                RegExToUse = regExToUse
            };

            var compileRuleResult = alphaRule.Compile();
            compileRuleResult.Should().BeTrue();

            var game = new Game {Name = nameToUse};

            var executeResult = alphaRule.IsMatch(game);
            _testOutcomeHelper.WriteLine($"executeResult={executeResult}; expectedResult={expectedResult} for nameToUse={nameToUse}");
            executeResult.Should().Be(expectedResult);
        }
    }
}