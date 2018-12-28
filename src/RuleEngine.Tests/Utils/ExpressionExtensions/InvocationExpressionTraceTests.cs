using System;
using System.Linq.Expressions;
using System.Text;
using RuleEngine.Utils;
using Xunit;
using Xunit.Abstractions;

namespace RuleEngine.Tests.Utils.ExpressionExtensions
{
    public class InvocationExpressionTraceTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public InvocationExpressionTraceTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void TraceInvocationExpression()
        {
            Expression<Func<int, int, bool>> largeSumTest = (num1, num2) => (num1 + num2) > 1000;

            var invocationExpression =
                Expression.Invoke(
                    largeSumTest,
                    Expression.Constant(539),
                    Expression.Constant(281));
            _testOutputHelper.WriteLine($"invocationExpression: {invocationExpression}");

            var sb = new StringBuilder();
            invocationExpression.TraceNode(sb);
            _testOutputHelper.WriteLine(sb.ToString());
        }
    }
}