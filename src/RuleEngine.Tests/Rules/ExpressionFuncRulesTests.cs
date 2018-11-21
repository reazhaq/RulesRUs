using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using ModelForUnitTests;
using RuleEngine.Rules;
using RuleEngine.Tests.Fixture;
using Xunit;
using Xunit.Abstractions;

namespace RuleEngine.Tests.Rules
{
    public class ExpressionFuncRulesTests : IClassFixture<ExpressionRulesFixture>
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly Game _game1;
        private readonly Game _game2;

        public ExpressionFuncRulesTests(ExpressionRulesFixture expressionRuleFixture, ITestOutputHelper testOutputHelper)
        {
            _game1 = expressionRuleFixture.Game1;
            _game2 = expressionRuleFixture.Game2;
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void OneOutParameterExpressionTest()
        {
            var ruleReturningAFixedValue = new ExpressionFuncRule<int>(() => int.MaxValue);
            var compileResult = ruleReturningAFixedValue.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(ruleReturningAFixedValue)}:{Environment.NewLine}" +
                                         $"{ruleReturningAFixedValue.ExpressionDebugView()}");

            var executeResult = ruleReturningAFixedValue.Execute();
            executeResult.Should().BeOfType(typeof(int)).And.Be(int.MaxValue);
        }

        [Fact]
        public void OneInOneOutParameterExpressionTest()
        {
            // g => IIF(((g == null) OrElse (g.Players == null)), 0, g.Players.Count)
            var ruleReturningCountOfPlayers = new ExpressionFuncRule<Game, int>(
                                                g => (g == null || g.Players == null) ? 0 : g.Players.Count);
            var compileResult = ruleReturningCountOfPlayers.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(ruleReturningCountOfPlayers)}:{Environment.NewLine}" +
                                         $"{ruleReturningCountOfPlayers.ExpressionDebugView()}");

            var executeResult = ruleReturningCountOfPlayers.Execute(_game1);
            executeResult.Should().BeOfType(typeof(int)).And.Be(_game1.Players.Count);
        }

        [Fact]
        public void TwoInOneOutParameterExpressionTest()
        {
            var ruleReturningTotalCountOfPlayers = new ExpressionFuncRule<Game, Game, int>((g1, g2) =>
                                                    ((g1 == null || g1.Players == null) ? 0 : g1.Players.Count) +
                                                        ((g2 == null || g2.Players == null) ? 0 : g2.Players.Count)
                                                );
            var compileResult = ruleReturningTotalCountOfPlayers.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(ruleReturningTotalCountOfPlayers)}:{Environment.NewLine}" +
                                         $"{ruleReturningTotalCountOfPlayers.ExpressionDebugView()}");

            var executeResult = ruleReturningTotalCountOfPlayers.Execute(_game1, _game2);
            executeResult.Should().BeOfType(typeof(int)).And.Be(_game1.Players.Count + _game2.Players.Count);
        }

        [Fact]
        public void TwoInOneOutParameterExpressionTest2()
        {
            var ruleFindCountriesNotInOther = new ExpressionFuncRule<Game, Game, IEnumerable<Player>>((g1, g2) =>
                g1.Players.Except(g2.Players, new PlayerCountryEqualityComparer())
            );
            var compileResult = ruleFindCountriesNotInOther.Compile();
            _testOutputHelper.WriteLine($"{nameof(ruleFindCountriesNotInOther)}:{Environment.NewLine}" +
                                         $"{ruleFindCountriesNotInOther.ExpressionDebugView()}");

            compileResult.Should().BeTrue();
            var executeResult = ruleFindCountriesNotInOther.Execute(_game1, _game2).ToList();
            executeResult.Should().BeOfType(typeof(List<Player>));
            _testOutputHelper.WriteLine(".............g1.except(g2)....");
            executeResult.ForEach(p => _testOutputHelper.WriteLine($"country: {p.Country.CountryCode}"));

            executeResult = ruleFindCountriesNotInOther.Execute(_game2, _game1).ToList();
            _testOutputHelper.WriteLine(".............g2.except(g1)....");
            executeResult.ForEach(p => _testOutputHelper.WriteLine($"country: {p.Country.CountryCode}"));
        }
    }
}