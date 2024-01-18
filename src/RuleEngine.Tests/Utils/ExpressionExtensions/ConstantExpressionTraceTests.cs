namespace RuleEngine.Tests.Utils.ExpressionExtensions;

public class ConstantExpressionTraceTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public ConstantExpressionTraceTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void TraceConstantExpression()
    {
        var c1 = Expression.Constant(5);
        _testOutputHelper.WriteLine($"c1: {c1}");

        var sb = new StringBuilder();
        c1.TraceNode(sb);
        _testOutputHelper.WriteLine(sb.ToString());
    }
}