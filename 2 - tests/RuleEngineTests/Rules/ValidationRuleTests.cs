using FluentAssertions;
using RuleEngine.Rules;
using RuleEngineTests.Fixture;
using RuleEngineTests.Model;
using Xunit;
using Xunit.Abstractions;

namespace RuleEngineTests.Rules
{
    public class ValidationRuleTests : IClassFixture<ValidationRuleFixture>
    {
        private readonly ITestOutputHelper _testOutcomeHelper;
        private readonly Game _game;

        public ValidationRuleTests(ValidationRuleFixture validationRuleFixture, ITestOutputHelper testOutcomeHelper)
        {
            _game = validationRuleFixture.Game;
            _testOutcomeHelper = testOutcomeHelper;
        }

        [Fact]
        public void RuleToCheckIfAnIntegerMatchesRuleValueOrNot()
        {
            var numberShouldBe5Rule = new ValidationRule<int>
            {
                ValueToValidateAgainst = new ConstantRule<int> {Value = "5"},
                OperatorToUse = "Equal",
                RuleError = new RuleError { Code="c1", Message = "number is not 5"}
            };
            var compileResult = numberShouldBe5Rule.Compile();
            compileResult.Should().BeTrue();

            var numberShouldNotBe5Rule = new ValidationRule<int>
            {
                ValueToValidateAgainst = new ConstantRule<int> { Value = "5" },
                OperatorToUse = "NotEqual",
                RuleError = new RuleError { Code = "c2", Message = "number is 5"}
            };
            compileResult = numberShouldNotBe5Rule.Compile();
            compileResult.Should().BeTrue();

            var ruleExecuteResult = numberShouldBe5Rule.Execute(5);
            ruleExecuteResult.Should().BeTrue();
            _testOutcomeHelper.WriteLine($"with 5 {nameof(numberShouldBe5Rule)} execute result: {ruleExecuteResult}");

            ruleExecuteResult = numberShouldBe5Rule.Execute(6);
            ruleExecuteResult.Should().BeFalse();
            _testOutcomeHelper.WriteLine($"with 6 {nameof(numberShouldBe5Rule)} failed. " +
                                         $"Error code={numberShouldBe5Rule.RuleError.Code}, " +
                                         $"message={numberShouldBe5Rule.RuleError.Message}");

            ruleExecuteResult = numberShouldNotBe5Rule.Execute(6);
            ruleExecuteResult.Should().BeTrue();
            _testOutcomeHelper.WriteLine($"with 6 {nameof(numberShouldNotBe5Rule)} execute result: {ruleExecuteResult}");

            ruleExecuteResult = numberShouldNotBe5Rule.Execute(5);
            ruleExecuteResult.Should().BeFalse();
            _testOutcomeHelper.WriteLine($"with 5 {nameof(numberShouldNotBe5Rule)} failed. " +
                                         $"Error code={numberShouldNotBe5Rule.RuleError.Code}, " +
                                         $"message={numberShouldNotBe5Rule.RuleError.Message}");
        }

        [Fact]
        public void RuleToCheckIfRootObjectIsNullOrNot()
        {
            var checkForNotNullRule = new ValidationRule<Game>
            {
                ValueToValidateAgainst = new ConstantRule<Game> {Value = "null"},
                OperatorToUse = "NotEqual"
            };
            var compileResult = checkForNotNullRule.Compile();
            compileResult
                .Should()
                .BeTrue();

            var checkForNullRule = new ValidationRule<Game>
            {
                ValueToValidateAgainst = new ConstantRule<Game> {Value = "null"},
                OperatorToUse = "Equal"
            };
            compileResult = checkForNullRule.Compile();
            compileResult
                .Should()
                .BeTrue();

            var ruleExecuteResult = checkForNotNullRule.Execute(_game);
            ruleExecuteResult
                .Should()
                .BeTrue();
            _testOutcomeHelper.WriteLine($"with non-null parameter executeResult = {ruleExecuteResult}; expecting true");

            ruleExecuteResult = checkForNotNullRule.Execute(null);
            ruleExecuteResult
                .Should()
                .BeFalse();
            _testOutcomeHelper.WriteLine($"with null parameter executeResult = {ruleExecuteResult}; expecting false");

            ruleExecuteResult = checkForNullRule.Execute(_game);
            ruleExecuteResult
                .Should()
                .BeFalse();
            _testOutcomeHelper.WriteLine($"with non-null parameter executeResult = {ruleExecuteResult}; expecting false");

            ruleExecuteResult = checkForNullRule.Execute(null);
            ruleExecuteResult
                .Should()
                .BeTrue();
            _testOutcomeHelper.WriteLine($"with null parameter executeResult = {ruleExecuteResult}; expecting true");
        }

        [Fact]
        public void ApplyRuleToFieldOrProperty()
        {
            var rankingLessThan100Rule = new ValidationRule<Game>
            {
                OperatorToUse = "LessThan",
                ValueToValidateAgainst = new ConstantRule<int> { Value = "100" },
                ObjectToValidate = "Ranking",
                RuleError = new RuleError { Code = "c1", Message = "Ranking must be less than 100" }
            };

            var compileResult = rankingLessThan100Rule.Compile();
            compileResult.Should().BeTrue();

            var executeResult = rankingLessThan100Rule.Execute(_game);
            executeResult.Should().BeTrue();

            var someOtherGameWithHighRanking = new Game {Ranking = 101};
            executeResult = rankingLessThan100Rule.Execute(someOtherGameWithHighRanking);
            executeResult.Should().BeFalse();
            _testOutcomeHelper.WriteLine($"with {nameof(someOtherGameWithHighRanking.Ranking)}={someOtherGameWithHighRanking.Ranking} " +
                                         $"{nameof(rankingLessThan100Rule)} failed. " +
                                         $"Error code={rankingLessThan100Rule.RuleError.Code}, " +
                                         $"message={rankingLessThan100Rule.RuleError.Message}");
        }

        [Fact]
        public void ApplyRuleToSubFieldOrProperty()
        {
            var nameLengthGreaterThan3Rule = new ValidationRule<Game>
            {
                OperatorToUse = "GreaterThan",
                ValueToValidateAgainst = new ConstantRule<int> {Value = "3"},
                ObjectToValidate = "Name.Length",
                RuleError = new RuleError { Code = "c1", Message = "Name length must be greater than 3"}
            };

            var compileResult = nameLengthGreaterThan3Rule.Compile();
            compileResult.Should().BeTrue();

            var executeResult = nameLengthGreaterThan3Rule.Execute(_game);
            executeResult.Should().BeTrue();

            var someGameWithShortName = new Game {Name = "foo"};
            executeResult = nameLengthGreaterThan3Rule.Execute(someGameWithShortName);
            executeResult.Should().BeFalse();
            _testOutcomeHelper.WriteLine($"with {nameof(someGameWithShortName.Name)}={someGameWithShortName.Name} " +
                                         $"{nameof(nameLengthGreaterThan3Rule)} failed. " +
                                         $"Error code={nameLengthGreaterThan3Rule.RuleError.Code}, " +
                                         $"message={nameLengthGreaterThan3Rule.RuleError.Message}");
        }
    }
}