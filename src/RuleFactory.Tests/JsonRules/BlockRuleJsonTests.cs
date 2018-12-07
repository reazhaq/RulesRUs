using System;
using FluentAssertions;
using ModelForUnitTests;
using Newtonsoft.Json;
using RuleEngine.Common;
using RuleEngine.Rules;
using Xunit;
using Xunit.Abstractions;

namespace RuleFactory.Tests.JsonRules
{
    public class BlockRuleJsonTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public BlockRuleJsonTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void UpdateMultiplePropertiesOfaGameObject()
        {
            var nameChangeRule = new UpdateValueRule<Game>
            {
                ObjectToUpdate = "Name",
                SourceDataRule = new ConstantRule<string> { Value = "some fancy name" }
            };
            var rankingChangeRule = new UpdateValueRule<Game>
            {
                ObjectToUpdate = "Ranking",
                SourceDataRule = new ConstantRule<int> { Value = "1000" }
            };
            var descriptionChangeRule = new UpdateValueRule<Game>
            {
                ObjectToUpdate = "Description",
                SourceDataRule = new ConstantRule<string> { Value = "some cool description" }
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
            game.Name.Should().Be("some fancy name");
            game.Ranking.Should().Be(1000);
            game.Description.Should().Be("some cool description");

            // convert to json
            var ruleJson = JsonConvert.SerializeObject(blockRule, new JsonConverterForRule());
            _testOutputHelper.WriteLine($"{nameof(ruleJson)}:{Environment.NewLine}{ruleJson}");
            // re-hydrate from json
            var ruleFromJson = (ActionBlockRule<Game>)JsonConvert.DeserializeObject<Rule>(ruleJson, new JsonConverterForRule());
            var compileResult2 = ruleFromJson.Compile();
            compileResult2.Should().BeTrue();
            _testOutputHelper.WriteLine(ruleFromJson.ExpressionDebugView());

            var game2 = new Game();
            ruleFromJson.Execute(game2);
            _testOutputHelper.WriteLine($"game object updated:{Environment.NewLine}{game2}");
            game2.Name.Should().Be("some fancy name");
            game2.Ranking.Should().Be(1000);
            game2.Description.Should().Be("some cool description");
        }

        [Fact]
        public void ConditionalRuleWithBlock()
        {
            var nameChangeRule = new UpdateValueRule<Game>
            {
                ObjectToUpdate = "Name",
                SourceDataRule = new ConstantRule<string> { Value = "some fancy name" }
            };
            var rankingChangeRule = new UpdateValueRule<Game>
            {
                ObjectToUpdate = "Ranking",
                SourceDataRule = new ConstantRule<int> { Value = "1000" }
            };
            var descriptionChangeRule = new UpdateValueRule<Game>
            {
                ObjectToUpdate = "Description",
                SourceDataRule = new ConstantRule<string> { Value = "some cool description" }
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

            var converter = new JsonConverterForRule();
            // convert to json
            var ruleJson = JsonConvert.SerializeObject(conditionalUpdateValue, converter);
            _testOutputHelper.WriteLine($"{nameof(ruleJson)}:{Environment.NewLine}{ruleJson}");
            // re-hydrate from json
            var ruleFromJson = JsonConvert.DeserializeObject<ConditionalIfThActionRule<Game>>(ruleJson, converter);

            var compileResult2 = ruleFromJson.Compile();
            compileResult2.Should().BeTrue();

            var game2 = new Game {Name = "some name"};
            _testOutputHelper.WriteLine($"before game2.Name: {game2.Name}");
            ruleFromJson.Execute(game2);
            _testOutputHelper.WriteLine($"after game2.Name: {game2.Name}");
            game2.Name.Should().Be("some fancy name");
            game2.Ranking.Should().Be(1000);
            game2.Description.Should().Be("some cool description");
            _testOutputHelper.WriteLine($"{game2}");
        }

        [Fact]
        public void EmptyBlockRuleThrowsException()
        {
            var emptyBlockRule = new FuncBlockRule<object, object>();
            var exception = Assert.Throws<RuleEngineException>(() => emptyBlockRule.Compile());
            exception.Message.Should().Be("last rule must return a value of System.Object");

            var converter = new JsonConverterForRule();
            // convert to json
            var ruleJson = JsonConvert.SerializeObject(emptyBlockRule, converter);
            _testOutputHelper.WriteLine(ruleJson);
            // re-hydrate from json
            var ruleFromJson = JsonConvert.DeserializeObject<FuncBlockRule<object, object>>(ruleJson, converter);

            var exception2 = Assert.Throws<RuleEngineException>(() => ruleFromJson.Compile());
            exception2.Message.Should().Be("last rule must return a value of System.Object");
        }

        [Fact]
        public void ExceptionWhenLastRuleReturnsNoValue()
        {
            var someRule = new ConditionalIfThActionRule<object>();
            var someBlockRule = new FuncBlockRule<object, object>();
            someBlockRule.Rules.Add(someRule);
            var exception = Assert.Throws<RuleEngineException>(() => someBlockRule.Compile());
            exception.Message.Should().Be("last rule must return a value of System.Object");

            var converter = new JsonConverterForRule();
            // convert to json
            var ruleJson = JsonConvert.SerializeObject(someBlockRule, converter);
            _testOutputHelper.WriteLine(ruleJson);
            // re-hydrate from json
            var ruleFromJson = JsonConvert.DeserializeObject<FuncBlockRule<object, object>>(ruleJson, converter);

            var exception2 = Assert.Throws<RuleEngineException>(() => ruleFromJson.Compile());
            exception2.Message.Should().Be("last rule must return a value of System.Object");
        }

        [Fact]
        public void FuncBlockRuleReturnsLastRuleResult()
        {
            var ruleReturning5 = new ConstantRule<int, int> { Value = "5" };
            var blockRule = new FuncBlockRule<int, int>();
            blockRule.Rules.Add(ruleReturning5);
            var compileResult = blockRule.Compile();
            compileResult.Should().BeTrue();

            var five = blockRule.Execute(99);
            five.Should().Be(5);

            var converter = new JsonConverterForRule();
            // convert to json
            var ruleJson = JsonConvert.SerializeObject(blockRule, converter);
            _testOutputHelper.WriteLine(ruleJson);
            // re-hydrate from json
            var ruleFromJson = JsonConvert.DeserializeObject<FuncBlockRule<int, int>>(ruleJson, converter);
            var compileResult2 = ruleFromJson.Compile();
            compileResult2.Should().BeTrue();

            var five2 = ruleFromJson.Execute(99);
            five2.Should().Be(5);
        }

        [Fact]
        public void ReturnsUpdatedGame()
        {
            var nameChangeRule = new UpdateValueRule<Game>
            {
                ObjectToUpdate = "Name",
                SourceDataRule = new ConstantRule<string> { Value = "some fancy name" }
            };
            var rankingChangeRule = new UpdateValueRule<Game>
            {
                ObjectToUpdate = "Ranking",
                SourceDataRule = new ConstantRule<int> { Value = "1000" }
            };
            var descriptionChangeRule = new UpdateValueRule<Game>
            {
                ObjectToUpdate = "Description",
                SourceDataRule = new ConstantRule<string> { Value = "some cool description" }
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

            var jsonConverterForRule = new JsonConverterForRule();
            var json = JsonConvert.SerializeObject(blockRule, jsonConverterForRule);
            _testOutputHelper.WriteLine(json);

            var blockRule2 = JsonConvert.DeserializeObject<FuncBlockRule<Game, Game>>(json, jsonConverterForRule);
            compileResult = blockRule2.Compile();
            compileResult.Should().BeTrue();

            var game2 = blockRule2.Execute(new Game());
            game2.Name.Should().Be("some fancy name");
            game2.Ranking.Should().Be(1000);
            game2.Description.Should().Be("some cool description");
            _testOutputHelper.WriteLine($"{game2}");
        }

        [Fact]
        public void ReturnsNewOrUpdatedGame()
        {
            var nullGame = new ConstantRule<Game> { Value = "null" };
            var nullGameCheckRule = new ValidationRule<Game>
            {
                ValueToValidateAgainst = nullGame,
                OperatorToUse = "Equal"
            };

            var newGameRule = new StaticMethodCallRule<Game>
            {
                MethodClassName = "ModelForUnitTests.Game",
                MethodToCall = "CreateGame"
            };

            var selfReturnRule = new SelfReturnRule<Game>();
            var gameObjectRule = new ConditionalFuncRule<Game, Game>
            {
                ConditionRule = nullGameCheckRule,
                TrueRule = newGameRule,
                FalseRule = selfReturnRule
            };
            var assignRule = new UpdateValueRule<Game> { SourceDataRule = gameObjectRule };

            var nameChangeRule = new UpdateValueRule<Game>
            {
                ObjectToUpdate = "Name",
                SourceDataRule = new ConstantRule<string> { Value = "some fancy name" }
            };
            var rankingChangeRule = new UpdateValueRule<Game>
            {
                ObjectToUpdate = "Ranking",
                SourceDataRule = new ConstantRule<int> { Value = "1000" }
            };
            var descriptionChangeRule = new UpdateValueRule<Game>
            {
                ObjectToUpdate = "Description",
                SourceDataRule = new ConstantRule<string> { Value = "some cool description" }
            };

            var blockRule = new FuncBlockRule<Game, Game>();
            blockRule.Rules.Add(assignRule);
            blockRule.Rules.Add(nameChangeRule);
            blockRule.Rules.Add(rankingChangeRule);
            blockRule.Rules.Add(descriptionChangeRule);
            blockRule.Rules.Add(selfReturnRule);

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

            var jsonConverterForRule = new JsonConverterForRule();
            var json = JsonConvert.SerializeObject(blockRule, jsonConverterForRule);
            _testOutputHelper.WriteLine(json);

            var blockRule2 = JsonConvert.DeserializeObject<FuncBlockRule<Game, Game>>(json, jsonConverterForRule);
            compileResult = blockRule2.Compile();
            compileResult.Should().BeTrue();

            var game2 = blockRule2.Execute(new Game());
            game2.Name.Should().Be("some fancy name");
            game2.Ranking.Should().Be(1000);
            game2.Description.Should().Be("some cool description");
            _testOutputHelper.WriteLine($"{game2}");
        }
    }
}