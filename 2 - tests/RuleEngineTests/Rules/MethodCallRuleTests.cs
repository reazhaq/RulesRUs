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
                ObjectToCallMethodOn = "Name",
                MethodToCall = "Equals",
                Inputs = { input1, StringComparison.CurrentCultureIgnoreCase }
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
                ObjectToCallMethodOn = "Name",
                MethodToCall = "Equals",
                Inputs = { new ConstantRule<string> { Value = input1 }, new ConstantRule<StringComparison> { Value = "CurrentCultureIgnoreCase" } }
            };

            var compileResult = nameEqualsRule.Compile();
            compileResult.Should().BeTrue();


            var executeResult = nameEqualsRule.Execute(_game1);
            executeResult.Should().Be(expectedResult);

            executeResult = nameEqualsRule.Execute(_game2);
            executeResult.Should().Be(!expectedResult);
        }

        [Fact]
        public void CallAVoidMethod()
        {
            var playerCountRule = new MethodVoidCallRule<Game>
            {
                MethodToCall = "FlipActive"
            };

            var compileResult = playerCountRule.Compile();
            compileResult.Should().BeTrue();

            var currentActiveState = _game1.Active;
            playerCountRule.Execute(_game1);
            _game1.Active.Should().Be(!currentActiveState);
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(1000, false)]
        public void CheckToSeeIfPlayerExistsInAGame(int id, bool expectedResult)
        {
            var gameHasPlayerWithCertainId = new MethodCallRule<Game, bool>
            {
                MethodToCall = "HasPlayer",
                Inputs = {id}
            };

            var compileResult = gameHasPlayerWithCertainId.Compile();
            compileResult.Should().BeTrue();

            var executeResult = gameHasPlayerWithCertainId.Execute(_game1);
            executeResult.Should().Be(expectedResult);
        }

        [Fact]
        public void CallAStringMethodOnDescriptionObject()
        {
            var gameNameContainsKeyWrodCool = new MethodCallRule<Game, bool>
            {
                MethodToCall = "Contains",
                ObjectToCallMethodOn = "Description",
                Inputs = { new ConstantRule<string> { Value = "cool" } }
            };

            var compileResult = gameNameContainsKeyWrodCool.Compile();
            compileResult.Should().BeTrue();

            var executeResult = gameNameContainsKeyWrodCool.Execute(_game1);
            executeResult.Should().BeFalse();

            executeResult = gameNameContainsKeyWrodCool.Execute(_game2);
            executeResult.Should().BeTrue();
        }
    }
}