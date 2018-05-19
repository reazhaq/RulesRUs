using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Microsoft.CSharp.RuntimeBinder;
using RuleEngine.Utils;
using Xunit;
using Xunit.Abstractions;

namespace RuleEngine.Tests.Utils.ExressionExtensions
{
    public class DynamicExpressionTraceTests
    {
        private readonly ITestOutputHelper _testOutputHelper;
        public static IEnumerable<object[]> SizesAndSuffixes =>
            Enumerable.Range(0, 6).Select(i => new object[] { i, i > 0 & i < 5 ? i.ToString() : "N" });

        private static readonly Type[] Types =
        {
            typeof(int), typeof(object), typeof(DateTime), typeof(UnaryExpressionTraceTests)
        };

        public static IEnumerable<object[]> SizesAndTypes
            => Enumerable.Range(1, 6).SelectMany(i => Types, (i, t) => new object[] { i, t });

        public DynamicExpressionTraceTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
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
            _testOutputHelper.WriteLine($"exp: {exp}");

            var sb = new StringBuilder();
            exp.TraceNode(sb);
            _testOutputHelper.WriteLine(sb.ToString());
        }
    }
}