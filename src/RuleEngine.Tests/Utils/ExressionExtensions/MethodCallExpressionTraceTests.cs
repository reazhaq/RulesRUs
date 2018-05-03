using System;
using System.Linq.Expressions;
using System.Text;
using FluentAssertions;
using RuleEngine.Tests.Model;
using RuleEngine.Utils;
using Xunit;
using Xunit.Abstractions;

namespace RuleEngine.Tests.Utils.ExressionExtensions
{
    public class MethodCallExpressionTraceTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public MethodCallExpressionTraceTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void TraceMethodCallExpression()
        {
            var stringConst = Expression.Constant("something", typeof(string));
            var p1 = Expression.Parameter(typeof(string), "p1");
            var callExp = Expression.Call(stringConst,
                typeof(string).GetMethodInfo("Equals", new[] {typeof(string)}),
                p1);
            _testOutputHelper.WriteLine($"callExp: {callExp}");

            var sb = new StringBuilder();
            callExp.TraceNode(sb);
            _testOutputHelper.WriteLine(sb.ToString());
        }

        [Fact]
        public void TraceMethodCallExpression2()
        {
            var createGameMethodInfo = typeof(Game).GetMethodInfo("CreateGame", null);
            var callExp = Expression.Call(createGameMethodInfo, (Expression[])null);
            _testOutputHelper.WriteLine($"callExp: {callExp}");

            var sb = new StringBuilder();
            callExp.TraceNode(sb);
            _testOutputHelper.WriteLine(sb.ToString());

            var lambda = Expression.Lambda<Func<Game>>(callExp);
            var compiled = lambda.Compile();
            compiled.Should().NotBeNull();

            var newGame = compiled();
            newGame.Should().NotBeNull();
            newGame.Name.Should().Be("new");
            newGame.Active.Should().BeFalse();
        }

        [Theory]
        [InlineData("one")]
        [InlineData("two")]
        public void TraceMethodCallExpression3(string name)
        {
            var param = Expression.Parameter(typeof(string), "name");

            var createGameMethodInfo = typeof(Game).GetMethodInfo("CreateGame", new []{typeof(string)});
            var callExp = Expression.Call(createGameMethodInfo, new Expression[]{param});
            _testOutputHelper.WriteLine($"callExp: {callExp}");

            var sb = new StringBuilder();
            callExp.TraceNode(sb);
            _testOutputHelper.WriteLine(sb.ToString());

            var lambda = Expression.Lambda<Func<string, Game>>(callExp, param);
            var compiled = lambda.Compile();
            compiled.Should().NotBeNull();

            var newGame = compiled(name);
            newGame.Should().NotBeNull();
            newGame.Name.Should().Be(name);
            newGame.Active.Should().BeFalse();
        }
    }
}