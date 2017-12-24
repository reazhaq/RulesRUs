using System;
using FluentAssertions;
using RuleEngine.Rules;
using RuleEngineTests.Fixture;
using RuleEngineTests.Model;
using Xunit;
using Xunit.Abstractions;

namespace RuleEngineTests.Rules
{
    public class MethodCallRuleTests : IClassFixture<ExpressionRulesFixture>
    {
        private readonly ITestOutputHelper _testOutcomeHelper;
        private readonly Game _game1;
        private readonly Game _game2;

        public MethodCallRuleTests(ExpressionRulesFixture expressionRuleFixture, ITestOutputHelper testOutcomeHelper)
        {
            _game1 = expressionRuleFixture.Game1;
            _game2 = expressionRuleFixture.Game2;
            _testOutcomeHelper = testOutcomeHelper;
        }

        [Theory]
        [InlineData("Game 1", true)]
        [InlineData("game 1", true)]
        [InlineData("game 2", false)]
        [InlineData("gaMe 2", false)]
        public void CallEqualsMethodOnName(string input1, bool expectedResult)
        {
            var nameEqualsRule = new MethodCallRule<Game, bool>
            {
                ObjectToValidate = "Name",
                OperatorToUse = "Equals",
                Inputs = {input1, StringComparison.CurrentCultureIgnoreCase}
            };

            var compileResult = nameEqualsRule.Compile();
            compileResult.Should().BeTrue();


            var executeResult = nameEqualsRule.Execute(_game1);
            executeResult.Should().Be(expectedResult);

            executeResult = nameEqualsRule.Execute(_game2);
            executeResult.Should().Be(!expectedResult);
        }

        [Theory]
        [InlineData("Game 1", true)]
        [InlineData("game 1", true)]
        [InlineData("game 2", false)]
        [InlineData("gaMe 2", false)]
        public void CallEqualsMethodOnNameUsingConstantRule(string input1, bool expectedResult)
        {
            var nameEqualsRule = new MethodCallRule<Game, bool>
            {
                ObjectToValidate = "Name",
                OperatorToUse = "Equals",
                Inputs = { new ConstantRule<string> { Value = input1}, new ConstantRule<StringComparison> { Value="CurrentCultureIgnoreCase"} }
            };

            var compileResult = nameEqualsRule.Compile();
            compileResult.Should().BeTrue();


            var executeResult = nameEqualsRule.Execute(_game1);
            executeResult.Should().Be(expectedResult);

            executeResult = nameEqualsRule.Execute(_game2);
            executeResult.Should().Be(!expectedResult);
        }
    }
}