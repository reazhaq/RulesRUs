﻿using System;
using System.Linq.Expressions;
using System.Text;
using FluentAssertions;
using RuleEngine.Utils;
using Xunit;
using Xunit.Abstractions;

namespace RuleEngine.Tests.Utils.ExressionExtensions
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

            var typeIsExpression = Expression.TypeIs(param, typeof(int));
            _testOutputHelper.WriteLine($"typeIsExpression: {typeIsExpression}");

            var sb = new StringBuilder();
            typeIsExpression.TraceNode(sb);
            _testOutputHelper.WriteLine(sb.ToString());

            var lambda = Expression.Lambda<Func<object, bool>>(typeIsExpression, param);
            var compiled = lambda.Compile(); // compiled becomes p=>p is int;
            compiled.Should().NotBeNull();

            compiled("blah").Should().BeFalse();
            compiled(5).Should().BeTrue();
            compiled((object) 55).Should().BeTrue();
        }
    }
}