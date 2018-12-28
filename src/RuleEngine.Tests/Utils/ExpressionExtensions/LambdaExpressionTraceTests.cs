using System;
using System.Linq.Expressions;
using System.Text;
using RuleEngine.Utils;
using Xunit;
using Xunit.Abstractions;

namespace RuleEngine.Tests.Utils.ExpressionExtensions
{
    public class LambdaExpressionTraceTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public LambdaExpressionTraceTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void TraceLambdaExpression()
        {
            var p0 = Expression.Parameter(typeof(int), "p0");
            var p1 = Expression.Parameter(typeof(int), "p1");

            var lambdaExpression = Expression.Lambda<Func<int, int, int>>(
                Expression.Add(p0, p1),
                new ParameterExpression[] {p0, p1});
            _testOutputHelper.WriteLine($"lambdaExpression: {lambdaExpression}");

            var sb = new StringBuilder();
            lambdaExpression.TraceNode(sb);
            _testOutputHelper.WriteLine(sb.ToString());
        }
    }
}