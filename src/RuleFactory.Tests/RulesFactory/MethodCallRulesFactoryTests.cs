using System;
using System.Collections.Generic;
using FluentAssertions;
using ModelForUnitTests;
using RuleEngine.Rules;
using RuleFactory.RulesFactory;
using RuleFactory.Tests.Fixture;
using Xunit;
using Xunit.Abstractions;

namespace RuleFactory.Tests.RulesFactory
{
    public class MethodCallRulesFactoryTests: IClassFixture<ExpressionRulesFixture>
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly Game _game1;
        private readonly Game _game2;

        public MethodCallRulesFactoryTests(ExpressionRulesFixture expressionRuleFixture, ITestOutputHelper testOutputHelper)
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
        public void CallEqualsMethodOnNameUsingConstantRuleUsingFactory(string param1, bool expectedResult)
        {
            // call Equals method on Name string object
            // compiles to: Param_0.Name.Equals("Game 1", CurrentCultureIgnoreCase)
            var param1Const = ConstantRulesFactory.CreateConstantRule<string>(param1);
            var param2Const = ConstantRulesFactory.CreateConstantRule<StringComparison>("CurrentCultureIgnoreCase");
            var nameEqualsRule = MethodCallRulesFactory.CreateMethodCallRule<Game, bool>("Equals", null, (g => g.Name),
                new List<Rule> { param1Const, param2Const });
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
        public void CallAVoidMethodUsingFactory()
        {
            // call FlipActive method on the game object
            // compiles to: Param_0.FlipActive()
            var playerCountRule = MethodCallRulesFactory.CreateMethodVoidCallRule<Game>("FlipActive", null, null, null);

            var compileResult = playerCountRule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(playerCountRule)}:{Environment.NewLine}" +
                                        $"{playerCountRule.ExpressionDebugView()}");

            var currentActiveState = _game1.Active;
            playerCountRule.Execute(_game1);
            _game1.Active.Should().Be(!currentActiveState);
        }

        [Fact]
        public void CallToUpperUsingFactory()
        {
            var rule = MethodCallRulesFactory.CreateMethodCallRule<string,string>("ToUpper", "System.String", null, null);
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
        public void CheckToSeeIfPlayerExistsInAGameUsingFactory(int id, bool expectedResult)
        {
            // call HasPlayer method on the game object
            // compiles to: Param_0.HasPlayer(1000)
            var const1 = ConstantRulesFactory.CreateConstantRule<int>(id.ToString());
            var gameHasPlayerWithCertainId =
                MethodCallRulesFactory.CreateMethodCallRule<Game, bool>("HasPlayer", null, null,
                    new List<Rule> {const1});

            var compileResult = gameHasPlayerWithCertainId.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(gameHasPlayerWithCertainId)}:{Environment.NewLine}" +
                                        $"{gameHasPlayerWithCertainId.ExpressionDebugView()}");

            var executeResult = gameHasPlayerWithCertainId.Execute(_game1);
            executeResult.Should().Be(expectedResult);
        }

        [Fact]
        public void CallAStringMethodOnDescriptionObjectUsingFactory()
        {
            // Description is a string - Call Contains method on Description
            // compiles to: Param_0.Description.Contains("cool")
            var const1 = ConstantRulesFactory.CreateConstantRule<string>("cool");
            var gameNameContainsKeyWordCool = MethodCallRulesFactory.CreateMethodCallRule<Game, bool>("Contains",
                                null, (g => g.Description), new List<Rule> {const1});

            var compileResult = gameNameContainsKeyWordCool.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(gameNameContainsKeyWordCool)}:{Environment.NewLine}" +
                                        $"{gameNameContainsKeyWordCool.ExpressionDebugView()}");

            // check to see if _game1 description contains keyword "cool"
            var executeResult = gameNameContainsKeyWordCool.Execute(_game1);
            executeResult.Should().BeFalse();

            // check to see if _game2 description contains keyword "cool"
            executeResult = gameNameContainsKeyWordCool.Execute(_game2);
            executeResult.Should().BeTrue();
        }

        [Fact]
        public void CallCreateGameStaticMethodUsingFactory()
        {
            //var game = Game.CreateGame();
            var rule = MethodCallRulesFactory.CreateStaticMethodCallRule<Game>("CreateGame", "ModelForUnitTests.Game", null);

            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"rule: {Environment.NewLine}" +
                                        $"{rule.ExpressionDebugView()}");

            var game = rule.Execute();
            game.Should().NotBeNull();
        }

        [Fact]
        public void CallCreateGameStaticMethod2UsingFactory()
        {
            //var game = Game.CreateGame("cool game");
            var rule = MethodCallRulesFactory.CreateStaticMethodCallRule<Game>("CreateGame", "ModelForUnitTests.Game", 
                                                        new List<Rule>{new ConstantRule<string> {Value = "cool game"}});

            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"rule: {Environment.NewLine}" +
                                        $"{rule.ExpressionDebugView()}");

            var game = rule.Execute();
            game.Should().NotBeNull();
            game.Name.Should().Be("cool game");
        }

        [Fact]
        public void CallCreateGameStaticMethod3UsingFactory()
        {
            //var game = Game.CreateGame("game", "description", 1, true);
            var methodParams = new List<Rule>
            {
                ConstantRulesFactory.CreateConstantRule<string>("game"),
                ConstantRulesFactory.CreateConstantRule<string>("description"),
                ConstantRulesFactory.CreateConstantRule<int>("1"),
                ConstantRulesFactory.CreateConstantRule<bool>("true")
            };
            var rule = MethodCallRulesFactory.CreateStaticMethodCallRule<Game>("CreateGame",
                                            "ModelForUnitTests.Game", methodParams);

            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"rule: {Environment.NewLine}" +
                                        $"{rule.ExpressionDebugView()}");

            var game = rule.Execute();
            game.Should().NotBeNull();
            game.Name.Should().Be("game");
            _testOutputHelper.WriteLine($"{game}");
        }

        [Fact]
        public void CallStaticVoidMethodUsingFactory()
        {
            var rule = MethodCallRulesFactory.CreateStaticVoidMethodCallRule("SomeVoidStaticMethod", "ModelForUnitTests.Game", null);
 
            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"rule: {Environment.NewLine}" +
                                        $"{rule.ExpressionDebugView()}");

            Game.SomeStaticIntValue = 0;
            rule.Execute();
            Game.SomeStaticIntValue.Should().Be(1);
        }

        [Fact]
        public void CallStaticVoidMethod2UsingFactory()
        {
            var rule = MethodCallRulesFactory.CreateStaticVoidMethodCallRule("SomeVoidStaticMethod", "ModelForUnitTests.Game", 
                                                                            new List<Rule>{new ConstantRule<int> {Value = "99"}});

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