namespace RuleEngine.Tests.Rules;

public class ValidationRulesTests : IClassFixture<ValidationRulesFixture>
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly Game _game;

    public ValidationRulesTests(ValidationRulesFixture validationRuleFixture, ITestOutputHelper testOutputHelper)
    {
        _game = validationRuleFixture.Game;
        _testOutputHelper = testOutputHelper;
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
        _testOutputHelper.WriteLine($"{nameof(numberShouldBe5Rule)}:{Environment.NewLine}{numberShouldBe5Rule.ExpressionDebugView()}");

        var numberShouldNotBe5Rule = new ValidationRule<int>
        {
            ValueToValidateAgainst = new ConstantRule<int> { Value = "5" },
            OperatorToUse = "NotEqual",
            RuleError = new RuleError { Code = "c2", Message = "number is 5"}
        };
        compileResult = numberShouldNotBe5Rule.Compile();
        compileResult.Should().BeTrue();
        _testOutputHelper.WriteLine($"{nameof(numberShouldNotBe5Rule)}:{Environment.NewLine}{numberShouldNotBe5Rule.ExpressionDebugView()}");

        var ruleExecuteResult = numberShouldBe5Rule.IsValid(5);
        ruleExecuteResult.Should().BeTrue();
        _testOutputHelper.WriteLine($"with 5 {nameof(numberShouldBe5Rule)} execute result: {ruleExecuteResult}");

        ruleExecuteResult = numberShouldBe5Rule.IsValid(6);
        ruleExecuteResult.Should().BeFalse();
        _testOutputHelper.WriteLine($"with 6 {nameof(numberShouldBe5Rule)} failed. " +
                                     $"Error code={numberShouldBe5Rule.RuleError.Code}, " +
                                     $"message={numberShouldBe5Rule.RuleError.Message}");

        ruleExecuteResult = numberShouldNotBe5Rule.IsValid(6);
        ruleExecuteResult.Should().BeTrue();
        _testOutputHelper.WriteLine($"with 6 {nameof(numberShouldNotBe5Rule)} execute result: {ruleExecuteResult}");

        ruleExecuteResult = numberShouldNotBe5Rule.IsValid(5);
        ruleExecuteResult.Should().BeFalse();
        _testOutputHelper.WriteLine($"with 5 {nameof(numberShouldNotBe5Rule)} failed. " +
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
        compileResult.Should().BeTrue();
        _testOutputHelper.WriteLine($"{nameof(checkForNotNullRule)}:{Environment.NewLine}{checkForNotNullRule.ExpressionDebugView()}");


        var checkForNullRule = new ValidationRule<Game>
        {
            ValueToValidateAgainst = new ConstantRule<Game> {Value = "null"},
            OperatorToUse = "Equal"
        };
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
        _testOutputHelper.WriteLine($"{nameof(rankingLessThan100Rule)}:{Environment.NewLine}" +
                                    $"{rankingLessThan100Rule.ExpressionDebugView()}");


        var validationResult = rankingLessThan100Rule.IsValid(_game);
        validationResult.Should().BeTrue();

        var someOtherGameWithHighRanking = new Game {Ranking = 101};
        validationResult = rankingLessThan100Rule.IsValid(someOtherGameWithHighRanking);
        validationResult.Should().BeFalse();
        _testOutputHelper.WriteLine($"with {nameof(someOtherGameWithHighRanking.Ranking)}={someOtherGameWithHighRanking.Ranking} " +
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
        _testOutputHelper.WriteLine($"{nameof(nameLengthGreaterThan3Rule)}:{Environment.NewLine}{nameLengthGreaterThan3Rule.ExpressionDebugView()}");


        var validationResult = nameLengthGreaterThan3Rule.IsValid(_game);
        validationResult.Should().BeTrue();

        var someGameWithShortName = new Game {Name = "foo"};
        validationResult = nameLengthGreaterThan3Rule.IsValid(someGameWithShortName);
        validationResult.Should().BeFalse();
        _testOutputHelper.WriteLine($"with {nameof(someGameWithShortName.Name)}={someGameWithShortName.Name} " +
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
            RuleError = new RuleError {Code = "c", Message = "m"},
            ChildrenRules =
            {
                new ValidationRule<Game>
                {
                    OperatorToUse = "NotEqual",
                    ValueToValidateAgainst = new ConstantRule<Game> {Value = "null"}
                },
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
        };

        var compileResult = gameNotNullAndNameIsGreaterThan3CharsRule.Compile();
        compileResult.Should().BeTrue();
        _testOutputHelper.WriteLine($"{nameof(gameNotNullAndNameIsGreaterThan3CharsRule)}:{Environment.NewLine}{gameNotNullAndNameIsGreaterThan3CharsRule.ExpressionDebugView()}");


        var validationResult = gameNotNullAndNameIsGreaterThan3CharsRule.IsValid(_game);
        validationResult.Should().BeTrue();

        validationResult = gameNotNullAndNameIsGreaterThan3CharsRule.IsValid(null);
        validationResult.Should().BeFalse();
        _testOutputHelper.WriteLine($"{nameof(gameNotNullAndNameIsGreaterThan3CharsRule)} failed. " +
                                     $"Error code={gameNotNullAndNameIsGreaterThan3CharsRule.RuleError.Code}, " +
                                     $"message={gameNotNullAndNameIsGreaterThan3CharsRule.RuleError.Message}");
    }

    [Fact]
    public void ValidationRuleWithOneNotChild()
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
        _testOutputHelper.WriteLine($"{nameof(gameNullRuleByUsingNotWithNotEqualToNullChild)}:{Environment.NewLine}{gameNullRuleByUsingNotWithNotEqualToNullChild.ExpressionDebugView()}");


        var validationResult = gameNullRuleByUsingNotWithNotEqualToNullChild.IsValid(_game);
        validationResult.Should().BeFalse();

        validationResult = gameNullRuleByUsingNotWithNotEqualToNullChild.IsValid(null);
        validationResult.Should().BeTrue();
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
        _testOutputHelper.WriteLine($"{nameof(gameIsNullOrNameIsGreaterThan3CharsRule)}:{Environment.NewLine}{gameIsNullOrNameIsGreaterThan3CharsRule.ExpressionDebugView()}");

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
    public void ValidationRuleWithTwoTypes()
    {
        var twoPlayersScoreGreaterThanRule = new ValidationRule<Player, Player>
        {
            OperatorToUse = "GreaterThan",
            ObjectToValidate1 = "CurrentScore",
            ObjectToValidate2 = "CurrentScore"
        };

        var compileResult = twoPlayersScoreGreaterThanRule.Compile();
        compileResult.Should().BeTrue();
        _testOutputHelper.WriteLine($"{nameof(twoPlayersScoreGreaterThanRule)}:{Environment.NewLine}" +
                                    $"{twoPlayersScoreGreaterThanRule.ExpressionDebugView()}");

        var validationResult = twoPlayersScoreGreaterThanRule.IsValid(_game.Players[0], _game.Players[1]);
        validationResult.Should().BeTrue();
        validationResult = twoPlayersScoreGreaterThanRule.IsValid(_game.Players[1], _game.Players[0]);
        validationResult.Should().BeFalse();
    }
}