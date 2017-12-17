using System;
using FluentAssertions;
using RuleEngine.Rules;
using RuleEngineTests.Fixture;
using RuleEngineTests.Model;
using Xunit;
using Xunit.Abstractions;

namespace RuleEngineTests.Rules
{
    public class ExpressionFuncRulesTests : IClassFixture<ExpressionRulesFixture>
    {
        private readonly ITestOutputHelper _testOutcomeHelper;
        private readonly Game _game1;
        private readonly Game _game2;

        public ExpressionFuncRulesTests(ExpressionRulesFixture expressionRuleFixture, ITestOutputHelper testOutcomeHelper)
        {
            _game1 = expressionRuleFixture.Game1;
            _game2 = expressionRuleFixture.Game2;
            _testOutcomeHelper = testOutcomeHelper;
        }

        [Fact]
        public void OneOutParameterExpressionTest()
        {
            var ruleReturningAFixedValue = new ExpressionFuncRules<int>(() => int.MaxValue);
            var compileResult = ruleReturningAFixedValue.Compile();
            compileResult.Should().BeTrue();
            var executeResult = ruleReturningAFixedValue.Execute();
            executeResult.Should().BeOfType(typeof(int)).And.Be(int.MaxValue);
        }

        [Fact]
        public void OneInOneOutParameterExpressionTest()
        {
            var ruleReturningCountOfPlayers = new ExpressionFuncRules<Game, int>(
                                                g => (g == null || g.Players == null) ? 0 : g.Players.Count);
            var compileResult = ruleReturningCountOfPlayers.Compile();
            compileResult.Should().BeTrue();
            var executeResult = ruleReturningCountOfPlayers.Execute(_game1);
            executeResult.Should().BeOfType(typeof(int)).And.Be(_game1.Players.Count);
        }

        [Fact]
        public void TwoInOneOutParameterExpressionTest()
        {
            var ruleReturningTotalCountOfPlayers = new ExpressionFuncRules<Game, Game, int>((g1, g2) =>
                                                    ((g1 == null || g1.Players == null) ? 0 : g1.Players.Count) +
                                                        ((g2 == null || g2.Players == null) ? 0 : g2.Players.Count)
                                                );
            var compileResult = ruleReturningTotalCountOfPlayers.Compile();
            compileResult.Should().BeTrue();
            var executeResult = ruleReturningTotalCountOfPlayers.Execute(_game1, _game2);
            executeResult.Should().BeOfType(typeof(int)).And.Be(_game1.Players.Count + _game2.Players.Count);
        }
    }
}