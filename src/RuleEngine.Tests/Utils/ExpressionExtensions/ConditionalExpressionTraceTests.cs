namespace RuleEngine.Tests.Utils.ExpressionExtensions;

public class ConditionalExpressionTraceTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public ConditionalExpressionTraceTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void TraceConditionalExpression()
    {
        var param = Expression.Parameter(typeof(int));
        var const5 = Expression.Constant(5, typeof(int));
        var exp1 = Expression.LessThan(param, const5);

        var exp2 = Expression.Constant("more");
        var exp3 = Expression.Constant("less");
        var cond = Expression.Condition(exp1, exp2, exp3);
        _testOutputHelper.WriteLine($"cond: {cond}");

        var sb = new StringBuilder();
        cond.TraceNode(sb);
        _testOutputHelper.WriteLine(sb.ToString());
    }
}