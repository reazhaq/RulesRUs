using FluentAssertions;
using RuleEngine.Rules;
using RuleEngineTests.Fixture;
using RuleEngineTests.Model;
using Xunit;
using Xunit.Abstractions;

namespace RuleEngineTests.Rules
{
    public class ValidationRulesTests : IClassFixture<ValidationRulesFixture>
    {
        private readonly ITestOutputHelper _testOutcomeHelper;
        private readonly Game _game;

        public ValidationRulesTests(ValidationRulesFixture validationRuleFixture, ITestOutputHelper testOutcomeHelper)
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

        [Fact]
        public void ValidationRuleWithAndAlsoChildrenValidationRules()
        {
            var gameNotNullAndNameIsGreaterThan3CharsRule = new ValidationRule<Game>
            {
                OperatorToUse = "AndAlso",
                RuleError = new RuleError { Code = "c", Message = "m"}
            };
            gameNotNullAndNameIsGreaterThan3CharsRule.ChildrenRules.Add
            (
                new ValidationRule<Game>
                {
                    OperatorToUse = "NotEqual",
                    ValueToValidateAgainst = new ConstantRule<Game> {Value = "null"}
                }
            );
            gameNotNullAndNameIsGreaterThan3CharsRule.ChildrenRules.Add
            (
                new ValidationRule<Game>
                {
                    ValueToValidateAgainst = new ConstantRule<string> {Value = "null"},
                    ObjectToValidate = "Name",
                    OperatorToUse = "NotEqual"
                }
            );
            gameNotNullAndNameIsGreaterThan3CharsRule.ChildrenRules.Add
            (
                new ValidationRule<Game>
                {
                    ValueToValidateAgainst = new ConstantRule<int> {Value = "3"},
                    ObjectToValidate = "Name.Length",
                    OperatorToUse = "GreaterThan"
                }
            );

            var compileResult = gameNotNullAndNameIsGreaterThan3CharsRule.Compile();
            compileResult.Should().BeTrue();

            var executeResult = gameNotNullAndNameIsGreaterThan3CharsRule.Execute(_game);
            executeResult.Should().BeTrue();

            executeResult = gameNotNullAndNameIsGreaterThan3CharsRule.Execute(null);
            executeResult.Should().BeFalse();
            _testOutcomeHelper.WriteLine($"{nameof(gameNotNullAndNameIsGreaterThan3CharsRule)} failed. " +
                                         $"Error code={gameNotNullAndNameIsGreaterThan3CharsRule.RuleError.Code}, " +
                                         $"message={gameNotNullAndNameIsGreaterThan3CharsRule.RuleError.Message}");
        }

        [Fact]
        public void ValidataionRuleWithOneNotChild()
        {
            var gameNullRuleByUsingNotWithNotEqualToNullChild = new ValidationRule<Game>
            {
                OperatorToUse = "Not",
                RuleError = new RuleError {Code = "c", Message = "m"}
            };
            gameNullRuleByUsingNotWithNotEqualToNullChild.ChildrenRules.Add(
                new ValidationRule<Game>
                {
                    OperatorToUse = "NotEqual",
                    ValueToValidateAgainst = new ConstantRule<Game> {Value = "null"}
                }
            );

            var compileResult = gameNullRuleByUsingNotWithNotEqualToNullChild.Compile();
            compileResult.Should().BeTrue();

            var executeResult = gameNullRuleByUsingNotWithNotEqualToNullChild.Execute(_game);
            executeResult.Should().BeFalse();

            executeResult = gameNullRuleByUsingNotWithNotEqualToNullChild.Execute(null);
            executeResult.Should().BeTrue();
        }

        [Fact]
        public void ValidationRuleWithOrElseChildrenValidationRules()
        {
            var gameIsNullOrNameIsGreaterThan3CharsRule = new ValidationRule<Game> {OperatorToUse = "OrElse"};
            gameIsNullOrNameIsGreaterThan3CharsRule.ChildrenRules.Add(new ValidationRule<Game>
            {
                OperatorToUse = "Equal",
                ValueToValidateAgainst = new ConstantRule<Game> {Value = "null"}
            });
            gameIsNullOrNameIsGreaterThan3CharsRule.ChildrenRules.Add
            (
                new ValidationRule<Game>
                {
                    OperatorToUse = "AndAlso",
                    ChildrenRules =
                    {
                        new ValidationRule<Game>
                        {
                            ValueToValidateAgainst = new ConstantRule<string> {Value = "null"},
                            ObjectToValidate = "Name",
                            OperatorToUse = "NotEqual"
                        },
                        new ValidationRule<Game>
                        {
                            ValueToValidateAgainst = new ConstantRule<int> {Value = "3"},
                            ObjectToValidate = "Name.Length",
                            OperatorToUse = "GreaterThan"
                        }

                    }
                }
            );

            var compileResult = gameIsNullOrNameIsGreaterThan3CharsRule.Compile();
            compileResult.Should().BeTrue();

            var executeResult = gameIsNullOrNameIsGreaterThan3CharsRule.Execute(_game);
            executeResult.Should().BeTrue();

            executeResult = gameIsNullOrNameIsGreaterThan3CharsRule.Execute(null);
            executeResult.Should().BeTrue();

            executeResult = gameIsNullOrNameIsGreaterThan3CharsRule.Execute(new Game {Name = null});
            executeResult.Should().BeFalse();

            executeResult = gameIsNullOrNameIsGreaterThan3CharsRule.Execute(new Game {Name = "a"});
            executeResult.Should().BeFalse();
        }

        [Fact]
        public void ValidationRuleWithTwoTypes()
        {
            var twoPlayersScoreRule = new ValidationRule<Player, Player>
            {
                OperatorToUse = "GreaterThan",
                ObjectToValidate1 = "CurrentScore",
                ObjectToValidate2 = "CurrentScore"
            };

            var compileResult = twoPlayersScoreRule.Compile();
            compileResult.Should().BeTrue();

            var executeResult = twoPlayersScoreRule.Execute(_game.Players[0], _game.Players[1]);
            executeResult.Should().BeTrue();
            executeResult = twoPlayersScoreRule.Execute(_game.Players[1], _game.Players[0]);
            executeResult.Should().BeFalse();
        }
    }
}