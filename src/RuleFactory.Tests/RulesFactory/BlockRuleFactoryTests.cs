using FluentAssertions;
using RuleEngine.Rules;
using RuleFactory.RulesFactory;
using System;
using System.Collections.Generic;
using ModelForUnitTests;
using Newtonsoft.Json;
using RuleEngine.Common;
using Xunit;
using Xunit.Abstractions;
using ConstantRulesFactory = RuleFactory.RulesFactory.ConstantRulesFactory;

namespace RuleFactory.Tests.RulesFactory
{
    public class BlockRuleFactoryTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public BlockRuleFactoryTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void UpdateMultiplePropertiesOfaGameObjectUsingFactory()
        {
            var nameConstRule = ConstantRulesFactory.CreateConstantRule<string>("some fancy name");
            var nameChangeRule = UpdateValueRulesFactory.CreateUpdateValueRule<Game>(g => g.Name, nameConstRule);
            var rankConstRule = ConstantRulesFactory.CreateConstantRule<int>("1000");
            var rankingChangeRule = UpdateValueRulesFactory.CreateUpdateValueRule<Game>(g => g.Ranking, rankConstRule);
            var descConstRule = ConstantRulesFactory.CreateConstantRule<string>("some cool description");
            var descriptionChangeRule = UpdateValueRulesFactory.CreateUpdateValueRule<Game>(g=>g.Description, descConstRule);

            IList<Rule> rules = new List<Rule>
            {
                nameChangeRule,
                rankingChangeRule,
                descriptionChangeRule
            };
            var blockRule = BlockRulesFactory.CreateActionBlockRule<Game>(rules);

            var compileResult = blockRule.Compile();
            compileResult.Should().BeTrue();

            _testOutputHelper.WriteLine(blockRule.ExpressionDebugView());

            var game = new Game();
            blockRule.Execute(game);
            _testOutputHelper.WriteLine($"game object updated:{Environment.NewLine}{game}");
            game.Name.Should().Be("some fancy name");
            game.Ranking.Should().Be(1000);
            game.Description.Should().Be("some cool description");
        }

        [Fact]
        public void ConditionalRuleWithBlockUsingFactory()
        {
            var sourceNameRule = ConstantRulesFactory.CreateConstantRule<string>("some fancy name");
            var nameChangeRule = UpdateValueRulesFactory.CreateUpdateValueRule<Game>(g => g.Name, sourceNameRule);

            var sourceRankRule = ConstantRulesFactory.CreateConstantRule<int>("1000");
            var rankingChangeRule = UpdateValueRulesFactory.CreateUpdateValueRule<Game>(g => g.Ranking, sourceRankRule);

            var sourceDescRule = ConstantRulesFactory.CreateConstantRule<string>("some cool description");
            var descriptionChangeRule = UpdateValueRulesFactory.CreateUpdateValueRule<Game>(g=>g.Description, sourceDescRule);
 
            var subRules = new List<Rule>
            {
                nameChangeRule,
                rankingChangeRule,
                descriptionChangeRule
            };
            var blockRule = BlockRulesFactory.CreateActionBlockRule<Game>(subRules);

            var param1Const = ConstantRulesFactory.CreateConstantRule<string>("some name");
            var param2Const = ConstantRulesFactory.CreateConstantRule<StringComparison>("CurrentCultureIgnoreCase");
            var nameEqualsRule = MethodCallRulesFactory.CreateMethodCallRule<Game, bool>("Equals", null, (g => g.Name),
                new List<Rule> { param1Const, param2Const });

            var conditionalUpdateValue =
                ConditionalRulesFactory.CreateConditionalIfThActionRule<Game>(nameEqualsRule, blockRule);

            var compileResult = conditionalUpdateValue.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(conditionalUpdateValue)}:{Environment.NewLine}" +
                                        $"{conditionalUpdateValue.ExpressionDebugView()}");

            var game = new Game { Name = "some name" };
            _testOutputHelper.WriteLine($"before game.Name: {game.Name}");
            conditionalUpdateValue.Execute(game);
            _testOutputHelper.WriteLine($"after game.Name: {game.Name}");
            game.Name.Should().Be("some fancy name");
            _testOutputHelper.WriteLine($"{game}");

            var jsonConverterForRule = new JsonConverterForRule();
            var json = JsonConvert.SerializeObject(conditionalUpdateValue, jsonConverterForRule);
            _testOutputHelper.WriteLine(json);

            var conditionalUpdateValue2 = JsonConvert.DeserializeObject<Rule>(json, jsonConverterForRule);
            compileResult = conditionalUpdateValue2.Compile();
            compileResult.Should().BeTrue();

            var game2 = new Game { Name = "some name" };
            _testOutputHelper.WriteLine($"before game2.Name: {game2.Name}");
            (conditionalUpdateValue2 as ConditionalIfThActionRule<Game>)?.Execute(game2);
            _testOutputHelper.WriteLine($"after game2.Name: {game2.Name}");
            game.Name.Should().Be("some fancy name");
            _testOutputHelper.WriteLine($"{game2}");
        }

        [Fact]
        public void EmptyBlockRuleThrowsExceptionUsingFactory()
        {
            var emptyBlockRule = BlockRulesFactory.CreateFuncBlockRule<object, object>();
            var exception = Assert.Throws<RuleEngineException>(() => emptyBlockRule.Compile());
            exception.Message.Should().Be("last rule must return a value of System.Object");
        }

        [Fact]
        public void ExceptionWhenLastRuleReturnsNoValueUsingFactory()
        {
            var someRule = new ConditionalIfThActionRule<object>();
            var someBlockRule = BlockRulesFactory.CreateFuncBlockRule<object, object>(new List<Rule> {someRule});
            var exception = Assert.Throws<RuleEngineException>(() => someBlockRule.Compile());
            exception.Message.Should().Be("last rule must return a value of System.Object");
        }

        [Fact]
        public void FuncBlockRuleReturnsLastRuleResultUsingFactory()
        {
            var ruleReturning5 = ConstantRulesFactory.CreateConstantRule<int, int>("5");
            var blockRule = BlockRulesFactory.CreateFuncBlockRule<int, int>(new List<Rule> {ruleReturning5});
            var compileResult = blockRule.Compile();
            compileResult.Should().BeTrue();

            var five = blockRule.Execute(99);
            five.Should().Be(5);
        }

        [Fact]
        public void ReturnsUpdatedGameUsingFactory()
        {
            var sourceNameRule = ConstantRulesFactory.CreateConstantRule<string>("some fancy name");
            var nameChangeRule = UpdateValueRulesFactory.CreateUpdateValueRule<Game>(g => g.Name, sourceNameRule);

            var sourceRankRule = ConstantRulesFactory.CreateConstantRule<int>("1000");
            var rankingChangeRule = UpdateValueRulesFactory.CreateUpdateValueRule<Game>(g => g.Ranking, sourceRankRule);

            var sourceDescRule = ConstantRulesFactory.CreateConstantRule<string>("some cool description");
            var descriptionChangeRule = UpdateValueRulesFactory.CreateUpdateValueRule<Game>(g=>g.Description, sourceDescRule);

            var selfReturnRule = new SelfReturnRule<Game>();

            var subRules = new List<Rule>
            {
                nameChangeRule,
                rankingChangeRule,
                descriptionChangeRule,
                selfReturnRule
            };

            var blockRule = BlockRulesFactory.CreateFuncBlockRule<Game, Game>(subRules);

            var compileResult = blockRule.Compile();
            compileResult.Should().BeTrue();

            var game = blockRule.Execute(new Game());
            game.Name.Should().Be("some fancy name");
            game.Ranking.Should().Be(1000);
            game.Description.Should().Be("some cool description");
            _testOutputHelper.WriteLine($"{game}");
        }

        [Fact]
        public void ReturnsNewOrUpdatedGameUsingFactory()
        {
            var nullGame = ConstantRulesFactory.CreateConstantRule<Game>("null");
            var nullGameCheckRule =
                ValidationRulesFactory.CreateValidationRule<Game>(LogicalOperatorAtTheRootLevel.Equal, nullGame);

            var newGameRule = MethodCallRulesFactory.CreateStaticMethodCallRule<Game>("CreateGame", "ModelForUnitTests.Game", null);

            var selfReturnRule = SelfReturnRuleFactory.CreateSelfReturnRule<Game>();
            var gameObjectRule = ConditionalRulesFactory.CreateConditionalFuncRule<Game, Game>(nullGameCheckRule, newGameRule,
                    selfReturnRule);

            var assignRule = UpdateValueRulesFactory.CreateUpdateValueRule<Game>(gameObjectRule);

            var nameConstRule = ConstantRulesFactory.CreateConstantRule<string>("some fancy name");
            var nameChangeRule = UpdateValueRulesFactory.CreateUpdateValueRule<Game>(g => g.Name, nameConstRule);
            var rankConstRule = ConstantRulesFactory.CreateConstantRule<int>("1000");
            var rankingChangeRule = UpdateValueRulesFactory.CreateUpdateValueRule<Game>(g => g.Ranking, rankConstRule);
            var descConstRule = ConstantRulesFactory.CreateConstantRule<string>("some cool description");
            var descriptionChangeRule = UpdateValueRulesFactory.CreateUpdateValueRule<Game>(g=>g.Description, descConstRule);

            IList<Rule> rules = new List<Rule>
            {
                assignRule,
                nameChangeRule,
                rankingChangeRule,
                descriptionChangeRule,
                selfReturnRule
            };
            var blockRule = BlockRulesFactory.CreateFuncBlockRule<Game, Game>(rules);

            var compileResult = blockRule.Compile();
            compileResult.Should().BeTrue();

            var game = blockRule.Execute(null);
            game.Name.Should().Be("some fancy name");
            game.Ranking.Should().Be(1000);
            game.Description.Should().Be("some cool description");
            game.Rating.Should().BeNullOrEmpty();
            _testOutputHelper.WriteLine($"{game}");

            var newGame = new Game { Rating = "high" };
            // newGame is not same as game object
            ReferenceEquals(game, newGame).Should().BeFalse();
            game = blockRule.Execute(newGame);
            // this call shall return the same newGame object with updated values
            ReferenceEquals(game, newGame).Should().BeTrue();
            game.Rating.Should().Be("high");
            _testOutputHelper.WriteLine($"newGame: {game}");
        }
    }
}
