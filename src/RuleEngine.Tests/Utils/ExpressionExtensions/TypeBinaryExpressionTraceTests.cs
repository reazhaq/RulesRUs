using System;
using System.Linq.Expressions;
using System.Text;
using FluentAssertions;
using RuleEngine.Utils;
using Xunit;
using Xunit.Abstractions;

namespace RuleEngine.Tests.Utils.ExpressionExtensions
{
    public class TypeBinaryExpressionTraceTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public TypeBinaryExpressionTraceTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void TypeIsTrace()
        {
            var param = Expression.Parameter(typeof(object));

            var typeIsIntExpression = Expression.TypeIs(param, typeof(int));
            _testOutputHelper.WriteLine($"typeIsExpression: {typeIsIntExpression}");

            var sb = new StringBuilder();
            typeIsIntExpression.TraceNode(sb);
            _testOutputHelper.WriteLine(sb.ToString());

            var typeIsLambda = Expression.Lambda<Func<object, bool>>(typeIsIntExpression, param);
            var compiledLambda = typeIsLambda.Compile(); // compiled becomes p=>p is int;
            compiledLambda.Should().NotBeNull();

            compiledLambda("blah").Should().BeFalse();
            compiledLambda(5).Should().BeTrue();
            object fiftyFive = 55;
            compiledLambda(fiftyFive).Should().BeTrue();
        }

        [Fact]
        public void TypeEqualTrace()
        {
            var exp1 = Expression.Constant(default(object), typeof(object));
            var typeByRef = typeof(int);
            var typeEqualExp = Expression.TypeEqual(exp1, typeByRef);
            _testOutputHelper.WriteLine($"typeEqualExp: {typeEqualExp}");

            var sb = new StringBuilder();
            typeEqualExp.TraceNode(sb);
            _testOutputHelper.WriteLine(sb.ToString());

            var lambda = Expression.Lambda<Func<bool>>(typeEqualExp);
            var compiled = lambda.Compile();
            compiled.Should().NotBeNull();

            compiled().Should().BeFalse();
        }

        [Fact]
        public void TypeAsTrace()
        {
            var exp1 = Expression.TypeAs(Expression.Constant(null, typeof(int?)), typeof(ValueType));
            _testOutputHelper.WriteLine($"exp1: {exp1}");

            var sb = new StringBuilder();
            exp1.TraceNode(sb);
            _testOutputHelper.WriteLine(sb.ToString());

            var lambda = Expression.Lambda<Func<ValueType>>(exp1);
            var compiled = lambda.Compile();
            compiled.Should().NotBeNull();

            compiled().Should().BeNull();
        }
    }
}