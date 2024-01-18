namespace RuleEngine.Tests.Utils.ExpressionExtensions;

public class BinaryExpressionTraceTests
{
    private readonly ITestOutputHelper _testOutputHelper;
    public BinaryExpressionTraceTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void TraceBinaryExpression()
    {
        var param = Expression.Parameter(typeof(int));
        var const5 = Expression.Constant(5, typeof(int));
        var binExp = Expression.LessThan(param, const5);
        _testOutputHelper.WriteLine($"binExp: {binExp}");

        var sb = new StringBuilder();
        binExp.TraceNode(sb);
        _testOutputHelper.WriteLine(sb.ToString());
    }
}