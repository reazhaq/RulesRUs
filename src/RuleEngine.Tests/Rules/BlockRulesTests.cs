using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using FluentAssertions;
using RuleEngine.Rules;
using RuleEngine.Utils;
using Xunit;
using Xunit.Abstractions;

namespace RuleEngine.Tests.Rules
{
    public class BlockRulesTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public BlockRulesTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        //[Fact]
        //public void ThreeExpressionInAGroup()
        //{
        //    var exp1 = new ExpressionActionRule<int>(i => ShowValue(i));
        //    var exp2 = new ExpressionActionRule<int>(i => ShowValue(i));
        //    var exp3 = new ExpressionActionRule<int>(i => ShowValue(i));

        //    var blockRule = new VoidBlockRule<int>();
        //    blockRule.Rules.Add(exp1);
        //    blockRule.Rules.Add(exp2);
        //    blockRule.Rules.Add(exp3);

        //    var compileResult = blockRule.Compile();
        //    compileResult.Should().BeTrue();
        //    //_testOutputHelper.WriteLine($"blockRule:{Environment.NewLine}" +
        //    //                            $"{blockRule.ExpressionDebugView()}");

        //    blockRule.Exectue(99);
        //}

        //private void ShowValue<T>(T param) =>_testOutputHelper.WriteLine($"value = {param}");

        //[Fact]
        //public void Blah()
        //{
        //    Expression<Action<int>> exp1 = i => ShowValue(i);
        //    Expression<Action<int>> exp2 = i => ShowValue(i);
        //    Expression<Action<int>> exp3 = i => ShowValue(i);

        //    //var foo = Expression.Block(Expression.Variable(typeof(int)), exp1, exp2, exp3);
        //    var foo = Expression.Block(exp1, exp2, exp3);

        //    var sb = new StringBuilder();
        //    foo.TraceNode(sb);
        //    _testOutputHelper.WriteLine(sb.ToString());

        //    var param = Expression.Parameter(typeof(int));
        //    var boo = Expression.Lambda<Action<int>>(foo, param).Compile();
        //    boo(8);
        //}

        ////[Fact]
        ////public void SingleElementBlock()
        ////{
        ////    Type type = 5.GetType();
        ////    ConstantExpression constant = Expression.Constant(5, type);

        ////    var d = Enumerable.Repeat(Expression.Variable(typeof(int)), 1);

        ////    BlockExpression block = Expression.Block(
        ////        d,
        ////        constant
        ////    );

        ////    Assert.Equal(type, block.Type);

        ////    Expression equal = Expression.Equal(constant, block);
        ////    var a = Expression.Lambda<Func<bool>>(equal).Compile();
        ////    Assert.True(a());

        ////    var b = Expression.Lambda<Func<int>>(block).Compile();
        ////    var c = b();
        ////}

        //private static IEnumerable<ParameterExpression> SingleParameter
        //{
        //    get { return Enumerable.Repeat(Expression.Variable(typeof(int)), 1); }
        //}

        //[Fact]
        //public void SingleElementBlock2()
        //{
        //    object value = 5;
        //    Type type = value.GetType();
        //    ConstantExpression constant = Expression.Constant(value, type);
        //    BlockExpression block = Expression.Block(
        //        SingleParameter,
        //        constant
        //    );

        //    Assert.Equal(type, block.Type);

        //    Expression equal = Expression.Equal(constant, block);
        //    var foo = Expression.Lambda<Func<bool>>(equal).Compile();

        //    var sb = new StringBuilder();
        //    block.TraceNode(sb);
        //    _testOutputHelper.WriteLine(sb.ToString());

        //    sb.Clear();
        //    equal.TraceNode(sb);
        //    _testOutputHelper.WriteLine(sb.ToString());

        //    var moo = foo();
        //}
    }
}