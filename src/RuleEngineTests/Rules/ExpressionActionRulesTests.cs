using FluentAssertions;
using RuleEngine.Rules;
using RuleEngine.Tests.Fixture;
using RuleEngine.Tests.Model;
using Xunit;
using Xunit.Abstractions;

namespace RuleEngine.Tests.Rules
{
    public class ExpressionActionRulesTests : IClassFixture<ExpressionRulesFixture>
    {
        private readonly ITestOutputHelper _testOutcomeHelper;
        private readonly Game _game1;
        private readonly Game _game2;

        public ExpressionActionRulesTests(ExpressionRulesFixture expressionRuleFixture, ITestOutputHelper testOutcomeHelper)
        {
            _game1 = expressionRuleFixture.Game1;
            _game2 = expressionRuleFixture.Game2;
            _testOutcomeHelper = testOutcomeHelper;
        }

        [Fact]
        public void UpdateGame1ScoreToNegative()
        {
            var updateGameRankingRule = new ExpressionActionRule<Game>(g => ApplySomeRule(g));
            var compileResult = updateGameRankingRule.Compile();
            compileResult.Should().BeTrue();
            updateGameRankingRule.Execute(_game1);
            _game1.Ranking.Should().Be(int.MinValue);
        }

        private void ApplySomeRule(Game game) => game.Ranking = int.MinValue;
    }
}