using System;
using System.Collections.Generic;
using FluentAssertions;
using RuleEngine.Rules;
using RuleFactory.RulesFactory;
using RuleFactory.Tests.Model;
using Xunit;
using Xunit.Abstractions;

namespace RuleFactory.Tests.RulesFactory
{
    public class ValidationRulesFactoryTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ValidationRulesFactoryTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void RuleToCheckIfAnIntegerMatchesRuleValueOrNot()
        {
            var constRule = ConstantRulesFactory.CreateConstantRule<int>("5");
            var numberShouldBe5Rule =
                ValidationRulesFactory.CreateValidationRule<int>(
                    ValidationRulesFactory.LogicalOperatorAtTheRootLevel.Equal, constRule);
            var compileResult = numberShouldBe5Rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(numberShouldBe5Rule)}:{Environment.NewLine}{numberShouldBe5Rule.ExpressionDebugView()}");

            var numberShouldNotBe5Rule =
                ValidationRulesFactory.CreateValidationRule<int>(
                    ValidationRulesFactory.LogicalOperatorAtTheRootLevel.NotEqual, constRule);
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
        public void RuleToCheckIfRootObjectIsNullOrNot()
        {
            var constRule = ConstantRulesFactory.CreateConstantRule<Game>("null");
            var checkForNotNullRule =
                ValidationRulesFactory.CreateValidationRule<Game>(
                    ValidationRulesFactory.LogicalOperatorAtTheRootLevel.NotEqual, constRule);
            var compileResult = checkForNotNullRule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(checkForNotNullRule)}:{Environment.NewLine}{checkForNotNullRule.ExpressionDebugView()}");


            var checkForNullRule =
                ValidationRulesFactory.CreateValidationRule<Game>(
                    ValidationRulesFactory.LogicalOperatorAtTheRootLevel.Equal, constRule);
            compileResult = checkForNullRule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(checkForNullRule)}:{Environment.NewLine}{checkForNullRule.ExpressionDebugView()}");


            var game = new Game();
            var ruleExecuteResult = checkForNotNullRule.IsValid(game);
            ruleExecuteResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"with non-null parameter validationResult = {ruleExecuteResult}; expecting true");

            ruleExecuteResult = checkForNotNullRule.IsValid(null);
            ruleExecuteResult.Should().BeFalse();
            _testOutputHelper.WriteLine($"with null parameter validationResult = {ruleExecuteResult}; expecting false");

            ruleExecuteResult = checkForNullRule.IsValid(game);
            ruleExecuteResult.Should().BeFalse();
            _testOutputHelper.WriteLine($"with non-null parameter validationResult = {ruleExecuteResult}; expecting false");

            ruleExecuteResult = checkForNullRule.IsValid(null);
            ruleExecuteResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"with null parameter validationResult = {ruleExecuteResult}; expecting true");
        }

        [Fact]
        public void CreateValidationRuleTest1()
        {
            var constRule = ConstantRulesFactory.CreateConstantRule<int>(value: "100");
            var rule = ValidationRulesFactory.CreateValidationRule<Game>((g => g.Ranking),
                                                        ValidationRulesFactory.LogicalOperatorAtTheRootLevel.LessThan,
                                                        constRule);

            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(rule)}:{Environment.NewLine}" +
                                        $"{rule.ExpressionDebugView()}");

            var game = new Game {Ranking = 98};
            var result = rule.IsValid(game);
            result.Should().BeTrue();

            game.Ranking = 100;
            result = rule.IsValid(game);
            result.Should().BeFalse();
        }

        [Fact]
        public void ApplyRuleToSubFieldOrProperty()
        {
            var constRule = ConstantRulesFactory.CreateConstantRule<int>("3");
            var nameLengthGreaterThan3Rule = ValidationRulesFactory.CreateValidationRule<Game>((g => g.Name.Length),
                ValidationRulesFactory.LogicalOperatorAtTheRootLevel.GreaterThan, constRule);
            var compileResult = nameLengthGreaterThan3Rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(nameLengthGreaterThan3Rule)}:{Environment.NewLine}" +
                                        $"{nameLengthGreaterThan3Rule.ExpressionDebugView()}");

            var game = new Game {Name = "Game 1"};
            var validationResult = nameLengthGreaterThan3Rule.IsValid(game);
            validationResult.Should().BeTrue();

            var someGameWithShortName = new Game {Name = "foo"};
            validationResult = nameLengthGreaterThan3Rule.IsValid(someGameWithShortName);
            validationResult.Should().BeFalse();
        }

        [Fact]
        public void ValidationRuleWithAndAlsoChildrenValidationRules()
        {
            var nullConst = ConstantRulesFactory.CreateConstantRule<Game>("null");
            var nullStringConst = ConstantRulesFactory.CreateConstantRule<string>("null");
            var intConst = ConstantRulesFactory.CreateConstantRule<int>("3");

            var child1 =
                ValidationRulesFactory.CreateValidationRule<Game>(
                    ValidationRulesFactory.LogicalOperatorAtTheRootLevel.NotEqual, nullConst);
            var child2 = ValidationRulesFactory.CreateValidationRule<Game>((g => g.Name),
                ValidationRulesFactory.LogicalOperatorAtTheRootLevel.NotEqual, nullStringConst);
            var child3 = ValidationRulesFactory.CreateValidationRule<Game>((g => g.Name.Length),
                ValidationRulesFactory.LogicalOperatorAtTheRootLevel.GreaterThan, intConst);

            var gameNotNullAndNameIsGreaterThan3CharsRule = ValidationRulesFactory.CreateValidationRule<Game>(
                ValidationRulesFactory.ChildrenBindingOperator.AndAlso, new List<Rule> {child1, child2, child3});

            var compileResult = gameNotNullAndNameIsGreaterThan3CharsRule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(gameNotNullAndNameIsGreaterThan3CharsRule)}:{Environment.NewLine}{gameNotNullAndNameIsGreaterThan3CharsRule.ExpressionDebugView()}");

            var game = new Game {Name = "Game 1"};
            var validationResult = gameNotNullAndNameIsGreaterThan3CharsRule.IsValid(game);
            validationResult.Should().BeTrue();

            game.Name = "foo";
            validationResult = gameNotNullAndNameIsGreaterThan3CharsRule.IsValid(game);
            validationResult.Should().BeFalse();

            validationResult = gameNotNullAndNameIsGreaterThan3CharsRule.IsValid(null);
            validationResult.Should().BeFalse();
        }
    }
}