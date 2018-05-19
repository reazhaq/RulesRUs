using System.Linq.Expressions;
using System.Text;
using RuleEngine.Utils;
using Xunit;
using Xunit.Abstractions;

namespace RuleEngine.Tests.Utils.ExressionExtensions
{
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
}