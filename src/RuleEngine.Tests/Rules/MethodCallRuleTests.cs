using System;
using FluentAssertions;
using RuleEngine.Rules;
using RuleEngine.Tests.Fixture;
using SampleModel;
using Xunit;
using Xunit.Abstractions;

namespace RuleEngine.Tests.Rules
{
    public class MethodCallRuleTests : IClassFixture<ExpressionRulesFixture>
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly Game _game1;
        private readonly Game _game2;

        public MethodCallRuleTests(ExpressionRulesFixture expressionRuleFixture, ITestOutputHelper testOutputHelper)
        {
            _game1 = expressionRuleFixture.Game1;
            _game2 = expressionRuleFixture.Game2;
            _testOutputHelper = testOutputHelper;
        }
        
        [Theory]
        [InlineData("Game 1", true)]
        [InlineData("game 1", true)]
        [InlineData("game 2", false)]
        [InlineData("gaMe 2", false)]
        public void CallEqualsMethodOnNameUsingConstantRule(string param1, bool expectedResult)
        {
            // call Equals method on Name string object
            // compiles to: Param_0.Name.Equals("Game 1", CurrentCultureIgnoreCase)
            var nameEqualsRule = new MethodCallRule<Game, bool>
            {
                ObjectToCallMethodOn = "Name",
                MethodToCall = "Equals",
                MethodParameters = { new ConstantRule<string> { Value = param1 },
                    new ConstantRule<StringComparison> { Value = "CurrentCultureIgnoreCase" }
                }
            };

            var compileResult = nameEqualsRule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(nameEqualsRule)}:{Environment.NewLine}" +
                                        $"{nameEqualsRule.ExpressionDebugView()}");

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
            var rule = new MethodVoidCallRule<Game>
            {
                MethodToCall = "FlipActive"
            };

            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(rule)}:{Environment.NewLine}" +
                                        $"{rule.ExpressionDebugView()}");

            var currentActiveState = _game1.Active;
            rule.Execute(_game1);
            _game1.Active.Should().Be(!currentActiveState);
        }

        [Fact]
        public void CallToUpper()
        {
            var rule = new MethodCallRule<string, string>{MethodToCall = "ToUpper"};
            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(rule)}:{Environment.NewLine}" +
                                        $"{rule.ExpressionDebugView()}");

            var foo = "foo";
            var FOO = rule.Execute(foo);
            FOO.Should().Be("FOO");
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
                MethodParameters = {new ConstantRule<int>{Value = id.ToString()}}
            };

            var compileResult = gameHasPlayerWithCertainId.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(gameHasPlayerWithCertainId)}:{Environment.NewLine}" +
                                        $"{gameHasPlayerWithCertainId.ExpressionDebugView()}");

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
                MethodParameters = { new ConstantRule<string> { Value = "cool" } }
            };

            var compileResult = gameNameContainsKeyWrodCool.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(gameNameContainsKeyWrodCool)}:{Environment.NewLine}" +
                                        $"{gameNameContainsKeyWrodCool.ExpressionDebugView()}");

            // check to see if _game1 description contains keyword "cool"
            var executeResult = gameNameContainsKeyWrodCool.Execute(_game1);
            executeResult.Should().BeFalse();

            // check to see if _game2 description contains keyword "cool"
            executeResult = gameNameContainsKeyWrodCool.Execute(_game2);
            executeResult.Should().BeTrue();
        }

        [Fact]
        public void CallCreateGameStaticMethod()
        {
            //var game = Game.CreateGame();
            var rule = new StaticMethodCallRule<Game>
            {
                MethodClassName = "SampleModel.Game",
                MethodToCall = "CreateGame"
            };

            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"rule: {Environment.NewLine}" +
                                        $"{rule.ExpressionDebugView()}");

            var game = rule.Execute();
            game.Should().NotBeNull();
        }

        [Fact]
        public void CallCreateGameStaticMethod2()
        {
            //var game = Game.CreateGame("cool game");
            var rule = new StaticMethodCallRule<Game>
            {
                MethodClassName = "SampleModel.Game",
                MethodToCall = "CreateGame",
                MethodParameters = { new ConstantRule<string> { Value = "cool game" } }
            };

            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"rule: {Environment.NewLine}" +
                                        $"{rule.ExpressionDebugView()}");

            var game = rule.Execute();
            game.Should().NotBeNull();
            game.Name.Should().Be("cool game");
        }

        [Fact]
        public void CallStaticVoidMethod()
        {
            var rule = new StaticVoidMethodCallRule
            {
                MethodClassName = "SampleModel.Game",
                MethodToCall = "SomeVoidStaticMethod"
            };

            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"rule: {Environment.NewLine}" +
                                        $"{rule.ExpressionDebugView()}");

            Game.SomeStaticIntValue = 0;
            rule.Execute();
            Game.SomeStaticIntValue.Should().Be(1);
        }

        [Fact]
        public void CallStaticVoidMethod2()
        {
            var rule = new StaticVoidMethodCallRule
            {
                MethodClassName = "SampleModel.Game",
                MethodToCall = "SomeVoidStaticMethod",
                MethodParameters = {new ConstantRule<int> {Value = "99"}}
            };

            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"rule: {Environment.NewLine}" +
                                        $"{rule.ExpressionDebugView()}");

            Game.SomeStaticIntValue = 0;
            rule.Execute();
            Game.SomeStaticIntValue.Should().Be(99);
        }
    }
}