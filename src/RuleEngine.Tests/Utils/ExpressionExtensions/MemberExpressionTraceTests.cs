namespace RuleEngine.Tests.Utils.ExpressionExtensions;

public class MemberExpressionTraceTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public MemberExpressionTraceTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void TraceMemberExpression()
    {
        var stringConst = Expression.Constant("something", typeof(string));
        var stringLength = Expression.Property(stringConst, typeof(string), "Length");
        _testOutputHelper.WriteLine($"stringLength: {stringLength}");

        var sb = new StringBuilder();
        stringLength.TraceNode(sb);
        _testOutputHelper.WriteLine(sb.ToString());
    }
}