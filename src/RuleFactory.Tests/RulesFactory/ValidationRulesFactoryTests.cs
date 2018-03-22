using System;
using FluentAssertions;
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
    }
}