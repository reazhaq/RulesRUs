namespace RuleEngine.Tests.Utils.ExpressionExtensions;

public class ParameterExpressionTraceTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public ParameterExpressionTraceTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void TraceParameterExpression()
    {
        var p0 = Expression.Parameter(typeof(int));
        _testOutputHelper.WriteLine($"p0: {p0}");

        var sb = new StringBuilder();
        p0.TraceNode(sb);
        _testOutputHelper.WriteLine(sb.ToString());
    }
}