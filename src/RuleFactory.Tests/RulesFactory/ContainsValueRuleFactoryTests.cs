namespace RuleFactory.Tests.RulesFactory;

public class ContainsValueRuleFactoryTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public ContainsValueRuleFactoryTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Theory]
    [InlineData("one", true)]
    [InlineData("Two", true)]
    [InlineData("THREE", true)]
    [InlineData("fIVe", true)]
    [InlineData("Six", true)]
    [InlineData("seven", false)]
    public void ContainsValueTestWithIgnoreCaseUsingFactory(string valueToSearch, bool expectedResult)
    {
        IList<string> collectionToSearch = new List<string> { "one", "two", "three", "four", "five", "six" };
        var containsRule = ContainsValueRuleFactory.CreateContainsValueRule(collectionToSearch,
                                                "System.StringComparer", "OrdinalIgnoreCase");

        var compileResult = containsRule.Compile();
        compileResult.Should().BeTrue();
        _testOutputHelper.WriteLine($"{nameof(containsRule)}:{Environment.NewLine}" +
                                    $"{containsRule.ExpressionDebugView()}");

        var containsValue = containsRule.ContainsValue(valueToSearch);
        _testOutputHelper.WriteLine($"expected: {expectedResult} - actual: {containsValue}");
        containsValue.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("one", true)]
    [InlineData("Two", false)]
    [InlineData("THREE", false)]
    [InlineData("fIVe", false)]
    [InlineData("Six", false)]
    [InlineData("seven", false)]
    public void ContainsValueTestCaseSensitiveUsingFactory(string valueToSearch, bool expectedResult)
    {
        IList<string> collectionToSearch = new List<string> { "one", "two", "three", "four", "five", "six" };
        var containsRule = ContainsValueRuleFactory.CreateContainsValueRule(collectionToSearch, null, null);

        var compileResult = containsRule.Compile();
        compileResult.Should().BeTrue();
        _testOutputHelper.WriteLine($"{nameof(containsRule)}:{Environment.NewLine}" +
                                    $"{containsRule.ExpressionDebugView()}");

        var containsValue = containsRule.ContainsValue(valueToSearch);
        _testOutputHelper.WriteLine($"expected: {expectedResult} - actual: {containsValue}");
        containsValue.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(2, true)]
    [InlineData(7, false)]
    public void ContainsValueTestForIntCollectionUsingFactory(int valueToSearch, bool expectedResult)
    {
        var containsRule = ContainsValueRuleFactory.CreateContainsValueRule(new List<int>{ 1, 2, 3, 4, 5, 6 },
                                                                                    null,null);

        var compileResult = containsRule.Compile();
        compileResult.Should().BeTrue();
        _testOutputHelper.WriteLine($"{nameof(containsRule)}:{Environment.NewLine}" +
                                    $"{containsRule.ExpressionDebugView()}");

        var containsValue = containsRule.ContainsValue(valueToSearch);
        _testOutputHelper.WriteLine($"expected: {expectedResult} - actual: {containsValue}");
        containsValue.Should().Be(expectedResult);
    }

    [Fact]
    public void CreateComparerOnTheFlyUsingReflectionUsingFactory()
    {
        var equalityComparerPropertyName="OrdinalIgnoreCase";
        var equalityComparerClassName = "System.StringComparer";
        var collectionToSearch = new List<string>{"one", "two", "three", "four", "five", "six"};
        var containsRule = ContainsValueRuleFactory.CreateContainsValueRule<string>(collectionToSearch,
            equalityComparerClassName, equalityComparerPropertyName);

        var compileResult = containsRule.Compile();
        compileResult.Should().BeTrue();
        _testOutputHelper.WriteLine($"{nameof(containsRule)}:{Environment.NewLine}" +
                                    $"{containsRule.ExpressionDebugView()}");

        var a1 = containsRule.ContainsValue("One");
        a1.Should().BeTrue();
        var a2 = containsRule.ContainsValue("tWo");
        a2.Should().BeTrue();
        var a7 = containsRule.ContainsValue("seven");
        a7.Should().BeFalse();
    }
}