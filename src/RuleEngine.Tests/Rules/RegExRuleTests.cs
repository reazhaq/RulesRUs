using System;
using FluentAssertions;
using RuleEngine.Rules;
using RuleEngine.Tests.Model;
using Xunit;
using Xunit.Abstractions;

namespace RuleEngine.Tests.Rules
{
    public class RegExRuleTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public RegExRuleTests(ITestOutputHelper testOutputHelper)
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
            var alphaRule = new RegExRule<Game>
            {
                ObjectToValidate = "Name",
                OperatorToUse = "IsMatch",
                RegExToUse = regExToUse
            };

            var compileRuleResult = alphaRule.Compile();
            compileRuleResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(alphaRule)}:{Environment.NewLine}{alphaRule.ExpressionDebugView()}");

            var game = new Game {Name = nameToUse};

            var executeResult = alphaRule.IsMatch(game);
            _testOutputHelper.WriteLine($"executeResult={executeResult}; expectedResult={expectedResult} for nameToUse={nameToUse}");
            executeResult.Should().Be(expectedResult);
        }
    }
}