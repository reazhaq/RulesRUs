using System;
using FluentAssertions;
using RuleEngine.Rules;
using RuleEngine.Tests.Fixture;
using RuleEngine.Tests.Model;
using Xunit;
using Xunit.Abstractions;

namespace RuleEngine.Tests.Rules
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
            // call Equals method on Name string object
            // compiles to: Param_0.Name.Equals("Game 1", CurrentCultureIgnoreCase)
            var nameEqualsRule = new MethodCallRule<Game, bool>
            {
                ObjectToCallMethodOn = "Name",
                MethodToCall = "Equals",
                Inputs = { input1, StringComparison.CurrentCultureIgnoreCase }
            };

            var compileResult = nameEqualsRule.Compile();
            compileResult.Should().BeTrue();
            _testOutcomeHelper.WriteLine($"{nameof(nameEqualsRule)}:{Environment.NewLine}{nameEqualsRule.ExpressionDebugView()}");

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
            // call Equals method on Name string object
            // compiles to: Param_0.Name.Equals("Game 1", CurrentCultureIgnoreCase)
            var nameEqualsRule = new MethodCallRule<Game, bool>
            {
                ObjectToCallMethodOn = "Name",
                MethodToCall = "Equals",
                Inputs = { new ConstantRule<string> { Value = input1 }, new ConstantRule<StringComparison> { Value = "CurrentCultureIgnoreCase" } }
            };

            var compileResult = nameEqualsRule.Compile();
            compileResult.Should().BeTrue();
            _testOutcomeHelper.WriteLine($"{nameof(nameEqualsRule)}:{Environment.NewLine}{nameEqualsRule.ExpressionDebugView()}");

            var executeResult = nameEqualsRule.Execute(_game1);
            executeResult.Should().Be(expectedResult);

            executeResult = nameEqualsRule.Execute(_game2);
            executeResult.Should().Be(!expectedResult);
        }

        [Fact]
        public void CallAVoidMethod()
        {
            // call FlipActive method on the game object
            // compiles to: Param_0.FlipActive()
            var playerCountRule = new MethodVoidCallRule<Game>
            {
                MethodToCall = "FlipActive"
            };

            var compileResult = playerCountRule.Compile();
            compileResult.Should().BeTrue();
            _testOutcomeHelper.WriteLine($"{nameof(playerCountRule)}:{Environment.NewLine}{playerCountRule.ExpressionDebugView()}");

            var currentActiveState = _game1.Active;
            playerCountRule.Execute(_game1);
            _game1.Active.Should().Be(!currentActiveState);
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(1000, false)]
        public void CheckToSeeIfPlayerExistsInAGame(int id, bool expectedResult)
        {
            // call HasPlayer method on the game object
            // compiles to: Param_0.HasPlayer(1000)
            var gameHasPlayerWithCertainId = new MethodCallRule<Game, bool>
            {
                MethodToCall = "HasPlayer",
                Inputs = {id}
            };

            var compileResult = gameHasPlayerWithCertainId.Compile();
            compileResult.Should().BeTrue();
            _testOutcomeHelper.WriteLine($"{nameof(gameHasPlayerWithCertainId)}:{Environment.NewLine}{gameHasPlayerWithCertainId.ExpressionDebugView()}");

            var executeResult = gameHasPlayerWithCertainId.Execute(_game1);
            executeResult.Should().Be(expectedResult);
        }

        [Fact]
        public void CallAStringMethodOnDescriptionObject()
        {
            // Description is a string - Call Contains method on Description
            // compiles to: Param_0.Description.Contains("cool")
            var gameNameContainsKeyWrodCool = new MethodCallRule<Game, bool>
            {
                MethodToCall = "Contains",
                ObjectToCallMethodOn = "Description",
                Inputs = { new ConstantRule<string> { Value = "cool" } }
            };

            var compileResult = gameNameContainsKeyWrodCool.Compile();
            compileResult.Should().BeTrue();
            _testOutcomeHelper.WriteLine($"{nameof(gameNameContainsKeyWrodCool)}:{Environment.NewLine}{gameNameContainsKeyWrodCool.ExpressionDebugView()}");

            // check to see if _game1 description contains keyword "cool"
            var executeResult = gameNameContainsKeyWrodCool.Execute(_game1);
            executeResult.Should().BeFalse();

            // check to see if _game2 description contains keyword "cool"
            executeResult = gameNameContainsKeyWrodCool.Execute(_game2);
            executeResult.Should().BeTrue();
        }
    }
}