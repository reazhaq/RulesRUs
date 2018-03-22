using System;
using FluentAssertions;
using RuleFactory.RulesFactory;
using Xunit;
using Xunit.Abstractions;

namespace RuleFactory.Tests.RulesFactory
{
    public class ConstantRulesFactoryTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ConstantRulesFactoryTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void CreateConstantRuleTest1()
        {
            var rule = ConstantRulesFactory.CreateConstantRule<int>("55");
            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(rule)}:{Environment.NewLine}" +
                                        $"{rule.ExpressionDebugView()}");

            var value = rule.Get();
            _testOutputHelper.WriteLine($"expected: 55 - actual: {value}");
            value.Should().Be(55);
        }

        [Fact]
        public void CreateConstantRuleTest2()
        {
            var rule = ConstantRulesFactory.CreateConstantRule<double>("99.1");
            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(rule)}:{Environment.NewLine}" +
                                        $"{rule.ExpressionDebugView()}");

            var value = rule.Get();
            _testOutputHelper.WriteLine($"expected: 99.1 - actual: {value}");
            value.Should().Be(99.1);
        }

        [Fact]
        public void CreateConstantRuleTest3()
        {
            var stringValue = "55";
            var rule = ConstantRulesFactory.CreateConstantRule<int, string>(stringValue);
            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(rule)}:{Environment.NewLine}" +
                                        $"{rule.ExpressionDebugView()}");

            var value = rule.Get(int.MinValue);
            _testOutputHelper.WriteLine($"expected: {stringValue} - actual: {value}");
            value.Should().BeOfType<string>().And.Be(stringValue);
        }

        [Fact]
        public void CreateConstantRuleTest4()
        {
            var rule = ConstantRulesFactory.CreateConstantRule<int, bool?>("null");
            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(rule)}:{Environment.NewLine}" +
                                        $"{rule.ExpressionDebugView()}");

            var value = rule.Get(int.MinValue);
            value.Should().Be(default(bool?));
        }

        [Fact]
        public void CreateConstantRuleTest5()
        {
            var rule = ConstantRulesFactory.CreateConstantRule<int, bool?>("false");
            var compileResult = rule.Compile();
            compileResult.Should().BeTrue();
            _testOutputHelper.WriteLine($"{nameof(rule)}:{Environment.NewLine}" +
                                        $"{rule.ExpressionDebugView()}");

            var value = rule.Get(int.MinValue);
            value.Should().Be(false);
        }
    }
}