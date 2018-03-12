using System;
using System.Collections.Generic;
using FluentAssertions;
using RuleEngine.Rules;
using RuleFactory.Factory;
using Xunit;
using Xunit.Abstractions;

namespace RuleFactory.Tests.Factory
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
            var propValueDictionary = new Dictionary<string, string>
            {
                {"TypeName", "System.String"},
                {"Value", "one"}
            };
            var rule = ConstantRuleFactories.CreateConstantRule(propValueDictionary);
            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"expression:{Environment.NewLine}{rule.ExpressionDebugView()}");

            var result = (rule as ConstantRule<string>)?.Get();
            _testOutputHelper.WriteLine($"expected: one - actual: {result}");
            result.Should().BeOfType<string>().And.Be("one");
        }

        [Fact]
        public void CreateConstantRuleFromPrimitiveTypeAndStringTest()
        {
            var rule = ConstantRuleFactories.CreateConstantRuleFromPrimitiveTypeAndString("System.String", "two");
            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"expression:{Environment.NewLine}{rule.ExpressionDebugView()}");

            var result = (rule as ConstantRule<string>)?.Get();
            _testOutputHelper.WriteLine($"expected: two - actual: {result}");
            result.Should().BeOfType<string>().And.Be("two");
        }
    }
}