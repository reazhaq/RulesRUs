using System.Linq.Expressions;
using System.Text;
using RuleEngine.Utils;
using Xunit;
using Xunit.Abstractions;

namespace RuleEngine.Tests.Utils.ExressionExtensions
{
    public class NewExpressionTraceTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public NewExpressionTraceTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void TraceNewExpression()
        {
            var newExp = Expression.New(typeof(object));
            //var newExp = Expression.New(typeof(object).GetConstructors()[0],
            //                        Enumerable.Empty<ParameterExpression>());
            _testOutputHelper.WriteLine($"newExp: {newExp}");

            var sb = new StringBuilder();
            newExp.TraceNode(sb);
            _testOutputHelper.WriteLine(sb.ToString());
        }

        private class SomeClassWithByRefCtor
        {
            public SomeClassWithByRefCtor(ref int x)
            {
                x = x + 99;
            }
        }

        private class SomeClassWithParamCtor
        {
            public SomeClassWithParamCtor(int x){}

            public SomeClassWithParamCtor(int x, int y){}
        }

        [Fact]
        public void TraceNewExpressionWithByRefCtor()
        {
            var p0 = Expression.Parameter(typeof(int).MakeByRefType());
            var newExp = Expression.New(typeof(SomeClassWithByRefCtor).GetConstructors()[0], p0);
            _testOutputHelper.WriteLine($"newExp: {newExp}");

            var sb = new StringBuilder();
            newExp.TraceNode(sb);
            _testOutputHelper.WriteLine(sb.ToString());
        }

        [Fact]
        public void TraceNewExpressionWithParamCtor()
        {
            var ctor = typeof(SomeClassWithParamCtor).GetConstructors()[0];
            var arg1 = Expression.Constant(5);
            var newExp = Expression.New(ctor, arg1);
            _testOutputHelper.WriteLine($"newExp: {newExp}");

            var sb = new StringBuilder();
            newExp.TraceNode(sb);
            _testOutputHelper.WriteLine(sb.ToString());
        }

        [Fact]
        public void TraceNewExpressionWithParamCtor2()
        {
            var ctor = typeof(SomeClassWithParamCtor).GetConstructors()[1];
            var arg1 = Expression.Constant(5);
            var arg2 = Expression.Constant(9);
            var newExp = Expression.New(ctor, arg1, arg2);
            _testOutputHelper.WriteLine($"newExp: {newExp}");

            var sb = new StringBuilder();
            newExp.TraceNode(sb);
            _testOutputHelper.WriteLine(sb.ToString());
        }
    }
}