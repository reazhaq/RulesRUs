using System;
using FluentAssertions;
using RuleEngine.Common;
using RuleEngine.Rules;
using SampleModel;
using Xunit;
using Xunit.Abstractions;

namespace RuleEngine.Tests.Rules
{
    public class BlockRulesTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public BlockRulesTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void UpdateMultiplePropertiesOfaGameObject()
        {
            var nameChangeRule = new UpdateValueRule<Game>
            {
                ObjectToUpdate = "Name",
                SourceDataRule = new ConstantRule<string> {Value = "some fancy name"}
            };
            var rankingChangeRule = new UpdateValueRule<Game>
            {
                ObjectToUpdate = "Ranking",
                SourceDataRule = new ConstantRule<int>{Value = "1000"}
            };
            var descriptionChangeRule = new UpdateValueRule<Game>
            {
                ObjectToUpdate = "Description",
                SourceDataRule = new ConstantRule<string>{Value = "some cool description"}
            };

            var blockRule = new ActionBlockRule<Game>();
            blockRule.Rules.Add(nameChangeRule);
            blockRule.Rules.Add(rankingChangeRule);
            blockRule.Rules.Add(descriptionChangeRule);

            var compileResult = blockRule.Compile();
            compileResult.Should().BeTrue();

            _testOutputHelper.WriteLine(blockRule.ExpressionDebugView());

            var game = new Game();
            blockRule.Execute(game);
            _testOutputHelper.WriteLine($"game object updated:{Environment.NewLine}{game}");
        }

        [Fact]
        public void ConditionalRuleWithBlock()
        {
            var nameChangeRule = new UpdateValueRule<Game>
            {
                ObjectToUpdate = "Name",
                SourceDataRule = new ConstantRule<string> {Value = "some fancy name"}
            };
            var rankingChangeRule = new UpdateValueRule<Game>
            {
                ObjectToUpdate = "Ranking",
                SourceDataRule = new ConstantRule<int>{Value = "1000"}
            };
            var descriptionChangeRule = new UpdateValueRule<Game>
            {
                ObjectToUpdate = "Description",
                SourceDataRule = new ConstantRule<string>{Value = "some cool description"}
            };

            var blockRule = new ActionBlockRule<Game>();
            blockRule.Rules.Add(nameChangeRule);
            blockRule.Rules.Add(rankingChangeRule);
            blockRule.Rules.Add(descriptionChangeRule);

            var conditionalUpdateValue = new ConditionalIfThActionRule<Game>
            {
                ConditionRule = new MethodCallRule<Game, bool>
                {
                    ObjectToCallMethodOn = "Name",
                    MethodToCall = "Equals",
                    MethodParameters = { new ConstantRule<string> { Value = "some name" }, 
                            new ConstantRule<StringComparison> { Value = "CurrentCultureIgnoreCase" }
                    }
                },
                TrueRule = blockRule
            };

            var compileResult = conditionalUpdateValue.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(conditionalUpdateValue)}:{Environment.NewLine}" +
                                        $"{conditionalUpdateValue.ExpressionDebugView()}");

            var game = new Game { Name = "some name" };
            _testOutputHelper.WriteLine($"before game.Name: {game.Name}");
            conditionalUpdateValue.Execute(game);
            _testOutputHelper.WriteLine($"after game.Name: {game.Name}");
            game.Name.Should().Be("some fancy name");
            game.Ranking.Should().Be(1000);
            game.Description.Should().Be("some cool description");
            _testOutputHelper.WriteLine($"{game}");
        }

        [Fact]
        public void EmptyBlockRuleThrowsException()
        {
            var emptyBlockRule = new FuncBlockRule<object, object>();
            var exception = Assert.Throws<RuleEngineException>(() => emptyBlockRule.Compile());
            exception.Message.Should().Be("last rule must return a value of System.Object");
        }

        [Fact]
        public void ExceptionWhenLastRuleReturnsNoValue()
        {
            var someRule = new ConditionalIfThActionRule<object>();
            var someBlockRule = new FuncBlockRule<object, object>();
            someBlockRule.Rules.Add(someRule);
            var exception = Assert.Throws<RuleEngineException>(() => someBlockRule.Compile());
            exception.Message.Should().Be("last rule must return a value of System.Object");
        }

        [Fact]
        public void FuncBlockRuleReturnsLastRuleResult()
        {
            var ruleReturning5 = new ConstantRule<int, int> {Value = "5"};
            var blockRule = new FuncBlockRule<int,int>();
            blockRule.Rules.Add(ruleReturning5);
            var compileResult = blockRule.Compile();
            compileResult.Should().BeTrue();

            var five = blockRule.Execute(99);
            five.Should().Be(5);
        }

        [Fact]
        public void ReturnsUpdatedGame()
        {
            var nameChangeRule = new UpdateValueRule<Game>
            {
                ObjectToUpdate = "Name",
                SourceDataRule = new ConstantRule<string> {Value = "some fancy name"}
            };
            var rankingChangeRule = new UpdateValueRule<Game>
            {
                ObjectToUpdate = "Ranking",
                SourceDataRule = new ConstantRule<int>{Value = "1000"}
            };
            var descriptionChangeRule = new UpdateValueRule<Game>
            {
                ObjectToUpdate = "Description",
                SourceDataRule = new ConstantRule<string>{Value = "some cool description"}
            };
            var selfReturnRule = new SelfReturnRule<Game>();

            var blockRule = new FuncBlockRule<Game, Game>();
            blockRule.Rules.Add(nameChangeRule);
            blockRule.Rules.Add(rankingChangeRule);
            blockRule.Rules.Add(descriptionChangeRule);
            blockRule.Rules.Add(selfReturnRule);

            var compileResult = blockRule.Compile();
            compileResult.Should().BeTrue();

            var game = blockRule.Execute(new Game());
            game.Name.Should().Be("some fancy name");
            game.Ranking.Should().Be(1000);
            game.Description.Should().Be("some cool description");
            _testOutputHelper.WriteLine($"{game}");
        }

        [Fact]
        public void ReturnsNewOrUpdatedGame()
        {
            var nullGame = new ConstantRule<Game>{Value = "null"};
            var nullGameCheckRule = new ValidationRule<Game>
            {
                ValueToValidateAgainst = nullGame,
                OperatorToUse = "Equal"
            };
            //var compileResult = nullGameCheckRule.Compile();
            //compileResult.Should().BeTrue();

            var newGameRule = new StaticMethodCallRule<Game>
            {
                MethodClassName = "SampleModel.Game",
                MethodToCall = "CreateGame"
            };

            var gameObjectRule = new ConditionalFuncRule<Game, Game>
            {
                ConditionRule = nullGameCheckRule,
                TrueRule = newGameRule,
                FalseRule = new SelfReturnRule<Game>()
            };
            //var compileResult = gameObjectRule.Compile();
            //compileResult.Should().BeTrue();

            //var game = gameObjectRule.Execute(null);

            //game.Description = "blah blah blah";
            //var foo = gameObjectRule.Execute(game);
            //object.ReferenceEquals(game, foo).Should().BeTrue();

            var nameChangeRule = new UpdateValueRule<Game>
            {
                ObjectToUpdate = "Name",
                SourceDataRule = new ConstantRule<string> {Value = "some fancy name"}
            };
            var rankingChangeRule = new UpdateValueRule<Game>
            {
                ObjectToUpdate = "Ranking",
                SourceDataRule = new ConstantRule<int>{Value = "1000"}
            };
            var descriptionChangeRule = new UpdateValueRule<Game>
            {
                ObjectToUpdate = "Description",
                SourceDataRule = new ConstantRule<string>{Value = "some cool description"}
            };
            var selfReturnRule = new SelfReturnRule<Game>();

            var blockRule = new FuncBlockRule<Game, Game>();
            blockRule.Rules.Add(gameObjectRule);
            blockRule.Rules.Add(nameChangeRule);
            blockRule.Rules.Add(rankingChangeRule);
            blockRule.Rules.Add(descriptionChangeRule);
            blockRule.Rules.Add(selfReturnRule);

            var compileResult = blockRule.Compile();
            compileResult.Should().BeTrue();

            //var game = blockRule.Execute(null);
            var game = blockRule.Execute(new Game());
            game.Name.Should().Be("some fancy name");
            game.Ranking.Should().Be(1000);
            game.Description.Should().Be("some cool description");
            _testOutputHelper.WriteLine($"{game}");
        }
    }
}