using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using FluentAssertions;
using Microsoft.CSharp.RuntimeBinder;
using RuleEngine.Tests.Model;
using RuleEngine.Utils;
using Xunit;
using Xunit.Abstractions;
using Binder = Microsoft.CSharp.RuntimeBinder.Binder;

namespace RuleEngine.Tests.Utils.ExressionExtensions
{
    public class UnaryExpressionTraceTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public UnaryExpressionTraceTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void TraceUnaryExpression1()
        {
            var val1 = Expression.Constant(5);
            var uExp = Expression.Negate(val1);
            _testOutputHelper.WriteLine($"uExp: {uExp}");

            var sb = new StringBuilder();
            uExp.TraceNode(sb);
            _testOutputHelper.WriteLine(sb.ToString());
        }

        [Fact]
        public void TraceUnaryExpression2()
        {
            var val1 = Expression.Constant(null, typeof(int?));
            var uExp = Expression.Negate(val1);
            _testOutputHelper.WriteLine($"uExp: {uExp}");

            var sb = new StringBuilder();
            uExp.TraceNode(sb);
            _testOutputHelper.WriteLine(sb.ToString());
        }

        [Fact]
        public void TraceUnaryExpression3()
        {
            var val1 = Expression.Constant(true, typeof(bool));
            var uExp = Expression.Not(val1);
            _testOutputHelper.WriteLine($"uExp: {uExp}");

            var sb = new StringBuilder();
            uExp.TraceNode(sb);
            _testOutputHelper.WriteLine(sb.ToString());
        }

        [Fact]
        public void TraceUnaryExpression4()
        {
            var param = Expression.Parameter(typeof(bool?));
            var uExp = Expression.Not(param);
            _testOutputHelper.WriteLine($"uExp: {uExp}");

            var sb = new StringBuilder();
            uExp.TraceNode(sb);
            _testOutputHelper.WriteLine(sb.ToString());
        }

        [Fact]
        public void TraceUnaryExpression5()
        {
            var uExp = Expression.Unbox(Expression.Default(typeof(object)), typeof(int?));
            _testOutputHelper.WriteLine($"uExp: {uExp}");

            var sb = new StringBuilder();
            uExp.TraceNode(sb);
            _testOutputHelper.WriteLine(sb.ToString());
        }
    }
}