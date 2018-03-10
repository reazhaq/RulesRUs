using System;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace RuleFactory.Tests
{
    public class ConstantRuleFactoriesTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ConstantRuleFactoriesTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void StringConstantRuleCreatedUsingFactory()
        {
            var rule = ConstantRuleFactories.CreateConstantRule<string>("one");
            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"expression:{Environment.NewLine}{rule.ExpressionDebugView()}");

            var result = rule.Get();
            _testOutputHelper.WriteLine($"expected: one - actual: {result}");
            result.Should().BeOfType<string>().And.Be("one");
        }

        //[Fact]
        //public void StringConstantRuleCreatedUsingFactoryThatTakesTypeName()
        //{
        //    var rule = ConstantRuleFactories.CreateConstantRule("system.string", "two");

        //    var compileResult = rule.Compile();
        //    compileResult.Should().BeTrue();
        //    _testOutputHelper.WriteLine($"expression:{Environment.NewLine}{rule.ExpressionDebugView()}");

        //    var result = rule.Get();
        //    _testOutputHelper.WriteLine($"expected: one - actual: {result}");
        //    result.Should().BeOfType<string>().And.Be("two");
        //}
    }
}