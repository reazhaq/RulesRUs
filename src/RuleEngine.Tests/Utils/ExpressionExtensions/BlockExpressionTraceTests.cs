using System.Linq.Expressions;
using System.Text;
using RuleEngine.Utils;
using Xunit;
using Xunit.Abstractions;

namespace RuleEngine.Tests.Utils.ExpressionExtensions
{
    public class BlockExpressionTraceTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public BlockExpressionTraceTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void TraceBlockExpression()
        {
            var exp1 = Expression.Constant(99, typeof(int));
            var block = Expression.Block(exp1);
            _testOutputHelper.WriteLine($"block: {block}");

            var sb = new StringBuilder();
            block.TraceNode(sb);
            _testOutputHelper.WriteLine(sb.ToString());
        }
    }
}