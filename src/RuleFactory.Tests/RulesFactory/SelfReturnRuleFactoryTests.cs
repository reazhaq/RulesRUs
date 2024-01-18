namespace RuleFactory.Tests.RulesFactory;

public class SelfReturnRuleFactoryTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public SelfReturnRuleFactoryTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void CreateASelfReturnStringRuleUsingFactory()
    {
        var rule = SelfReturnRuleFactory.CreateSelfReturnRule<string>();
        var compileResult = rule.Compile();
        compileResult.Should().BeTrue();
        _testOutputHelper.WriteLine($"rule: {rule}");

        var result = rule.Get("one");
        result.Should().Be("one");

        result = rule.Get(null);
        result.Should().BeNull();

        result = rule.Get("two");
        result.Should().Be("two");
    }

    [Fact]
    public void CreateASelfReturnNullableIntRuleUsingFactory()
    {
        var rule = SelfReturnRuleFactory.CreateSelfReturnRule<int?>();
        var compileResult = rule.Compile();
        compileResult.Should().BeTrue();
        _testOutputHelper.WriteLine($"rule: {rule}");

        var result = rule.Get(5);
        result.Should().Be(5);

        result = rule.Get(null);
        result.Should().BeNull();
    }

    [Theory]
    [InlineData(5)]
    [InlineData(-5)]
    [InlineData(int.MaxValue)]
    public void IntSelfReturnUsingFactory(int someValue)
    {
        var rule = SelfReturnRuleFactory.CreateSelfReturnRule<int>();
        var compileResult = rule.Compile();
        compileResult.Should().BeTrue();
        _testOutputHelper.WriteLine($"selfReturnRule for Int:{Environment.NewLine}" +
                                    $"{rule.ExpressionDebugView()}");

        var value = rule.Get(someValue);
        value.Should().Be(someValue);
    }

    [Theory]
    [InlineData("one")]
    [InlineData(null)]
    [InlineData("")]
    public void StringSelfReturnUsingFactory(string someValue)
    {
        var rule = SelfReturnRuleFactory.CreateSelfReturnRule<string>();
        var compileResult = rule.Compile();
        compileResult.Should().BeTrue();
        _testOutputHelper.WriteLine($"selfReturnRule for String:{Environment.NewLine}" +
                                    $"{rule.ExpressionDebugView()}");

        var value = rule.Get(someValue);
        value.Should().Be(someValue);

        var referenceEquals = ReferenceEquals(someValue, value);
        referenceEquals.Should().BeTrue();
    }

    [Fact]
    public void GameSelfReturnUsingFactory()
    {
        var rule = SelfReturnRuleFactory.CreateSelfReturnRule<Game>();
        var compileResult = rule.Compile();
        compileResult.Should().BeTrue();
        _testOutputHelper.WriteLine($"selfReturnRule for Game:{Environment.NewLine}" +
                                    $"{rule.ExpressionDebugView()}");

        var someGame = new Game();
        var value = rule.Get(someGame);
        value.Should().Be(someGame);

        var referenceEquals = ReferenceEquals(someGame, value);
        referenceEquals.Should().BeTrue();
    }
}