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
    public class ValidationRulesFactoryTests : IClassFixture<ValidationRulesFixture>
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly Game _game;

        public ValidationRulesFactoryTests(ValidationRulesFixture validationRuleFixture, ITestOutputHelper testOutputHelper)
        {
            _game = validationRuleFixture.Game;
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void RuleToCheckIfAnIntegerMatchesRuleValueOrNotUsingFactory()
        {
            var constRule = ConstantRulesFactory.CreateConstantRule<int>("5");
            var numberShouldBe5Rule =
                ValidationRulesFactory.CreateValidationRule<int>(
                    LogicalOperatorAtTheRootLevel.Equal, constRule);
            var compileResult = numberShouldBe5Rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(numberShouldBe5Rule)}:{Environment.NewLine}{numberShouldBe5Rule.ExpressionDebugView()}");

            var numberShouldNotBe5Rule =
                ValidationRulesFactory.CreateValidationRule<int>(
                    LogicalOperatorAtTheRootLevel.NotEqual, constRule);
            compileResult = numberShouldNotBe5Rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(numberShouldNotBe5Rule)}:{Environment.NewLine}" +
                                        $"{numberShouldNotBe5Rule.ExpressionDebugView()}");

            var ruleExecuteResult = numberShouldBe5Rule.IsValid(5);
            ruleExecuteResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"with 5 {nameof(numberShouldBe5Rule)} execute result: {ruleExecuteResult}");

            ruleExecuteResult = numberShouldBe5Rule.IsValid(6);
            ruleExecuteResult.Should().BeFalse();

            ruleExecuteResult = numberShouldNotBe5Rule.IsValid(6);
            ruleExecuteResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"with 6 {nameof(numberShouldNotBe5Rule)} execute result: {ruleExecuteResult}");

            ruleExecuteResult = numberShouldNotBe5Rule.IsValid(5);
            ruleExecuteResult.Should().BeFalse();
        }

        [Fact]
        public void RuleToCheckIfRootObjectIsNullOrNotUsingFactory()
        {
            var constRule = ConstantRulesFactory.CreateConstantRule<Game>("null");
            var checkForNotNullRule =
                ValidationRulesFactory.CreateValidationRule<Game>(
                    LogicalOperatorAtTheRootLevel.NotEqual, constRule);
            var compileResult = checkForNotNullRule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(checkForNotNullRule)}:{Environment.NewLine}{checkForNotNullRule.ExpressionDebugView()}");


            var checkForNullRule =
                ValidationRulesFactory.CreateValidationRule<Game>(
                    LogicalOperatorAtTheRootLevel.Equal, constRule);
            compileResult = checkForNullRule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(checkForNullRule)}:{Environment.NewLine}{checkForNullRule.ExpressionDebugView()}");


            var ruleExecuteResult = checkForNotNullRule.IsValid(_game);
            ruleExecuteResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"with non-null parameter validationResult = {ruleExecuteResult}; expecting true");

            ruleExecuteResult = checkForNotNullRule.IsValid(null);
            ruleExecuteResult.Should().BeFalse();
            _testOutputHelper.WriteLine($"with null parameter validationResult = {ruleExecuteResult}; expecting false");

            ruleExecuteResult = checkForNullRule.IsValid(_game);
            ruleExecuteResult.Should().BeFalse();
            _testOutputHelper.WriteLine($"with non-null parameter validationResult = {ruleExecuteResult}; expecting false");

            ruleExecuteResult = checkForNullRule.IsValid(null);
            ruleExecuteResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"with null parameter validationResult = {ruleExecuteResult}; expecting true");
        }

        [Fact]
        public void CreateValidationRuleTest1UsingFactory()
        {
            var constRule = ConstantRulesFactory.CreateConstantRule<int>(value: "100");
            var rule = ValidationRulesFactory.CreateValidationRule<Game>((g => g.Ranking),
                                                        LogicalOperatorAtTheRootLevel.LessThan,
                                                        constRule);

            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(rule)}:{Environment.NewLine}" +
                                        $"{rule.ExpressionDebugView()}");

            var game = new Game { Ranking = 98 };
            var result = rule.IsValid(game);
            result.Should().BeTrue();

            game.Ranking = 100;
            result = rule.IsValid(game);
            result.Should().BeFalse();
        }

        [Fact]
        public void ApplyRuleToSubFieldOrPropertyUsingFactory()
        {
            var constRule = ConstantRulesFactory.CreateConstantRule<int>("3");
            var nameLengthGreaterThan3Rule = ValidationRulesFactory.CreateValidationRule<Game>((g => g.Name.Length),
                LogicalOperatorAtTheRootLevel.GreaterThan, constRule);
            var compileResult = nameLengthGreaterThan3Rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(nameLengthGreaterThan3Rule)}:{Environment.NewLine}" +
                                        $"{nameLengthGreaterThan3Rule.ExpressionDebugView()}");

            var validationResult = nameLengthGreaterThan3Rule.IsValid(_game);
            validationResult.Should().BeTrue();

            var someGameWithShortName = new Game { Name = "foo" };
            validationResult = nameLengthGreaterThan3Rule.IsValid(someGameWithShortName);
            validationResult.Should().BeFalse();
        }

        [Fact]
        public void ValidationRuleWithAndAlsoChildrenValidationRulesUsingFactory()
        {
            var nullConst = ConstantRulesFactory.CreateConstantRule<Game>("null");
            var nullStringConst = ConstantRulesFactory.CreateConstantRule<string>("null");
            var intConst = ConstantRulesFactory.CreateConstantRule<int>("3");

            var child1 =
                ValidationRulesFactory.CreateValidationRule<Game>(
                    LogicalOperatorAtTheRootLevel.NotEqual, nullConst);
            var child2 = ValidationRulesFactory.CreateValidationRule<Game>((g => g.Name),
                LogicalOperatorAtTheRootLevel.NotEqual, nullStringConst);
            var child3 = ValidationRulesFactory.CreateValidationRule<Game>((g => g.Name.Length),
                LogicalOperatorAtTheRootLevel.GreaterThan, intConst);

            var gameNotNullAndNameIsGreaterThan3CharsRule = ValidationRulesFactory.CreateValidationRule<Game>(
                ChildrenBindingOperator.AndAlso, new List<Rule> { child1, child2, child3 });

            var compileResult = gameNotNullAndNameIsGreaterThan3CharsRule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(gameNotNullAndNameIsGreaterThan3CharsRule)}:{Environment.NewLine}{gameNotNullAndNameIsGreaterThan3CharsRule.ExpressionDebugView()}");

            var validationResult = gameNotNullAndNameIsGreaterThan3CharsRule.IsValid(_game);
            validationResult.Should().BeTrue();

            validationResult = gameNotNullAndNameIsGreaterThan3CharsRule.IsValid(new Game{Name = "foo"});
            validationResult.Should().BeFalse();

            validationResult = gameNotNullAndNameIsGreaterThan3CharsRule.IsValid(null);
            validationResult.Should().BeFalse();
        }

        [Fact]
        public void ValidationRuleWithOneNotChildUsingFactory()
        {
            var constRule = ConstantRulesFactory.CreateConstantRule<Game>("null");
            var child1 =
                ValidationRulesFactory.CreateValidationRule<Game>(LogicalOperatorAtTheRootLevel.NotEqual, constRule);

            var gameNullRuleByUsingNotWithNotEqualToNullChild =
                ValidationRulesFactory.CreateValidationRule<Game>(ChildrenBindingOperator.Not, new List<Rule> {child1});
            var compileResult = gameNullRuleByUsingNotWithNotEqualToNullChild.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(gameNullRuleByUsingNotWithNotEqualToNullChild)}:{Environment.NewLine}" +
                                        $"{gameNullRuleByUsingNotWithNotEqualToNullChild.ExpressionDebugView()}");

            var validationResult = gameNullRuleByUsingNotWithNotEqualToNullChild.IsValid(_game);
            validationResult.Should().BeFalse();

            validationResult = gameNullRuleByUsingNotWithNotEqualToNullChild.IsValid(null);
            validationResult.Should().BeTrue();
        }

        [Fact]
        public void ValidationRuleWithOrElseChildrenValidationRulesUsingFactory()
        {
            var nullGame = ConstantRulesFactory.CreateConstantRule<Game>("null");
            var child1 =
                ValidationRulesFactory.CreateValidationRule<Game>(LogicalOperatorAtTheRootLevel.Equal, nullGame);

            var nullString = ConstantRulesFactory.CreateConstantRule<string>("null");
            var grandChild1 =
                ValidationRulesFactory.CreateValidationRule<Game>(g => g.Name, LogicalOperatorAtTheRootLevel.NotEqual,
                    nullString);

            var int3Const = ConstantRulesFactory.CreateConstantRule<int>("3");
            var grandChild2 = ValidationRulesFactory.CreateValidationRule<Game>(g => g.Name.Length,
                LogicalOperatorAtTheRootLevel.GreaterThan, int3Const);

            var child2 = ValidationRulesFactory.CreateValidationRule<Game>(ChildrenBindingOperator.AndAlso,
                new List<Rule> {grandChild1, grandChild2});

            var gameIsNullOrNameIsGreaterThan3CharsRule =
                ValidationRulesFactory.CreateValidationRule<Game>(ChildrenBindingOperator.OrElse,
                    new List<Rule> {child1, child2});
            
            var compileResult = gameIsNullOrNameIsGreaterThan3CharsRule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(gameIsNullOrNameIsGreaterThan3CharsRule)}:{Environment.NewLine}" +
                                        $"{gameIsNullOrNameIsGreaterThan3CharsRule.ExpressionDebugView()}");

            var validationResult = gameIsNullOrNameIsGreaterThan3CharsRule.IsValid(_game);
            validationResult.Should().BeTrue();

            validationResult = gameIsNullOrNameIsGreaterThan3CharsRule.IsValid(null);
            validationResult.Should().BeTrue();

            validationResult = gameIsNullOrNameIsGreaterThan3CharsRule.IsValid(new Game {Name = null});
            validationResult.Should().BeFalse();

            validationResult = gameIsNullOrNameIsGreaterThan3CharsRule.IsValid(new Game {Name = "a"});
            validationResult.Should().BeFalse();
        }

        [Fact]
        public void ValidationRuleWithTwoTypesUsingFactory()
        {
            var twoPlayersScoreRule = ValidationRulesFactory.CreateValidationRule<Player, Player>(
                                                                LogicalOperatorAtTheRootLevel.GreaterThan,
                                                                (p => p.CurrentScore), (p => p.CurrentScore));
            var compileResult = twoPlayersScoreRule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(twoPlayersScoreRule)}:{Environment.NewLine}{twoPlayersScoreRule.ExpressionDebugView()}");

            var validationResult = twoPlayersScoreRule.IsValid(_game.Players[0], _game.Players[1]);
            validationResult.Should().BeTrue();
            validationResult = twoPlayersScoreRule.IsValid(_game.Players[1], _game.Players[0]);
            validationResult.Should().BeFalse();
        }
    }
}