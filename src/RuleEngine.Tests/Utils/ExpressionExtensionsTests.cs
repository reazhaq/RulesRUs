using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using FluentAssertions;
using Microsoft.CSharp.RuntimeBinder;
using RuleEngine.Rules;
using RuleEngine.Utils;
using Xunit;
using Xunit.Abstractions;

namespace RuleEngine.Tests.Utils
{
    public class ExpressionExtensionsTests
    {
        private readonly ITestOutputHelper _testOutputHelper;
        public static IEnumerable<object[]> SizesAndSuffixes =>
            Enumerable.Range(0, 6).Select(i => new object[] { i, i > 0 & i < 5 ? i.ToString() : "N" });

        private static Type[] Types =
        {
            typeof(int), typeof(object), typeof(DateTime), typeof(ExpressionExtensionsTests)
        };

        public static IEnumerable<object[]> SizesAndTypes
            => Enumerable.Range(1, 6).SelectMany(i => Types, (i, t) => new object[] { i, t });

        public ExpressionExtensionsTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void TraceBinaryExpression()
        {
            var param = Expression.Parameter(typeof(int));
            var const5 = Expression.Constant(5, typeof(int));
            var binExp = Expression.LessThan(param, const5);

            var sb = new StringBuilder();
            binExp.TraceNode(sb);
            _testOutputHelper.WriteLine(sb.ToString());
        }

        [Fact]
        public void TraceBlockExpression()
        {
            var exp1 = Expression.Constant(99, typeof(int));
            var block = Expression.Block(exp1);

            var sb = new StringBuilder();
            block.TraceNode(sb);
            _testOutputHelper.WriteLine(sb.ToString());
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

            var sb = new StringBuilder();
            cond.TraceNode(sb);
            _testOutputHelper.WriteLine(sb.ToString());
        }

        [Fact]
        public void TraceConstantExpression()
        {
            var c1 = Expression.Constant(5);

            var sb = new StringBuilder();
            c1.TraceNode(sb);
            _testOutputHelper.WriteLine(sb.ToString());
        }

        [Theory, MemberData(nameof(SizesAndTypes))]
        public void TraceDynamicExpression(int size, Type type)
        {
            //CallSiteBinder
            var binder = Binder.GetMember(
                CSharpBinderFlags.None, "Member", GetType(),
                new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
            //DynamicExpression
            var exp = Expression.Dynamic(
                binder, type, Enumerable.Range(0, size).Select(_ => Expression.Constant(0)));
            Assert.Equal(type, exp.Type);
            Assert.Equal(ExpressionType.Dynamic, exp.NodeType);

            var sb = new StringBuilder();
            exp.TraceNode(sb);
            _testOutputHelper.WriteLine(sb.ToString());
        }
    }
}