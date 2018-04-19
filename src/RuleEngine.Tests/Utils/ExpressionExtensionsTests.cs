using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Microsoft.CSharp.RuntimeBinder;
using RuleEngine.Utils;
using Xunit;
using Xunit.Abstractions;
using Binder = Microsoft.CSharp.RuntimeBinder.Binder;

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
            _testOutputHelper.WriteLine($"binExp: {binExp}");

            var sb = new StringBuilder();
            binExp.TraceNode(sb);
            _testOutputHelper.WriteLine(sb.ToString());
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

        [Fact]
        public void TraceConditionalExpression()
        {
            var param = Expression.Parameter(typeof(int));
            var const5 = Expression.Constant(5, typeof(int));
            var exp1 = Expression.LessThan(param, const5);

            var exp2 = Expression.Constant("more");
            var exp3 = Expression.Constant("less");
            var cond = Expression.Condition(exp1, exp2, exp3);
            _testOutputHelper.WriteLine($"cond: {cond}");

            var sb = new StringBuilder();
            cond.TraceNode(sb);
            _testOutputHelper.WriteLine(sb.ToString());
        }

        [Fact]
        public void TraceConstantExpression()
        {
            var c1 = Expression.Constant(5);
            _testOutputHelper.WriteLine($"c1: {c1}");

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
            _testOutputHelper.WriteLine($"exp: {exp}");

            var sb = new StringBuilder();
            exp.TraceNode(sb);
            _testOutputHelper.WriteLine(sb.ToString());
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

        [Fact]
        public void TraceParameterExpression()
        {
            var p0 = Expression.Parameter(typeof(int));
            _testOutputHelper.WriteLine($"p0: {p0}");

            var sb = new StringBuilder();
            p0.TraceNode(sb);
            _testOutputHelper.WriteLine(sb.ToString());
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